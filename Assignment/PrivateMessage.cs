using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Assignment
{
    [DataContract]
    [Serializable]
    public class PrivateMessage
    {
        [DataMember]
        public string Sender { get; set; }
        [DataMember]
        public string Receiver { get; set; }
        [DataMember]
        public string Content { get; set; }
    }
}
