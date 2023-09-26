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
    public class ChatRoom
    {
        [DataMember]
        public string RoomName { get; set; }
        [DataMember]
        public List<string> Participants { get; set; }
        [DataMember]
        public List<PrivateMessage> PrivateMessages { get; set; }
        [DataMember]
        public List<Message> Messages { get; set; }
        [DataMember]
        public List<string> Files { get; set; }
        [DataMember]
        public bool IsPrivate { get; set; }

        public ChatRoom(string roomName)
        {
            RoomName = roomName;
            Participants = new List<string>();
            Messages = new List<Message>();
            Files = new List<string>();
            PrivateMessages = new List<PrivateMessage>();
            IsPrivate = false;
        }

        public string GetParticipantsAsString()
        {
            string participantsList = string.Join(", ", Participants);
            return participantsList;
        }
    }
}