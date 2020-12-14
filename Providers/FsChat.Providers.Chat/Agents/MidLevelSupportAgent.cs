using FsChat.Contracts.Data;
using FsChat.Providers.Chat.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Providers.Chat.Agents
{
    public class MidLevelSupportAgent : SupportAgent
    {
        public MidLevelSupportAgent(Guid agentId) : base(agentId)
        {
            Seniority = Enums.SupportAgentSeniority.MidLevel;
            Capacity = AgentSettings.MidLevelCapacity;
        }
    }
}
