using System;
using System.IO;
using System.Windows.Forms;

namespace VirtualCamera
{
    public partial class ObjSelector : Form
    {
        private MainWindow mainWindow;

        public ObjSelector()
        {
            InitializeComponent();
            filePathsBox.Items.AddRange(Directory.GetFiles(@"3dModels"));
        }

        private void FilePathsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainWindow != null) mainWindow.Close();
            mainWindow = new MainWindow(filePathsBox.SelectedItem.ToString());
            mainWindow.Show();
        }
    }
}
