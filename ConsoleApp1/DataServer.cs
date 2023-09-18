using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Assignment;

namespace Console1
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DataServer : DataServerInterface
    {
        private DatabaseClass database;

        private List<ChatRoom> chatRoomsList = new List<ChatRoom>();
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
                    return true; // The account exists 
                }
            }

            database.AddUser(username);
            return false; // The account does not exist
        }

        public Boolean CreateChatRoom(string roomName)
        {
            if (!chatRoomsList.Any(room => room.RoomName == roomName))
            {
                ChatRoom newChatRoom = new ChatRoom(roomName);

                chatRoomsList.Add(newChatRoom);

                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean JoinChatRoom(string roomName, string username)
        {
            ChatRoom chatRoom = chatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

            if (chatRoom != null)
            {
                if (!chatRoom.Participants.Contains(username))
                {
                    chatRoom.Participants.Add(username);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public string LeaveChatRoom(string roomName, string username)
        {
            ChatRoom chatRoom = chatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

            if (chatRoom != null)
            {
                if (chatRoom.Participants.Contains(username))
                {
                    chatRoom.Participants.Remove(username);
                    return "Left chat room successfully";
                }
                else
                {
                    return "You are not a participant in this chat room";
                }
            }
            else
            {
                return "Chat room does not exist";
            }
        }
        public List<string> CreateInitialChatRooms()
        {
            List<string> initialChatRoomNames = new List<string>();

            string[] initialRoomNames = { "ChatRoom 1", "ChatRoom 2", "ChatRoom 3" };

            foreach (string roomName in initialRoomNames)
            {
                if (!chatRoomsList.Any(room => room.RoomName == roomName))
                {
                    ChatRoom newChatRoom = new ChatRoom(roomName);
                    chatRoomsList.Add(newChatRoom);
                    initialChatRoomNames.Add(roomName);
                }
            }

            return initialChatRoomNames;
        }

        public string SendMessage(string sender, string roomName, string message)
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

                    chatRoom.Messages.Add(newMessage);
                    return "Message sent successfully";
                }
                else
                {
                    return "You are not a participant in this chat room";
                }
            }
            else
            {
                return "Chat room does not exist";
            }
        }
    }
}