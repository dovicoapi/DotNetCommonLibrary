using System.Net;
using System.Runtime.Serialization;

namespace Dovico.CommonLibrary
{
    [DataContract(Name = "Error", Namespace = "")]
    public class WebFaultExceptionDetails
    {
        public WebFaultExceptionDetails(HttpStatusCode iStatusCode, string sDescription)
        {
            Status = iStatusCode.ToString();
            Description = sDescription;
        }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}