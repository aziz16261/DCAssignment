
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
        //  private DispatcherTimer refreshTimer;

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

            string URL = "net.tcp://localhost:8100/ChatServer";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();

            AvailableRooms.ItemsSource = foob.CreateInitialChatRooms(new List<ChatRoom>()).Select(room => room.RoomName);

            //  refreshTimer = new DispatcherTimer();
            //  refreshTimer.Interval = TimeSpan.FromSeconds(4); 
            //  refreshTimer.Tick += RefreshTimer_Tick;
            //   refreshTimer.Start();

            Task.Run(() => StartUpdatingChatRooms());

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
                foob.RemoveAccount(username);

                Username.Text = ("You have been logged out: " + username);
                NBox.Text = ("Enter a unique username to log in again.");
                LoginButton.IsEnabled = true;
                NBox.IsEnabled = true;

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
            Dispatcher.Invoke(() =>
            {
                username = NBox.Text;
            });

            if (!string.IsNullOrEmpty(username))
            {
                if (AvailableRooms.SelectedItem != null)
                {
                    string selectedRoom = AvailableRooms.SelectedItem.ToString();

                    List<ChatRoom> chatRoomsList = DataServer.ChatRoomsList;

                    if (chatRoomsList != null)
                    {
                        ChatRoom userCurrentRoom = chatRoomsList.FirstOrDefault(room => room.Participants.Contains(username));

                        if (userCurrentRoom != null)
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
                Dispatcher.Invoke(() =>
                {
                    AvailableRooms.SelectedIndex = -1;
                    MessageBox.Show("Failed, user is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
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
        /// This code below is for handling the file sharing, it doesn't work just yet, probably needs the buttons to be on seperate threads
        /// </summary>

        private void FileTransfer_Click(object sender, RoutedEventArgs e)
        {
            bool usernameExists = foob.CheckAccount(NBox.Text);

            if (usernameExists)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "txt files |*.txt|Image files|*.jpg;*.jpeg;*.png;*.bmp";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Title = "Select file to upload";
                dialog.ShowDialog();

                string uploadedFile = dialog.FileName;
                string username = NBox.Text;
                ChatRoom userCurrentRoom = foob.GetChatRooms(NBox.Text).FirstOrDefault(room => room.Participants.Contains(username));

                MessageTextBox.Text = foob.UploadFile(uploadedFile, userCurrentRoom);
                SendBox_Click(sender, e);
            }
        }

        private void FileTransferSendButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /*    private void RefreshChatRoomsAndMessages()
            {
                List<ChatRoom> chatRoomsList = foob.GetChatRooms(NBox.Text);

                if (chatRoomsList != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        AvailableRooms.ItemsSource = chatRoomsList.Select(room => room.RoomName);

                       string chatRoomName = chatroom_name_Block.Text;
                       ChatRoom selectedChatRoom = chatRoomsList.FirstOrDefault(room => room.RoomName == chatRoomName);

                        if (selectedChatRoom != null)
                        {
                            chatBox.Text = string.Join(Environment.NewLine, selectedChatRoom.Messages.Select(msg => $"{msg.Sender}: {msg.Content}"));

                            currentRoomParticipants.ItemsSource = selectedChatRoom.Participants;

                            AvailableRooms.ItemsSource = chatRoomsList.Select(room => room.RoomName);
                        }
                    });
                }
            }

            private void RefreshTimer_Tick(object sender, EventArgs e)
            {
                RefreshChatRoomsAndMessages();
            } */

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
                    MessageBox.Show("Please enter a recipient's username.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Recipient's username does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            Dispatcher.Invoke(() =>
            {
                senderUsername = NBox.Text;
            });

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



        private async void StartUpdatingChatRooms()
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
    }
}