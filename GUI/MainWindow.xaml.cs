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

        public MainWindow()
        {
            InitializeComponent();

            ChannelFactory<DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            //Set the URL and create the connection!
            string URL = "net.tcp://localhost:8100/ChatServer";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = NBox.Text;

            if (foob.CheckAccount(username) == false)
            {
                MessageBox.Show("Login successful!");
            }

            else if (foob.CheckAccount(username) == true)
            {
                MessageBox.Show("Username already exists.");
            }
        }
    }
}
