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
using System.Runtime.InteropServices;

namespace Console1
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DataServer : DataServerInterface
    {
        public static List<ChatRoom> ChatRoomsList { get; private set; } = new List<ChatRoom>();
        public event Action<List<ChatRoom>> ChatRoomsUpdated;
        public static List<Username> UsernamesList { get; private set; } = new List<Username>();

        public bool CheckAccount(string username)
        {
            if (UsernamesList.Any(user => user.Name == username))
            {
                Console.WriteLine("User exists: " + username);
                return true;
            }

            return false;
        }

        public void CreateAccount(string username)
        {
            if (!CheckAccount(username))
            {
                Console.WriteLine("Logging in as: " + username);
                Console.WriteLine("Added account " + username);

                Username newUser = new Username(username);
                UsernamesList.Add(newUser);
            }
        }

        public void RemoveAccount(string username)
        {
            var userToRemove = UsernamesList.FirstOrDefault(user => user.Name == username);

            if (userToRemove != null)
            {
                Console.WriteLine("Logged out: " + username);
                UsernamesList.Remove(userToRemove);
            }
            else
            {
                Console.WriteLine("Username not found: " + username);
            }
        }

        public List<ChatRoom> CreateChatRoom(string roomName, List<ChatRoom> chatRoomsList)
        {
            if (!DataServer.ChatRoomsList.Any(room => room.RoomName == roomName))
            {
                DataServer.ChatRoomsList.Add(new ChatRoom(roomName));
                Console.WriteLine("New chat room created: " + roomName);
            }
            else
            {
                Console.WriteLine("Chat room with the same name already exists: " + roomName);
            }

            ChatRoomsUpdated?.Invoke(DataServer.ChatRoomsList);

            return DataServer.ChatRoomsList;
        }


        public List<ChatRoom> JoinChatRoom(string roomName, string username, List<ChatRoom> chatRoomsList)
        {
            ChatRoom chatRoom = DataServer.ChatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

            if (chatRoom != null)
            {
                Console.WriteLine("Found chat room: " + chatRoom.RoomName);

                if (!chatRoom.Participants.Contains(username))
                {
                    chatRoom.Participants.Add(username);
                    Console.WriteLine("Added participant: " + username);
                }
                else
                {
                    Console.WriteLine("Participant already exists " + username);
                }

                ChatRoomsUpdated?.Invoke(DataServer.ChatRoomsList);
            }
            else
            {
                Console.WriteLine("Chat room not found: " + roomName);
            }

            return ChatRoomsList;
        }
        public List<ChatRoom> LeaveChatRoom(string roomName, string username, List<ChatRoom> chatRoomsList)
        {
            ChatRoom chatRoom = DataServer.ChatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

            if (chatRoom != null)
            {
                if (chatRoom.Participants.Contains(username))
                {
                    chatRoom.Participants.Remove(username);
                    Console.WriteLine("Removed participant from chat room: " + username);
                }
                else
                {
                    Console.WriteLine("Participant is not in chat room: " + username);
                }

                ChatRoomsUpdated?.Invoke(DataServer.ChatRoomsList);
            }

            return ChatRoomsList.Where(room => room.Participants.Contains(username) || !room.IsPrivate).ToList();
        }

        public List<ChatRoom> CreateInitialChatRooms(List<ChatRoom> chatRoomsList)
        {
            for (int i = 1; i <= 3; i++)
            {
                string roomName = "ChatRoom " + i;

                if (!DataServer.ChatRoomsList.Any(room => room.RoomName == roomName))
                {
                    ChatRoom newChatRoom = new ChatRoom(roomName);
                    DataServer.ChatRoomsList.Add(newChatRoom);
                }
            }

            ChatRoomsUpdated?.Invoke(DataServer.ChatRoomsList);

            return ChatRoomsList;
        }

        public List<ChatRoom> SendMessage(string sender, string roomName, string message, List<ChatRoom> chatRoomsList)
        {
            ChatRoom chatRoom = DataServer.ChatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

            if (chatRoom != null)
            {
                if (chatRoom.Participants.Contains(sender))
                {
                    Message newMessage = new Message
                    {
                        Sender = sender,
                        Content = message
                    };

                    Console.WriteLine("Sender: " + sender + "\nMessage: " + message);
                    chatRoom.Messages.Add(newMessage);

                    ChatRoomsUpdated?.Invoke(DataServer.ChatRoomsList);
                }
                else
                {
                    Console.WriteLine("Sender is not a participant in the chat room: " + sender);
                }
            }
            else
            {
                Console.WriteLine("Chat room not found: " + roomName);
            }

            return DataServer.ChatRoomsList;
        }

        public List<ChatRoom> GetChatRooms(string username)
        {
            try { 
                return ChatRoomsList.Where(room => room.Participants.Contains(username) || !room.IsPrivate).ToList();
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public List<ChatRoom> GetChatRoomss()
        {
            return ChatRoomsList;
        }


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

        public void SendPrivateMessage(string sender, string receiver, string message)
        {
            string privateRoomName = $"{sender}_{receiver}";

            ChatRoom privateChatRoom = ChatRoomsList.FirstOrDefault(room => room.RoomName == privateRoomName);
            if (privateChatRoom == null)
            {
                CreatePrivateChatRoom(sender, receiver);
                privateChatRoom = ChatRoomsList.FirstOrDefault(room => room.RoomName == privateRoomName);
            }

            if (privateChatRoom != null)
            {
                Message newMessage = new Message
                {
                    Sender = sender,
                    Content = message
                };

                privateChatRoom.Messages.Add(newMessage);
                Console.WriteLine(sender + " " + newMessage);
                ChatRoomsUpdated?.Invoke(ChatRoomsList);
            }
        }

        public List<ChatRoom> CreatePrivateChatRoom(string sender, string receiver)
        {
            string privateRoomName = $"{sender}_{receiver}";

            if (!ChatRoomsList.Any(room => room.RoomName == privateRoomName))
            {
                ChatRoom privateChatRoom = new ChatRoom(privateRoomName)
                {
                    IsPrivate = true
                };

                privateChatRoom.Participants.Add(sender);
                privateChatRoom.Participants.Add(receiver);

                ChatRoomsList.Add(privateChatRoom);
                Console.WriteLine("Added " + sender + ", " + receiver + " to the private room " + privateRoomName);
            }

            ChatRoomsUpdated?.Invoke(ChatRoomsList);

            return ChatRoomsList;
        }

        /// <summary>
        /// This code below is for handling the file sharing, it doesn't work just yet, probably needs the buttons to be on seperate threads
        /// </summary>
        public string UploadFile(string filePath, byte[] fileData, string currentChatroom)
        {
            Console.WriteLine("Started upload process");
            string fileName = Path.GetFileName(filePath);
            File.WriteAllBytes("localhost\\" + fileName, fileData);

            GetChatRoom(currentChatroom, ChatRoomsList).Files.Add(fileName);
            Console.WriteLine("Successfully added file " + fileName);
            return fileName;
        }
    }
}