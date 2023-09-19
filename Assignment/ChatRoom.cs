using Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    [DataContractAttribute]
    public class ChatRoom
    {
        public string RoomName { get; set; }
        public List<string> Participants { get; } = new List<string>();
        public List<Message> Messages { get; set; }


        public ChatRoom(string roomName)
        {
            RoomName = roomName;
        }

        public List<string> GetParticipants()
        {
            return Participants;
        }
    }
}



