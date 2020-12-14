using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FsChat.Contracts.Data.Enums;

namespace FsChat.Providers.Chat.Agents
{
    public abstract class SupportAgent
    {
        public SupportAgent(Guid agentId)
        {
            AgentId = agentId;
            ChatSessionQ = new ConcurrentDictionary<Guid, ChatSession>();
        }

        public decimal Capacity { get; protected set; }
        public Guid AgentId { get; }
        public SupportAgentSeniority Seniority { get; protected set; }
        public ConcurrentDictionary<Guid, ChatSession> ChatSessionQ { get; set; }
    }
}
