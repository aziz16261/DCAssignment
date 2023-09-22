using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Assignment;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel.Dispatcher;

namespace Console1
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DataServer : DataServerInterface
    {
        private DatabaseClass database;

        public List<ChatRoom> ChatRoomsList { get; set; } = new List<ChatRoom>();

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


            return false;
        }

        public void CreateAccount(string username)
        {

            if (!CheckAccount(username))
            {
                Console.WriteLine("Logging in as: " + username);
                Console.WriteLine("Added account " + username);
                database.AddUser(username);
            }
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
            if (!ChatRoomsList.Any(room => room.RoomName == roomName))
            {
                ChatRoomsList.Add(new ChatRoom(roomName));
                Console.WriteLine("New chat room created: " + roomName);
            }
            else
            {
                Console.WriteLine("Chat room with the same name already exists: " + roomName);
            }

            return ChatRoomsList; 
        }

        public List<ChatRoom> JoinChatRoom(string roomName, string username, List<ChatRoom> chatRoomsList)
        {
            ChatRoom chatRoom = ChatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

            if (chatRoom != null)
            {
                Console.WriteLine("Found chat room: " + chatRoom.RoomName);

                if (!chatRoom.Participants.Contains(username))
                {
                    chatRoom.Participants.Add(username);
                    Console.WriteLine("Added participant: " + username);
                    return ChatRoomsList;
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
            ChatRoom chatRoom = ChatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

            if (chatRoom != null)
            {
                if (chatRoom.Participants.Contains(username))
                {
                    chatRoom.Participants.Remove(username);
                    Console.WriteLine("removed participant from chat room: " + username);
                    return ChatRoomsList;
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

                if (!ChatRoomsList.Any(room => room.RoomName == roomName))
                {
                    ChatRoom newChatRoom = new ChatRoom(roomName);
                    ChatRoomsList.Add(newChatRoom);
                }
            }

            return ChatRoomsList;
        }
        public List<ChatRoom> SendMessage(string sender, string roomName, string message, List<ChatRoom> chatRoomsList)
        {
            ChatRoom chatRoom = ChatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

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
                    return ChatRoomsList;
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

         public List<ChatRoom> GetChatRooms() { return ChatRoomsList; }

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

        /// <summary>
        /// This code below is for handling the file sharing, it doesn't work just yet, probably needs the buttons to be on seperate threads
        /// </summary>
        public string UploadFile(string fileName, ChatRoom currentChatroom)
        {
            Console.WriteLine("Started upload process");
            byte[] fileData = File.ReadAllBytes(fileName);
            
            FileStore newFile = new FileStore(fileName, fileData);
            currentChatroom.Files.Add(newFile);
            Console.WriteLine("Successfully added file " + fileName);
            return fileName;
        }

    }
}