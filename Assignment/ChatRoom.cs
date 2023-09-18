using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class ChatRoom
    {
        public string RoomName { get; set; }
        public List<string> Participants { get; } = new List<string>();

        public ChatRoom(string roomName)
        {
            RoomName = roomName;
        }
    }
}



