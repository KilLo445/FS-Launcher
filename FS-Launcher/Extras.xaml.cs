using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
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
        string flushdnsBat;

        string hostsFile;
        string hostsBackup;

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public Extras()
        {
            InitializeComponent();

            rootPath = Directory.GetCurrentDirectory();
            tempPath = Path.GetTempPath();
            fsTemp = Path.Combine(tempPath, "FS Launcher");

            fsLauncher = Path.Combine(rootPath, "FS-Launcher.exe");

            firewallBatCreate = Path.Combine(rootPath, "files", "firewall", "windows", "create_firewall_rule_windows.bat");
            firewallBatDelete = Path.Combine(rootPath, "files", "firewall", "windows", "delete_firewall_rule_windows.bat");
            flushdnsBat = Path.Combine(rootPath, "files", "flushdns.bat");

            hostsFile = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts"));
            hostsBackup = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts.bak"));
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

        private void HostsButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdministrator())
            {
                MessageBox.Show("Please note:\n\nThis will create a backup of your hosts file, and if you undo these entries through the launcher, it will revert to your backup and remove any entries created after you created the backup.", "Hosts", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                MessageBoxResult writeHosts = MessageBox.Show("Are you sure you want to write hosts?", "Hosts", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (writeHosts == MessageBoxResult.Yes)
                {
                    try
                    {
                        File.Copy(hostsFile, hostsBackup, true);
                    }
                    catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }

                    try
                    {
                        string entry0 = " ";
                        string entry1 = "185.38.21.82 185.38.21.83";
                        string entry2 = "185.38.21.82 185.38.21.84";
                        string entry3 = "185.38.21.82 185.38.21.85";

                        using (StreamWriter w = File.AppendText(hostsFile))
                        {
                            w.WriteLine(entry0);
                            w.WriteLine(entry1);
                            w.WriteLine(entry2);
                            w.WriteLine(entry3);
                        }

                        MessageBox.Show("Hosts entries added successfully.", "Hosts", MessageBoxButton.OK, MessageBoxImage.Information);
                        MessageBox.Show("If this is not working for you, it is recommended to flush your DNS, you can do so by right clicking the \"hosts\" button.", "Flush DNS", MessageBoxButton.OK, MessageBoxImage.Information);
                        MessageBoxResult restartRequired = MessageBox.Show("A restart is required for these changes to take effect.\n\nWould you like to restart now?", "Restart required", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (restartRequired == MessageBoxResult.Yes)
                        {
                            try
                            {
                                Process.Start("shutdown.exe", "-r -t 0");
                                Application.Current.Shutdown();
                            }
                            catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                        }
                        else { return; }
                    }
                    catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
                else { return; }
            }
            else
            {
                MessageBox.Show("Please run FS Launcher as Admin.", "Hosts", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void DelHosts_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdministrator())
            {
                MessageBoxResult delHosts = MessageBox.Show("Are you sure you want to delete hosts?", "Hosts", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (delHosts == MessageBoxResult.Yes)
                {
                    try
                    {
                        File.Copy(hostsBackup, hostsFile, true);
                        File.Delete(hostsBackup);
                        MessageBox.Show("Backup restored.", "Hosts", MessageBoxButton.OK, MessageBoxImage.Information);

                        MessageBoxResult restartRequired = MessageBox.Show("A restart is required for these changes to take effect.\n\nWould you like to restart now?", "Restart required", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (restartRequired == MessageBoxResult.Yes)
                        {
                            try
                            {
                                Process.Start("shutdown.exe", "-r -t 0");
                                Application.Current.Shutdown();
                            }
                            catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                        }
                        else { return; }
                    }
                    catch (Exception ex) { MessageBox.Show($"{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                }
            }
            else
            {
                MessageBox.Show("Please run FS Launcher as Admin.", "Hosts", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
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

        private void FlushDNS_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult flushDNS = MessageBox.Show("Are you sure you want to flush DNS?", "Flush DNS", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (flushDNS == MessageBoxResult.Yes)
            {
                ExecuteAsAdmin(flushdnsBat);
            } 
        }

        public void ExecuteAsAdmin(string adminFileName)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = adminFileName;
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Verb = "runas";
                proc.Start();
            }
            catch
            {
                MessageBox.Show($"Error launching as admin.\nPlease accept admin prompt.\n\nFile: {adminFileName}", "Error");
                return;
            }
        }
    }
}
