using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace FS_Launcher
{
    public partial class Extras : Window
    {
        string rootPath;
        string tempPath;
        string fsTemp;

        string fsLauncher;

        string firewallBatCreate;
        string firewallBatDelete;

        public Extras()
        {
            InitializeComponent();

            rootPath = Directory.GetCurrentDirectory();
            tempPath = Path.GetTempPath();
            fsTemp = Path.Combine(tempPath, "FS Launcher");

            fsLauncher = Path.Combine(rootPath, "FS-Launcher.exe");

            firewallBatCreate = Path.Combine(rootPath, "files", "firewall", "windows", "create_firewall_rule_windows.bat");
            firewallBatDelete = Path.Combine(rootPath, "files", "firewall", "windows", "delete_firewall_rule_windows.bat");
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

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey keyFSL = Registry.CurrentUser.OpenSubKey(@"Software\FS Launcher", true);
            Object obFirstRun = keyFSL.GetValue("FirstRun");
            if (obFirstRun != null)
            {
                string strFirstRun = (obFirstRun as String);
                if (strFirstRun == "1")
                {
                    MessageBoxResult resetSoftware = MessageBox.Show("Are you sure you want to reset FS Launcher?", "Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (resetSoftware == MessageBoxResult.Yes)
                    {
                        try
                        {
                            ResetFSL();
                        }
                        catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                    }
                }
            }
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

        private void RestartFSL()
        {
            Process.Start(fsLauncher);
            Application.Current.Shutdown();
        }

        private void ResetFSL()
        {
            try
            {
                RegistryKey keyFSL = Registry.CurrentUser.OpenSubKey(@"Software\FS Launcher", true);
                Object obFirstRun = keyFSL.GetValue("FirstRun");
                Object obFirstPlay = keyFSL.GetValue("FirstPlay");
                Object obFirewall = keyFSL.GetValue("Firewall");
                if (obFirstRun != null)
                {
                    string strFirstRun = (obFirstRun as String);
                    if (strFirstRun == "1")
                    {
                        keyFSL.SetValue("FirstRun", "0");
                    }
                }
                if (obFirstPlay != null)
                {
                    string strFirstPlay = (obFirstPlay as String);
                    if (strFirstPlay == "1")
                    {
                        keyFSL.SetValue("FirstPlay", "0");
                    }
                }
                if (obFirewall != null)
                {
                    string strFirewall = (obFirewall as String);
                    if (strFirewall == "1")
                    {
                        bool resetFirewall = false;
                        while (resetFirewall == false)
                        {
                            try
                            {
                                Process proc = new Process();
                                proc.StartInfo.FileName = firewallBatDelete;
                                proc.StartInfo.UseShellExecute = true;
                                proc.StartInfo.Verb = "runas";
                                proc.Start();
                                resetFirewall = true;
                            }
                            catch
                            {
                                resetFirewall = false;
                                MessageBox.Show("Please accept the admin prompt.");
                            }
                        }

                        keyFSL.SetValue("Firewall", "0");
                        keyFSL.DeleteValue("GRFSPath");
                        keyFSL.Close();
                    }
                }

                RestartFSL();
            }
            catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}
