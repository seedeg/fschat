using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FsChat.Contracts.Data.Response
{
    [DataContract]
    public class GenericResponse
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "successMessage")]
        public string SuccessMessage { get; set; }

        [DataMember(Name = "warningMessage")]
        public string WarningMessage { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
