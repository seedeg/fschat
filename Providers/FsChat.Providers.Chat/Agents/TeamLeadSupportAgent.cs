using FsChat.Contracts.Data;
using FsChat.Providers.Chat.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Providers.Chat.Agents
{
    public class TeamLeadSupportAgent : SupportAgent
    {
        public TeamLeadSupportAgent(Guid agentId) : base(agentId)
        {
            Seniority = Enums.SupportAgentSeniority.TeamLead;
            Capacity = AgentSettings.TeamLeadCapacity;
        }
    }
}
