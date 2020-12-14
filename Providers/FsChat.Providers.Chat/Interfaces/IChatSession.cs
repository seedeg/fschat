using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Providers.Chat.Interfaces
{
    public interface IChatSession
    {
        void UpdateHeartbeat();
        void MarkAsProcessing();
        void MarkAsComplete();
        void MarkAsTimedOut();
    }
}
