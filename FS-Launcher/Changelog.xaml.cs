using System;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace FS_Launcher
{
    public partial class Changelog : Window
    {
        string changelogLink = "https://pastebin.com/raw/znP4q1p2";
        string changelogContent;

        int fontSize = 14;

        public Changelog()
        {
            InitializeComponent();

            this.Topmost = true;

            WebClient webClient = new WebClient();
            changelogContent = webClient.DownloadString(changelogLink);

            ChangelogText.Text = changelogContent;
            ChangelogText.FontSize = fontSize;
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
