
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataServerInterface foob;

        public bool IsLoggedIn = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;


            ChannelFactory<DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            tcp.OpenTimeout = new TimeSpan(0, 20, 0);
            tcp.CloseTimeout = new TimeSpan(0, 20, 0);
            tcp.SendTimeout = new TimeSpan(0, 20, 0);
            tcp.ReceiveTimeout = new TimeSpan(0, 20, 0);
            //Set the URL and create the connection!

            string URL = "net.tcp://localhost:8100/ChatServer";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();

            AvailableRooms.ItemsSource = foob.CreateInitialChatRooms(new List<ChatRoom>()).Select(room => room.RoomName);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = NBox.Text;

            if (foob.CheckAccount(username) == true)
            {
                NBox.Text = ("Username already exists.");
            }
            else
            {
                Username.Text = ("Currently logged in as: ") + username;
                foob.CreateAccount(username);
                IsLoggedIn = true;
            }
        }
        private void LogoutButton_Click_1(object sender, RoutedEventArgs e)
        {
            string username = NBox.Text;

            foob.RemoveAccount(username);
            Username.Text = ("you have been logged out: ") + username;
            IsLoggedIn = false;
            NBox.Text = ("Enter a unique username to Log in again.");
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string roomName = roomName_txt.Text;

            if (IsLoggedIn)
            {
                List<ChatRoom> chatRoomsList = foob.GetChatRooms();

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
                        chatRoomsList = foob.GetChatRooms();

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
            if (AvailableRooms.SelectedItem != null)
            {
                string selectedRoom = AvailableRooms.SelectedItem.ToString();
                string username = NBox.Text;

                List<ChatRoom> chatRoomsList = foob.GetChatRooms(); 

                if (chatRoomsList == null)
                {
                    MessageBox.Show("Failed to retrieve chat rooms", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    ChatRoom userCurrentRoom = chatRoomsList.FirstOrDefault(room => room.Participants.Contains(username));

                    if (userCurrentRoom != null)
                    {
                        MessageBox.Show("You are already a participant in a different room. Please leave the current room if you want to join this one.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        chatRoomsList = foob.JoinChatRoom(selectedRoom, username, chatRoomsList); 

                        if (IsLoggedIn == true)
                        {
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

                        AvailableRooms.SelectedItem = null;
                    }
                }
            }
        }
        private void SendBox_Click(object sender, RoutedEventArgs e)
        {
            string roomName = chatroom_name_Block.Text;
            string message = MessageTextBox.Text;

            List<ChatRoom> chatRoomsList = foob.GetChatRooms(); 

            if (IsLoggedIn == true)
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
            if (IsLoggedIn)
            {
                string roomName = chatroom_name_Block.Text;
                string username = NBox.Text;

                List<ChatRoom> chatRoomsList = foob.GetChatRooms(); // Retrieve the chat list

                chatRoomsList = foob.LeaveChatRoom(roomName, username, chatRoomsList);

                if (chatRoomsList != null)
                {
                    chatroom_name_Block.Text = string.Empty;
                    currentRoomParticipants.ItemsSource = null;

                    AvailableRooms.ItemsSource = null;
                    AvailableRooms.ItemsSource = chatRoomsList.Select(room => room.RoomName);
                    chatBox.Clear();

                    // Do not update any global ChatRoomsList; only update the local variable
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
        /// This code below is for handling the file sharing, it doesn't work just yet, probably needs the buttons to be on seperate threads
        /// </summary>

        private void FileTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn) {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "txt files |*.txt|Image files|*.jpg;*.jpeg;*.png;*.bmp";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Title = "Select file to upload";
                dialog.ShowDialog();

                string uploadedFile = dialog.FileName;
                string username = NBox.Text;
                ChatRoom userCurrentRoom = foob.GetChatRooms().FirstOrDefault(room => room.Participants.Contains(username));

                MessageTextBox.Text = foob.UploadFile(uploadedFile, userCurrentRoom);
                SendBox_Click(sender, e); 
            }
        }

        private void FileTransferSendButton_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}