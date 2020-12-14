using FsChat.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Providers.Chat.Agents
{
    public class SeniorSupportAgent : SupportAgent
    {
        public SeniorSupportAgent(Guid agentId) : base(agentId)
        {
            Seniority = Enums.SupportAgentSeniority.Senior;
            Capacity = 0.8m;
        }
    }
}
