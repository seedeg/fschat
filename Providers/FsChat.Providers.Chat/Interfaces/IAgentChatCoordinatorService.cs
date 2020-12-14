using FsChat.Providers.Chat;
using FsChat.Providers.Chat.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Providers.Chat.Interfaces
{
    public interface IAgentChatCoordinatorService
    {
        void AddAgentToTeam(SupportAgent agent);
        int GetTeamCapacity();
        void AssignChatToNextAvailableAgent(ChatSession chatSession);
        void EndAgentChat(Guid sessionId);
    }
}
