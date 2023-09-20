
using Assignment;
using Console1;
using DatabaseLib;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataServerInterface foob;

        public List<ChatRoom> ChatRoomsList { get; set; } = new List<ChatRoom>();

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

            ChatRoomsList = foob.CreateInitialChatRooms(ChatRoomsList);
            AvailableRooms.ItemsSource = ChatRoomsList.Select(room => room.RoomName);
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

            ChatRoomsList = foob.CreateChatRoom(roomName, ChatRoomsList);

            if (IsLoggedIn)
            {
                AvailableRooms.ItemsSource = null;

                AvailableRooms.ItemsSource = ChatRoomsList.Select(room => room.RoomName);
            }
            else if (!IsLoggedIn)
            {
                MessageBox.Show("Failed, user is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (ChatRoomsList.Any(room => room.RoomName == roomName))
            {
                MessageBox.Show("Failed, chat room with that name already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Failed to create chat room", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (AvailableRooms.SelectedItem != null)
            {
                string selectedRoom = AvailableRooms.SelectedItem.ToString();
                string username = NBox.Text;

                ChatRoomsList = foob.JoinChatRoom(selectedRoom, username, ChatRoomsList);

                if ( IsLoggedIn == true)
                {
                    ChatRoom selectedChatRoom = null;

                    foreach (ChatRoom room in ChatRoomsList)
                    {
                        if (room.RoomName == selectedRoom)
                        {
                            selectedChatRoom = room;
                            break;
                        }
                    }

                    if (selectedChatRoom != null)
                    {
                        chatroom_name_Block.Text = selectedChatRoom.RoomName;

                        Console.WriteLine("Participants: " + selectedChatRoom.GetParticipantsAsString());
                        Console.WriteLine("Participants count: " + selectedChatRoom.Participants.Count);

                        currentRoomParticipants.ItemsSource = selectedChatRoom.Participants;
                    }
                    else
                    {
                        Console.WriteLine("Selected chat room not found in ChatRoomsList");
                    }
                }

                AvailableRooms.SelectedItem = null;
            }
        }

        private void SendBox_Click(object sender, RoutedEventArgs e)
        {
            string roomName = chatroom_name_Block.Text;
            string message = MessageTextBox.Text;
            List<ChatRoom> updatedChatRoomsList = foob.SendMessage(NBox.Text, roomName, message, ChatRoomsList);

            if (IsLoggedIn == true)
            {
                ChatRoom userChatRoom = ChatRoomsList.FirstOrDefault(room => room.Participants.Contains(NBox.Text));

                if (userChatRoom != null)
                {

                    if (updatedChatRoomsList != null)
                    {
                        ChatRoomsList = updatedChatRoomsList;

                        ChatRoom updatedChatRoom = ChatRoomsList.FirstOrDefault(room => room.RoomName == roomName);

                        if (updatedChatRoom != null)
                        {
                            chatBox.Text = string.Join(Environment.NewLine, updatedChatRoom.Messages.Select(msg => $"{msg.Sender}: {msg.Content}"));
                            MessageTextBox.Clear();
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
    }
}