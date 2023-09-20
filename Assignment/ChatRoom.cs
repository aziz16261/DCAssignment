using Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    [DataContract]
    [Serializable]
    public class ChatRoom
    {
        [DataMember]
        public string RoomName { get; set; }
        [DataMember]
        public List<string> Participants { get; set; }
        [DataMember]
        public List<Message> Messages { get; set; }

        public ChatRoom(string roomName)
        {
            RoomName = roomName;
            Participants = new List<string>();
            Messages = new List<Message>();
        }

        public List<string> GetParticipants()
        {
            return Participants;
        }

        public string GetParticipantsAsString()
        {
            string participantsList = string.Join(", ", Participants);
            return participantsList;
        }
    }
}



