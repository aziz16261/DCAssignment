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
        public List<FileStore> Files { get; set; }
        [DataMember]
        public bool IsPrivate { get; set; }

        public ChatRoom(string roomName)
        {
            RoomName = roomName;
            Participants = new List<string>();
            Messages = new List<Message>();
            Files = new List<FileStore>();
            PrivateMessages = new List<PrivateMessage>();
            IsPrivate = false;
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

        public List<PrivateMessage> GetPrivateMessages(string sender, string receiver)
        {
            return Messages
                .Where(msg =>
                    (msg.Sender == sender && Participants.Contains(receiver)) ||
                    (msg.Sender == receiver && Participants.Contains(sender)))
                .Select(msg => new PrivateMessage { Sender = msg.Sender, Content = msg.Content })
                .ToList();
        }
    }
}