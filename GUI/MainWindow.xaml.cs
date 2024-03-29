﻿
using Assignment;
using Console1;
using DatabaseLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Reflection.Emit;
using System.Windows.Threading;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataServerInterface foob;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            ChannelFactory<DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            tcp.TransferMode = TransferMode.Streamed;

            string URL = "net.tcp://localhost:8100/ChatServer";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();

            AvailableRooms.ItemsSource = foob.CreateInitialChatRooms(new List<ChatRoom>()).Select(room => room.RoomName);

            Task.Run(() => StartUpdatingChatRooms());
            Task.Run(() => StartUpdatingChatRoomParticipants());
            Task.Run(() => StartUpdatingChatRoomMessages());
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = NBox.Text;

            bool usernameExists = foob.CheckAccount(username);

            if (usernameExists)
            {
                MessageBox.Show("User already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter username", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else
            {
                foob.CreateAccount(username);
                Username.Text = "Currently logged in as: " + username;
                LoginButton.IsEnabled = false;
                NBox.IsEnabled = false;
            }
        }

        private void LogoutButton_Click_1(object sender, RoutedEventArgs e)
        {
            string username = NBox.Text;
            bool usernameExists = foob.CheckAccount(username);

            if (usernameExists)
            {
                List<ChatRoom> chatRoomsList = foob.GetChatRooms(username);
                bool isParticipantInChatRoom = chatRoomsList.Any(room => room.Participants.Contains(username));

                if (isParticipantInChatRoom)
                {
                    MessageBox.Show("You cannot log out while you are in a chat room, Please leave the chat room first", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    foob.RemoveAccount(username);
                    Username.Text = "You have been logged out: " + username;
                    NBox.Text = "Enter a unique username to log in again";
                    LoginButton.IsEnabled = true;
                    NBox.IsEnabled = true;

                    AvailableRooms.ItemsSource = null;
                    chatBox.Text = string.Empty;
                    currentRoomParticipants.ItemsSource = null;
                    chatroom_name_Block.Text = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("User is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string roomName = roomName_txt.Text;
            bool usernameExists = foob.CheckAccount(NBox.Text);

            if (usernameExists)
            {
                List<ChatRoom> chatRoomsList = foob.GetChatRooms(NBox.Text);

                if (chatRoomsList == null)
                {
                    MessageBox.Show("Failed to retrieve chat rooms", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (chatRoomsList.Any(room => room.RoomName == roomName))
                {
                    MessageBox.Show("Failed, chat room with that name already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    chatRoomsList = foob.CreateChatRoom(roomName, chatRoomsList);

                    if (chatRoomsList != null)
                    {
                        chatRoomsList = foob.GetChatRooms(NBox.Text);

                        AvailableRooms.ItemsSource = null;
                        AvailableRooms.ItemsSource = chatRoomsList.Select(room => room.RoomName);
                    }
                    else
                    {
                        MessageBox.Show("Failed to create chat room", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Failed, user is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string username = null;
            username = NBox.Text;

            if (!string.IsNullOrEmpty(username))
            {
                if (AvailableRooms.SelectedItem != null)
                {
                    string selectedRoom = AvailableRooms.SelectedItem.ToString();

                    List<ChatRoom> chatRoomsList = foob.GetChatRooms(NBox.Text);

                    if (chatRoomsList != null)
                    {
                        ChatRoom userCurrentRoom = chatRoomsList.FirstOrDefault(room => room.Participants.Contains(username));

                        if (userCurrentRoom != null && !userCurrentRoom.IsPrivate)
                        {
                            MessageBox.Show("You are already a participant in a different room, Please leave the current room if you want to join this one", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            chatRoomsList = foob.JoinChatRoom(selectedRoom, username, chatRoomsList);

                            ChatRoom selectedChatRoom = chatRoomsList.FirstOrDefault(room => room.RoomName == selectedRoom);

                            if (selectedChatRoom != null)
                            {
                                chatroom_name_Block.Text = selectedChatRoom.RoomName;
                                Console.WriteLine("Participants: " + selectedChatRoom.GetParticipantsAsString());
                                Console.WriteLine("Participants count: " + selectedChatRoom.Participants.Count);
                                currentRoomParticipants.ItemsSource = selectedChatRoom.Participants;
                            }
                            else
                            {
                                Console.WriteLine("Selected chat room not found in chatRoomsList");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to retrieve chat rooms", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                    AvailableRooms.SelectedIndex = -1;
                    MessageBox.Show("Failed, user is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SendBox_Click(object sender, RoutedEventArgs e)
        {
            string roomName = chatroom_name_Block.Text;
            string message = MessageTextBox.Text;

            List<ChatRoom> chatRoomsList = foob.GetChatRooms(NBox.Text);

            bool usernameExists = foob.CheckAccount(NBox.Text);

            if (usernameExists)
            {
                ChatRoom userChatRoom = chatRoomsList.FirstOrDefault(room => room.Participants.Contains(NBox.Text));

                if (userChatRoom != null)
                {
                    List<ChatRoom> updatedChatRoomsList = foob.SendMessage(NBox.Text, roomName, message, chatRoomsList);

                    if (updatedChatRoomsList != null)
                    {
                        ChatRoom updatedChatRoom = updatedChatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

                        if (updatedChatRoom != null)
                        {
                            chatBox.Text = string.Join(Environment.NewLine, updatedChatRoom.Messages.Select(msg => $"{msg.Sender}: {msg.Content}"));
                            MessageTextBox.Clear();

                            chatRoomsList = updatedChatRoomsList;
                        }
                        else
                        {
                            MessageBox.Show("Chat room not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to send message", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("You are not in a chat room", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Failed, user is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LeaveChatRoom_Click(object sender, RoutedEventArgs e)
        {
            bool usernameExists = foob.CheckAccount(NBox.Text);

            if (usernameExists)
            {
                string roomName = chatroom_name_Block.Text;
                string username = NBox.Text;

                List<ChatRoom> chatRoomsList = foob.GetChatRooms(Username.Text);

                chatRoomsList = foob.LeaveChatRoom(roomName, username, chatRoomsList);

                if (chatRoomsList != null)
                {
                    chatroom_name_Block.Text = string.Empty;
                    currentRoomParticipants.ItemsSource = null;

                    AvailableRooms.ItemsSource = null;
                    AvailableRooms.ItemsSource = chatRoomsList.Select(room => room.RoomName);
                    chatBox.Clear();
                }
                else
                {
                    MessageBox.Show("Failed to leave the chat room", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("User is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// This code below is for handling the file sharing
        /// </summary>

        private void FileTransfer_Click(object sender, RoutedEventArgs e)
        {
            bool usernameExists = foob.CheckAccount(NBox.Text);
            List<ChatRoom> chatRoomsList = foob.GetChatRooms(NBox.Text);

            if (usernameExists)
            {
                ChatRoom userChatRoom = chatRoomsList.FirstOrDefault(room => room.Participants.Contains(NBox.Text));

                if (userChatRoom != null) { 
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = "txt files |*.txt|Image files|*.jpg;*.jpeg;*.png;*.bmp";
                    dialog.CheckFileExists = true;
                    dialog.CheckPathExists = true;
                    dialog.Title = "Select file to upload";
                    dialog.ShowDialog();

                    string filePath = dialog.FileName;
                    string username = NBox.Text;
                    string userCurrentRoom = chatroom_name_Block.Text;
                    if (filePath != "") { 
                        try { 
                            byte[] fileData = File.ReadAllBytes(filePath);

                            MessageTextBox.Text = username + ": sent file " + foob.UploadFile(filePath, fileData, userCurrentRoom);
                            foob.SendMessage(username, userCurrentRoom, MessageTextBox.Text, foob.GetChatRoomss());
                            MessageTextBox.Text = "";
                        } catch (IOException exc)
                        {
                            MessageBox.Show("File transfer failed due to " +  exc.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                } 
                else{
                    MessageBox.Show("You are not in a chat room", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("User is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void FileTransferSendButton_Click(object sender, RoutedEventArgs e)
        {
            FileDisplay fileWindow = new FileDisplay();
            string username = NBox.Text;
            string listonames = "";

            bool usernameExists = foob.CheckAccount(NBox.Text);
            if (usernameExists)
            {
                ChatRoom userCurrentRoom = foob.GetChatRoom(chatroom_name_Block.Text, foob.GetChatRooms(username));
                if (userCurrentRoom != null)
                {

                    if (userCurrentRoom.Files != null)
                    {
                        int count = userCurrentRoom.Files.Count;
                        listonames = count.ToString();
                        foreach (string file in userCurrentRoom.Files)
                        {

                            listonames += file + "\n";
                        }

                        fileWindow.setChatRoom(userCurrentRoom);
                        fileWindow.Show();
                    }
                }
                else
                {
                    MessageBox.Show("You are not in a chat room", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("User is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
}
        

        private void PrivateMessage_click(object sender, RoutedEventArgs e)
        {
            string senderUsername = NBox.Text;
            string receiverUsername = PMRecieverName.Text;
            string message = MessageTextBox.Text;

            bool usernameExists = foob.CheckAccount(NBox.Text);

            if (usernameExists)
            {
                if (string.IsNullOrWhiteSpace(receiverUsername))
                {
                    MessageBox.Show("Please enter a recipient's username", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (senderUsername.Equals(receiverUsername, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("You cannot send a private message to yourself.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                bool receiverExists = foob.CheckAccount(receiverUsername);
                if (!receiverExists)
                {
                    MessageBox.Show("Recipient's username does not exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foob.SendPrivateMessage(senderUsername, receiverUsername, message);

                PMRecieverName.Clear();
                MessageTextBox.Clear();
            }

            else
            {
                MessageBox.Show("User is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            string senderUsername = null;
            senderUsername = NBox.Text;
            List<ChatRoom> chatRoomsList = foob.GetChatRooms(senderUsername);

            bool usernameExists = foob.CheckAccount(senderUsername);

            if (usernameExists)
            {
                if (chatRoomsList != null)
                {
                    string chatRoomName = chatroom_name_Block.Text;
                    ChatRoom selectedChatRoom = chatRoomsList.FirstOrDefault(room => room.RoomName == chatRoomName);
                    AvailableRooms.ItemsSource = chatRoomsList.Select(room => room.RoomName);

                    if (selectedChatRoom != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            currentRoomParticipants.ItemsSource = selectedChatRoom.Participants;
                            chatBox.Text = string.Join(Environment.NewLine, selectedChatRoom.Messages.Select(msg => $"{msg.Sender}: {msg.Content}"));
                        });
                    }
                }
            }
            else
            {
                MessageBox.Show("User is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private async Task StartUpdatingChatRooms()
        {
            while (true)
            {
                await UpdateChatRoomsAsync();
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        private async Task UpdateChatRoomsAsync()
        {
            string username = null;

            Dispatcher.Invoke(() =>
            {
                username = NBox.Text;
            });

            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    List<ChatRoom> chatRoomsList = await Task.Run(() => foob.GetChatRooms(username));

                    if (chatRoomsList != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            AvailableRooms.ItemsSource = null;
                            AvailableRooms.ItemsSource = chatRoomsList.Select(room => room.RoomName);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
        private async Task StartUpdatingChatRoomParticipants()
        {
            while (true)
            {
                await UpdateChatRoomParticipantsAsync();
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }


        private async Task UpdateChatRoomParticipantsAsync()
        {
            string username = null;
            string selectedRoom = null;
            Dispatcher.Invoke(() =>
            {
                username = NBox.Text;
                selectedRoom = chatroom_name_Block.Text;
            });
            if (!string.IsNullOrEmpty(username))
            {
                List<ChatRoom> chatRoomsList = await Task.Run(() => foob.GetChatRooms(username));

                if (chatRoomsList != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ChatRoom selectedChatRoom = chatRoomsList.FirstOrDefault(room => room.RoomName == selectedRoom);
                        if (selectedChatRoom != null)
                        {
                            currentRoomParticipants.ItemsSource = null;
                            currentRoomParticipants.ItemsSource = selectedChatRoom.Participants;
                        }
                    });
                }
            }
        }

       private async Task StartUpdatingChatRoomMessages()
            {
            while (true)
            {
                await UpdateChatRoomMessagesAsync();
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
        private async Task UpdateChatRoomMessagesAsync()
        {
            string username = null;
            string selectedRoom = null;

            Dispatcher.Invoke(() =>
            {
                username = NBox.Text;
                selectedRoom = chatroom_name_Block.Text;
            });

            if (!string.IsNullOrEmpty(username))
            {
                List<ChatRoom> chatRoomsList = await Task.Run(() => foob.GetChatRooms(username));

                if (chatRoomsList != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ChatRoom selectedChatRoom = chatRoomsList.FirstOrDefault(room => room.RoomName == selectedRoom);
                        if (selectedChatRoom != null)
                        {
                            chatBox.Text = string.Join(Environment.NewLine, selectedChatRoom.Messages.Select(msg => $"{msg.Sender}: {msg.Content}"));
                        }
                    });
                }
            }
        }

    }
}