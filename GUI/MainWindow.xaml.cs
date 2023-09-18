﻿
using Console1;
using DatabaseLib;
using System;
using System.Collections.Generic;
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
            //Set the URL and create the connection!
            string URL = "net.tcp://localhost:8100/ChatServer";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();

            ChatRoomsList = foob.CreateInitialChatRooms();

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

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            IsLoggedIn = false;
            Username.Text = ("Successfully logged out");
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





        private void AvailableRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Does not work, to be done

           /*  if (AvailableRooms.SelectedItem != null)
            {
                string selectedRoom = AvailableRooms.SelectedItem.ToString();
                string username = Username.Text;

                Boolean result = foob.JoinChatRoom(selectedRoom, username);

                if (result == true && IsLoggedIn == true)
                {
                    CurrentRoom.Text = selectedRoom;

                    // Get the selected chat room from the ChatRoomsList property
                    ChatRoom selectedChatRoom = ChatRoomsList.FirstOrDefault(room => room.RoomName == selectedRoom);

                    if (selectedChatRoom != null)
                    {
                        UsersInChatRoom = selectedChatRoom.Participants;

                        // Now, UsersInChatRoom should be updated with the participants in the selected chat room.
                    }
                }

                // Clear the selection after joining the chat room
                AvailableRooms.SelectedItem = null;
            }*/
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            string username = NBox.Text;

            foob.RemoveAccount(username);
            Username.Text = ("you have been logged out: ") + username;
            NBox.Text = ("Enter a unique username to Log in again.");
        }
    }
}
