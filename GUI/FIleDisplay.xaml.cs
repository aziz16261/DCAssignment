using Assignment;
using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

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

            TextFileDisplay.Text = "";
            ImageFileDisplay.Source = null;

            if (Path.GetExtension(selectedFile) == ".txt")
            {
                TextFileDisplay.Text = File.ReadAllText("..\\..\\..\\ConsoleApp1\\bin\\Debug\\" + selectedFile);
            } else
            {
                string filePath = "..\\..\\..\\ConsoleApp1\\bin\\Debug\\" + selectedFile;
                Bitmap image = new Bitmap(filePath);
                var imageHandle = image.GetHbitmap();
                ImageFileDisplay.Source = Imaging.CreateBitmapSourceFromHBitmap(imageHandle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
        }
    }
}
