using Assignment;
using DatabaseLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for FileDisplay.xaml
    /// </summary>
    public partial class FileDisplay : Window
    {
        public ChatRoom currentChatRoom = new ChatRoom("Test");
        private List<string> fileNamesList = new List<string>();
        public FileDisplay()
        {
            InitializeComponent();
        }

        public void setChatRoom(ChatRoom userRoom)
        {
            FileSelectionBox.ItemsSource = null;
            FileSelectionBox.ItemsSource = userRoom.Files;
        }

        private void FileSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedFile = FileSelectionBox.SelectedItem.ToString();

            byte[] fileData = File.ReadAllBytes(selectedFile);
        }
    }
}
