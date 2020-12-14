using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Contracts.Data
{
    public class Enums
    {
        [DataContract]
        public enum SupportAgentSeniority
        {
            [EnumMember]
            Junior = 1,

            [EnumMember]
            MidLevel = 2,

            [EnumMember]
            Senior = 3,

            [EnumMember]
            TeamLead = 4
        }

        [DataContract]
        public enum ChatStatus
        {
            [EnumMember]
            Queued = 1,

            [EnumMember]
            Processing = 2,

            [EnumMember]
            Complete = 3,

            [EnumMember]
            TimedOut = 4
        }
    }
}
