using Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DatabaseLib
{
    [DataContract]
    [Serializable]
    public class Username
    {
        [DataMember]
        public string Name { get; set; }
        public Boolean IsLoggedIn { get; set; }

        public Username(string name)
        {
            Name = name;
            IsLoggedIn = true;
        }
    }
}