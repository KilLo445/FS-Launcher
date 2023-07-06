using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace FS_Launcher
{
    public partial class Changelog : Window
    {
        private string rootPath;
        private string tempPath;
        private string fsTemp;

        string changelogLink = "https://pastebin.com/raw/znP4q1p2";
        string changelogContent;

        int fontSize = 14;

        public Changelog()
        {
            InitializeComponent();

            ChangelogText.Text = "Downloading changelog...";
            ChangelogText.FontSize = fontSize;

            rootPath = Directory.GetCurrentDirectory();
            tempPath = Path.GetTempPath();
            fsTemp = Path.Combine(tempPath, "FS Launcher");

            this.Topmost = true;

            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
            wc.DownloadStringAsync(new Uri(changelogLink));
        }

        private void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            ChangelogText.Text = e.Result;
        }

        private void FontSizeIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                fontSize = fontSize + 10;
                if (fontSize >= 1)
                {
                    ChangelogText.FontSize = fontSize;
                }
                return;
            }
            else
            {
                fontSize = fontSize + 1;
                if (fontSize >= 1)
                {
                    ChangelogText.FontSize = fontSize;
                }
                return;
            }
        }

        private void FontSizeDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (fontSize == 1)
            {
                return;
            }
            else
            {
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    fontSize = fontSize - 10;
                    if (fontSize >= 1)
                    {
                        ChangelogText.FontSize = fontSize;
                    }
                    else
                    {
                        fontSize = 1;
                        ChangelogText.FontSize = fontSize;
                    }
                    return;
                }
                else
                {
                    fontSize = fontSize - 1;
                    if (fontSize >= 1)
                    {
                        ChangelogText.FontSize = fontSize;
                    }
                    else
                    {
                        fontSize = 1;
                        ChangelogText.FontSize = fontSize;
                    }
                    return;
                }
            }
        }
    }
}
