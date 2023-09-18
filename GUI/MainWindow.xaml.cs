using Console1;
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
                NBox.Text=("Username already exists.");
            }

            else
            {
                Username.Text = ("Currently logged in as: ") + username;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string roomName = roomName_txt.Text;

            string result = foob.CreateChatRoom(roomName);

            if (result == "Chat room created successfully")
            {
                ChatRoomsList.Add(roomName);

                AvailableRooms.ItemsSource = null;
                AvailableRooms.ItemsSource = ChatRoomsList;
            }
            else
            {
                roomName_txt.Text = "ERROR";
            }

        }


        private void ChatRoomListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
