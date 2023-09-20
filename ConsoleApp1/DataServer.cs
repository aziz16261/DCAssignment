using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Assignment;
using System.Diagnostics;

namespace Console1
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DataServer : DataServerInterface
    {
        private DatabaseClass database;

       // private List<ChatRoom> chatRoomsList = new List<ChatRoom>();
        public DataServer()
        {
            database = new DatabaseClass();
        }

        public bool CheckAccount(string username)
        {
            List<DatabaseStorage.DataStruct> dataStructList = database.GetDataStructList();

            foreach (var data in dataStructList)
            {
                if (data.AcctUsername == username)
                {
                    Console.WriteLine("username already exists: " + username);
                    return true; 
                }
            }

            Console.WriteLine("Logged in as: " + username);
            return false; 
        }

        public void RemoveAccount(string username)
        {
            List<DatabaseStorage.DataStruct> dataStructList = database.GetDataStructList();

            foreach (var data in dataStructList)
            {
                if (data.AcctUsername == username)
                {
                    Console.WriteLine("logged out: " + username);
                    database.RemoveUser(username);
                    return; 
                }
            }
        }

        public List<ChatRoom> CreateChatRoom(string roomName, List<ChatRoom> chatRoomsList)
        {
            if (!chatRoomsList.Any(room => room.RoomName == roomName))
            {
                chatRoomsList.Add(new ChatRoom(roomName));
                Console.WriteLine("new chat room created: " + roomName);
                return chatRoomsList;
            }

            return null;
        }

        public List<ChatRoom> JoinChatRoom(string roomName, string username, List<ChatRoom> chatRoomsList)
        {
            ChatRoom chatRoom = chatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

            if (chatRoom != null)
            {
                Console.WriteLine("Found chat room: " + chatRoom.RoomName);

                if (!chatRoom.Participants.Contains(username))
                {
                    chatRoom.Participants.Add(username);
                    Console.WriteLine("Added participant: " + username);
                    return chatRoomsList;
                }
                else
                {
                    Console.WriteLine("Participant already exists " + username);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Chat room not found: " + roomName);
                return null;
            }
        }

        public List<ChatRoom> LeaveChatRoom(string roomName, string username, List<ChatRoom> chatRoomsList)
        {
            ChatRoom chatRoom = chatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

            if (chatRoom != null)
            {
                if (chatRoom.Participants.Contains(username))
                {
                    chatRoom.Participants.Remove(username);
                    Console.WriteLine("removed participant from chat room: " + username);
                    return chatRoomsList;
                }
                else
                {
                    Console.WriteLine("participant is not in chat room: " + username);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public List<ChatRoom> CreateInitialChatRooms(List<ChatRoom> chatRoomsList)
        {
            for (int i = 1; i <= 3; i++)
            {
                string roomName = "ChatRoom " + i;

                if (!chatRoomsList.Any(room => room.RoomName == roomName))
                {
                    ChatRoom newChatRoom = new ChatRoom(roomName);
                    chatRoomsList.Add(newChatRoom);
                }
            }

            return chatRoomsList;
        }
        public List<ChatRoom> SendMessage(string sender, string roomName, string message, List<ChatRoom> chatRoomsList)
        {
            ChatRoom chatRoom = chatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

            if (chatRoom != null)
            {
                if (chatRoom.Participants.Contains(sender))
                {
                    Message newMessage = new Message
                    {
                        Sender = sender,
                        Content = message
                    };

                    Console.WriteLine("sender: " + sender + "\nmessage: " + message);
                    chatRoom.Messages.Add(newMessage);
                    return chatRoomsList;
                }

                else
                {
                    return null;
                }
            }

            else
            {
                return null;
            }
        }

       // public List<ChatRoom> GetChatRooms() { return chatRoomsList; }

        public ChatRoom GetChatRoom(string roomName, List<ChatRoom> chatRoomsList)
        {
            ChatRoom chatRoom = null;

            foreach (ChatRoom room in chatRoomsList)
            {
                if (room.RoomName == roomName)
                {
                    Console.WriteLine((room.RoomName));
                    chatRoom = room;
                    return chatRoom;
                }
            }

            return chatRoom;
        }

    }
}