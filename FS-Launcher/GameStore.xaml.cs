using System;
using System.IO;
using System.Windows;

namespace FS_Launcher
{
    public partial class GameStore : Window
    {
        private string rootPath;
        private string cfgPath;
        
        private string iniStore;

        private string gameStoreID;

        public GameStore()
        {
            InitializeComponent();

            this.Topmost = true;

            rootPath = Directory.GetCurrentDirectory();
            cfgPath = Path.Combine(rootPath, "cfg");

            iniStore = Path.Combine(cfgPath, "Store.ini");
        }

        private void SteamButton_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(iniStore, "1");
            Close();
        }

        private void UbisoftButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Coming soon.");
        }
    }
}
