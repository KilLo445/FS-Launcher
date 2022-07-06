using System;
using System.IO;
using System.Media;
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

        private void OtherButton_Click(object sender, RoutedEventArgs e)
        {
            gameStoreID = "0";
            File.WriteAllText(iniStore, gameStoreID);
            Close();
        }

        private void SteamButton_Click(object sender, RoutedEventArgs e)
        {
            gameStoreID = "1";
            File.WriteAllText(iniStore, gameStoreID);
            Close();
        }

        private void UbisoftButton_Click(object sender, RoutedEventArgs e)
        {
            gameStoreID = "2";
            File.WriteAllText(iniStore, gameStoreID);
            Close();
        }

        private void EpicGamesButton_Click(object sender, RoutedEventArgs e)
        {
            SystemSounds.Exclamation.Play();
            MessageBox.Show("Own the game on Epic Games? Please help me out by giving me the App ID! To do so, please join my Discord Server and DM me! For now, you can click other.\n\nDecentLoser#7263 (Please do not add me, I will not accept)\nhttps://discord.gg/hDBHjQr", "Epic Games App ID Unknown");
        }
    }
}
