using FsChat.Interfaces;
using FsChat.Interfaces.Logging;
using FsChat.Providers.Chat.Agents;
using FsChat.Providers.Chat.Interfaces;
using FsChat.Providers.Chat.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Providers.Chat
{
    //TODO for now everything is in memory. In a real implementation we would persist the agents and chats to a database and 
    //create entities, DTOs and their respective mappers accordingly
    public class AgentChatCoordinatorService : IAgentChatCoordinatorService
    {
        // At the moment the service handles 1 team (as per the diagram)
        // Ideally this is improved in the future so it can handle multiple teams
        private readonly ConcurrentDictionary<Guid, SupportAgent> team = new ConcurrentDictionary<Guid, SupportAgent>();
        private readonly ConcurrentDictionary<Guid, Guid> sessionAgents = new ConcurrentDictionary<Guid, Guid>();
        private readonly ILogProvider logProvider;

        public AgentChatCoordinatorService(
            ILogProvider logProvider)
        {
            this.logProvider = logProvider ?? throw new ArgumentNullException(nameof(logProvider));

            var expiredSessionsTimer = Observable.Timer(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10)).Subscribe(ProcessExpiredSessionsTick);
        }

        public void AddAgentToTeam(SupportAgent agent)
        {
            team.AddOrUpdate(agent.AgentId, agent, (key, oldValue) => agent);
        }

        public int GetTeamCapacity()
        {
            var juniors = team.Values.Where(x => x is JuniorSupportAgent);
            var midLevels = team.Values.Where(x => x is MidLevelSupportAgent);
            var seniors = team.Values.Where(x => x is SeniorSupportAgent);
            var teamLeads = team.Values.Where(x => x is TeamLeadSupportAgent);

            var capacity = 0m;

            if (juniors.Any()) capacity += juniors.Count() * ChatSettings.MaxChatConcurrency * juniors.First().Capacity;
            if (midLevels.Any()) capacity += midLevels.Count() * ChatSettings.MaxChatConcurrency * midLevels.First().Capacity;
            if (seniors.Any()) capacity += seniors.Count() * ChatSettings.MaxChatConcurrency * seniors.First().Capacity;
            if (teamLeads.Any()) capacity += teamLeads.Count() * ChatSettings.MaxChatConcurrency * teamLeads.First().Capacity;

            capacity *= ChatSettings.QueueSizeMultiplier;
            return (int)capacity;
        }

        public void AssignChatToNextAvailableAgent(ChatSession chatSession)
        {
            var nextJuniorAvailable = team.Values.Where(x => x is JuniorSupportAgent && x.ChatSessionQ.Count() < x.Capacity * 10).OrderBy(m => m.ChatSessionQ.Count()).FirstOrDefault();
            if (nextJuniorAvailable != null)
            {
                nextJuniorAvailable.ChatSessionQ.AddOrUpdate(chatSession.ChatSessionId, chatSession, (key, oldValue) => chatSession);
                sessionAgents.AddOrUpdate(chatSession.ChatSessionId, nextJuniorAvailable.AgentId, (key, oldValue) => nextJuniorAvailable.AgentId);
                logProvider.Trace(MethodBase.GetCurrentMethod().Name, $"Assigned Chat Session ID: {chatSession.ChatSessionId} to Junior Agent ID: {nextJuniorAvailable.AgentId}");
                return;
            }

            var nextMidLevelAvailable = team.Values.Where(x => x is MidLevelSupportAgent && x.ChatSessionQ.Count() < x.Capacity * 10).OrderBy(m => m.ChatSessionQ.Count()).FirstOrDefault(); ;
            if (nextMidLevelAvailable != null)
            {
                nextMidLevelAvailable.ChatSessionQ.AddOrUpdate(chatSession.ChatSessionId, chatSession, (key, oldValue) => chatSession);
                sessionAgents.AddOrUpdate(chatSession.ChatSessionId, nextMidLevelAvailable.AgentId, (key, oldValue) => nextMidLevelAvailable.AgentId);
                logProvider.Trace(MethodBase.GetCurrentMethod().Name, $"Assigned Chat Session ID: {chatSession.ChatSessionId} to Mid Level Agent ID: {nextMidLevelAvailable.AgentId}");
                return;
            }

            var nextSeniorAvailable = team.Values.Where(x => x is SeniorSupportAgent && x.ChatSessionQ.Count() < x.Capacity * 10).OrderBy(m => m.ChatSessionQ.Count()).FirstOrDefault(); ;
            if (nextSeniorAvailable != null)
            {
                nextSeniorAvailable.ChatSessionQ.AddOrUpdate(chatSession.ChatSessionId, chatSession, (key, oldValue) => chatSession);
                sessionAgents.AddOrUpdate(chatSession.ChatSessionId, nextSeniorAvailable.AgentId, (key, oldValue) => nextSeniorAvailable.AgentId);
                logProvider.Trace(MethodBase.GetCurrentMethod().Name, $"Assigned Chat Session ID: {chatSession.ChatSessionId} to Senior Agent ID: {nextSeniorAvailable.AgentId}");
                return;
            }

            var nextTeamLeadAvailable = team.Values.Where(x => x is TeamLeadSupportAgent && x.ChatSessionQ.Count() < x.Capacity * 10).OrderBy(m => m.ChatSessionQ.Count()).FirstOrDefault(); ;
            if (nextTeamLeadAvailable != null)
            {
                nextTeamLeadAvailable.ChatSessionQ.AddOrUpdate(chatSession.ChatSessionId, chatSession, (key, oldValue) => chatSession);
                sessionAgents.AddOrUpdate(chatSession.ChatSessionId, nextTeamLeadAvailable.AgentId, (key, oldValue) => nextTeamLeadAvailable.AgentId);
                logProvider.Trace(MethodBase.GetCurrentMethod().Name, $"Assigned Chat Session ID: {chatSession.ChatSessionId} to Team Lead Agent ID: {nextTeamLeadAvailable.AgentId}");
                return;
            }
        }

        public void EndAgentChat(Guid chatSessionId)
        {
            if (sessionAgents.TryGetValue(chatSessionId, out Guid sessionAgentId))
            {
                if (team.TryGetValue(sessionAgentId, out SupportAgent agent))
                {
                    if (agent.ChatSessionQ.TryRemove(chatSessionId, out ChatSession chatSession))
                    {
                        logProvider.Trace(MethodBase.GetCurrentMethod().Name, $"End Chat Session ID: {chatSessionId}");
                    }
                    if (sessionAgents.TryRemove(chatSessionId, out Guid removedValue))
                    {
                        logProvider.Trace(MethodBase.GetCurrentMethod().Name, $"End Chat Session ID: {chatSessionId} from Agent {agent.Seniority} ID: {agent.AgentId}");
                    }
                }
            }
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


        private void RemoveExpiredSessions()
        {
            foreach (var teamMember in team.Values)
            {
                var expiredSessionsToRemove = teamMember.ChatSessionQ.Where(x => DateTime.UtcNow.Subtract(x.Value.LastHeartbeatDate).TotalSeconds > ChatSettings.ChatSessionTimeoutSeconds).ToList();
                foreach (var expiredSessionToRemove in expiredSessionsToRemove)
                {
                    if (teamMember.ChatSessionQ.TryRemove(expiredSessionToRemove.Key, out ChatSession removedSession))
                    {
                        logProvider.Trace(MethodBase.GetCurrentMethod().Name, $"Removed Session ID {removedSession.ChatSessionId}");
                    }
                }
            }
        }
    }
}
