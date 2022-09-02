using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;

namespace FS_Launcher
{
    public partial class LauncherUpdate : Window
    {
        private string rootPath;
        private string updater;

        public LauncherUpdate()
        {
            InitializeComponent();

            this.Topmost = true;

            rootPath = Directory.GetCurrentDirectory();
            updater = Path.Combine(rootPath, "updater.exe");

            GetVersion();
        }

        private void GetVersion()
        {
            WebClient webClient = new WebClient();
            string updateVer = webClient.DownloadString("https://pastebin.com/raw/rg2GbmNL");

            UpdateVersion.Text = $"Update: v{updateVer}";
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(updater))
            {
                Process.Start(updater);
                Application.Current.Shutdown();
            }

        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
