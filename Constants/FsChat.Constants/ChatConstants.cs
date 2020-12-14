using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Constants
{
    public static class ChatConstants
    {
        public static short MaxChatConcurrency = 10;
        public static decimal QueueSizeMultiplier = 1.5m;
        public static short ChatSessionTimeoutSeconds = 15; // Did it more than 3 seconds to faciliate testing with debugging
    }
}
