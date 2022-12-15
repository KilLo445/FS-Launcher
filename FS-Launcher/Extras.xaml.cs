using System;
using System.Windows;
using System.Windows.Input;

namespace FS_Launcher
{
    /// <summary>
    /// Interaction logic for Extras.xaml
    /// </summary>
    public partial class Extras : Window
    {
        public Extras()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void RPCS3Button_Click(object sender, RoutedEventArgs e)
        {
            RPCS3 rpcs3Window = new RPCS3();
            this.Close();
            rpcs3Window.Show();
        }

        private void ChangelogButton_Click(object sender, RoutedEventArgs e)
        {
            Changelog changelogWindow = new Changelog();
            changelogWindow.Show();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mWindow = new MainWindow();
            this.Close();
            mWindow.Show();
        }
    }
}
