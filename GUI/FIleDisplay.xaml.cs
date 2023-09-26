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
            currentChatRoom = userRoom;
            string listonames = "";
            foreach (FileStore file in currentChatRoom.Files)
            {
                fileNamesList.Add(file.fileName);
            }

            foreach (string name in fileNamesList)
            {
                listonames += name + "\n";
            }

            TextFileDisplay.Text = listonames;
            FileSelectionBox.ItemsSource = null;
            FileSelectionBox.ItemsSource = fileNamesList;
        }

    }
}
