
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

        public List<string> ChatRoomsList { get; } = new List<string>();

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

            ChatRoomsList = foob.CreateInitialChatRooms();
            AvailableRooms.ItemsSource = ChatRoomsList;
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

            Boolean result = foob.CreateChatRoom(roomName);

            if (result == true && IsLoggedIn == true)
            {
                ChatRoomsList.Add(roomName);

                AvailableRooms.ItemsSource = null;
                AvailableRooms.ItemsSource = ChatRoomsList;
            }
            else if (IsLoggedIn == false)
            {
                MessageBox.Show("Failed, user is not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (ChatRoomsList.Contains(roomName))
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
            // Allows you to switch chat rooms, but doesn't let you go back to a previous selection

            if (AvailableRooms.SelectedItem != null)




        private void AvailableRooms_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AvailableRooms.SelectedItem != null)
                {
                    chatroom_name_Block.Text = selectedRoom;

                    // Get the selected chat room from the ChatRoomsList property
                    ChatRoom selectedChatRoom = foob.GetChatRoom(selectedRoom);
                    string selectedRoomName = AvailableRooms.SelectedItem.ToString();
                    string username = Username.Text;

                    bool result = foob.JoinChatRoom(selectedRoomName, username);

                    if (result && IsLoggedIn)
                    {
                        //string roomMessages = "";

                        currentRoomParticipants.ItemsSource = selectedChatRoom.Participants;

                        /* 
                         * For some reason the messages are null for the initial chat rooms
                        foreach (Message message in selectedChatRoom.Messages)
                        {
                            roomMessages += message.Sender + ": " + message.Content + "\n";
                        }

                        chatBox.Text = roomMessages;

                        */

                        ChatRoomName.Text = selectedRoomName;

                        List<ChatRoom> rooms = foob.ConvertChatRooms(ChatRoomsList);

                        ChatRoom selectedChatRoom = rooms.FirstOrDefault(room => room.RoomName == selectedRoomName);

                        if (selectedChatRoom != null)
                        {
                            List<string> participants = selectedChatRoom.GetParticipants();
                            UsersInChatRoom.ItemsSource = participants;
                        }

                        AvailableRooms.SelectedItem = null;
                    }
                    else
                    {
                        MessageBox.Show("Failed to join the chat room.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // Clear the selection after joining the chat room
                AvailableRooms.SelectedItem = -1;
            }
        }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
