using FsChat.Interfaces.Logging;
using FsChat.Providers.Chat.Agents;
using FsChat.Providers.Chat.Interfaces;
using FsChat.Providers.Chat.Settings;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace FsChat.Providers.Chat
{
    public class ChatSessionQueue : IChatSessionQueue
    {
        public ChatSessionQueue(
            ILogProvider logProvider,
            IAgentChatCoordinatorService agentChatCoordinatorService)
        {
            this.logProvider = logProvider ?? throw new ArgumentNullException(nameof(logProvider));
            this.agentChatCoordinatorService = agentChatCoordinatorService ?? throw new ArgumentNullException(nameof(agentChatCoordinatorService));

            queue = new ConcurrentDictionary<Guid, ChatSession>();

            SetupSupportTeam();

            //TODO the timer should ideally be in a seperate worker service so the system is more modular.
            //One disadvantage of having this here is that in our case, the session queue is being initialised by the API and therefore
            //the API does not remain stateless. This would pose problems if we were to horizontally scale the API.
            //It has been done here for now to have a faster implementation and due to the fact that we are keeping all data structures in memory and not persisting them
            //Since it is in the API's assembly's memory, we cannot access it from another binary (i.e. the hypothetical worker)
            var expiredSessionsTimer = Observable.Timer(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)).Subscribe(ProcessExpiredSessionsTick);
            var chatAssignmentsTimer = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)).Subscribe(ProcessChatSessionAssignmentTick);
        }

        private ConcurrentDictionary<Guid, ChatSession> queue;
        private readonly ILogProvider logProvider;
        private readonly IAgentChatCoordinatorService agentChatCoordinatorService;

        public ChatSession Dequeue()
        {
            if (queue.Any())
            {
                var nextChat = queue.FirstOrDefault(x => x.Value.ChatStatus == Contracts.Data.Enums.ChatStatus.Queued).Value;
                if (nextChat != null)
                {
                    nextChat.MarkAsProcessing();
                    return nextChat;
                }
            }

            return null;
        }

        public bool Enqueue(ChatSession session)
        {
            // TODO at the moment we are not handling the concept of office hours 
            // since the requirements in the document are not clear for this part.
            // Apart from this there is a time constraint. This could easily be added
            // in the future
            if (IsFull())
            {
                return false;
            }

            queue.AddOrUpdate(session.ChatSessionId, session, (key, oldValue) => session);
            return true;
        }

        public ChatSession GetChatSession(Guid sessionId)
        {
            if (queue.TryGetValue(sessionId, out ChatSession chatSession))
            {
                return chatSession;
            }
            return null;
        }

        public void UpdateHeartbeat(Guid sessionId)
        {
            if (queue.TryGetValue(sessionId, out ChatSession chatSession))
            {
                chatSession.UpdateHeartbeat();
            }
        }

        public void RemoveSession(Guid sessionId)
        {
            if (queue.TryRemove(sessionId, out ChatSession chatSession))
            {
                agentChatCoordinatorService.EndAgentChat(sessionId);
            }
        }

        public bool IsFull()
        {
            var teamCapacity = agentChatCoordinatorService.GetTeamCapacity();
            return queue.Count() >= teamCapacity;
        }

        // TODO setting up of the team is hardcoded at the moment. This should eventually be handled from the outside
        // We can eventually support multiple teams as well since at the moment we are creating just one team
        private void SetupSupportTeam()
        {
            // Example 1
            agentChatCoordinatorService.AddAgentToTeam(new JuniorSupportAgent(Guid.NewGuid()));
            agentChatCoordinatorService.AddAgentToTeam(new SeniorSupportAgent(Guid.NewGuid()));

            // Example 2
            //agentChatCoordinatorService.AddAgentToTeam(new JuniorSupportAgent(Guid.NewGuid()));
            //agentChatCoordinatorService.AddAgentToTeam(new JuniorSupportAgent(Guid.NewGuid()));
            //agentChatCoordinatorService.AddAgentToTeam(new MidLevelSupportAgent(Guid.NewGuid()));
        }

        private void ProcessExpiredSessionsTick(long count)
        {
            try
            {
                RemoveExpiredSessions();
            }
            catch (Exception ex)
            {
                logProvider.Trace(ex, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void ProcessChatSessionAssignmentTick(long count)
        {
            try
            {
                AssignChatSessions();
            }
            catch (Exception ex)
            {
                logProvider.Trace(ex, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void RemoveExpiredSessions()
        {
            var expiredSessionsToRemove = queue.Where(x => DateTime.UtcNow.Subtract(x.Value.LastHeartbeatDate).TotalSeconds > ChatSettings.ChatSessionTimeoutSeconds).ToList();
            foreach (var expiredSessionToRemove in expiredSessionsToRemove)
            {
                if (queue.TryRemove(expiredSessionToRemove.Key, out ChatSession removedSession))
                {
                    logProvider.Trace( MethodBase.GetCurrentMethod().Name, $"Removed queued Session ID {removedSession.ChatSessionId}");
                }
            }
        }

        private void AssignChatSessions()
        {
            var nextChatSession = Dequeue();
            if (nextChatSession != null)
            {
                agentChatCoordinatorService.AssignChatToNextAvailableAgent(nextChatSession);
            }
        }
    }
}
