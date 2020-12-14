using FsChat.Providers.Chat.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FsChat.Contracts.Data.Enums;

namespace FsChat.Providers.Chat
{
    public class ChatSession : IChatSession
    {
        public Guid ChatSessionId { get; }
        public ChatStatus ChatStatus { get; private set; }
        public DateTime LastHeartbeatDate { get; private set; }


        public ChatSession(Guid chatSessionId)
        {
            ChatSessionId = chatSessionId;
            LastHeartbeatDate = DateTime.UtcNow;
            ChatStatus = ChatStatus.Queued;
        }

        public void UpdateHeartbeat()
        {
            LastHeartbeatDate = DateTime.UtcNow;
        }

        public void MarkAsProcessing()
        {
            ChatStatus = ChatStatus.Processing;
        }

        public void MarkAsComplete()
        {
            ChatStatus = ChatStatus.Complete;
        }

        public void MarkAsTimedOut()
        {
            ChatStatus = ChatStatus.TimedOut;
        }
    }
}
