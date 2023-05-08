using System;
using System.Windows;
using System.Windows.Input;

namespace FS_Launcher
{
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
            try
            {
                Application.Current.Shutdown();
            }
            catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void MinimizeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void RPCS3Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RPCS3 rpcs3Window = new RPCS3();
                this.Close();
                rpcs3Window.Show();
            }
            catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ChangelogButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Changelog changelogWindow = new Changelog();
                changelogWindow.Show();
            }
            catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow mWindow = new MainWindow();
                this.Close();
                mWindow.Show();
            }
            catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}
