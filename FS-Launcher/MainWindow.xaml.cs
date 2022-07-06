using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net;
using System.Windows;
using System.Windows.Input;

using Ookii.Dialogs.Wpf;

namespace FS_Launcher
{
    public partial class MainWindow : Window
    {
        string launcherVersion = "0.0.1";

        private string rootPath;
        private string tempPath;
        private string fsTemp;
        private string cfgPath;
        private string cfgReset;
        private string versionFile;
        private string firstRunFile;
        private string updater;
        private string grfsExe;
        private string firewallBat;

        private string fsLauncher;

        private string gamePath;
        private string storeID;

        private string iniPath;
        private string iniStore;

        bool updateAvailable;
        private object webClient;

        public MainWindow()
        {
            InitializeComponent();

            rootPath = Directory.GetCurrentDirectory();
            tempPath = Path.GetTempPath();
            fsTemp = Path.Combine(tempPath, "FS Launcher");
            cfgPath = Path.Combine(rootPath, "cfg");
            cfgReset = Path.Combine(cfgPath, "! Reset.bat");
            versionFile = Path.Combine(rootPath, "version.txt");
            firstRunFile = Path.Combine(cfgPath, "FirstRun.ini");
            updater = Path.Combine(rootPath, "updater.exe");
            firewallBat = Path.Combine(fsTemp, "firewall.bat");

            fsLauncher = Path.Combine(rootPath, "FS-Launcher.exe");

            iniPath = Path.Combine(cfgPath, "Path.ini");
            iniStore = Path.Combine(cfgPath, "Store.ini");

            VersionText.Text = $"v{launcherVersion}";

            FirstRun();
            CheckShiftKey();
            GetCFG();

            grfsExe = Path.Combine(gamePath, "Future Soldier.exe");
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CheckForUpdates();

            MessageBox.Show("Please Note:\n\nThis program is still in development, and I am not amazing at coding, so updates and new features may be slow, but they will be released.", "Future Soldier Launcher", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FirstRun()
        {
            if (!File.Exists(firstRunFile))
            {
                var firstRunTxt = File.Create(firstRunFile);
                firstRunTxt.Close();
                File.WriteAllText(firstRunFile, "FirstRunComplete=True");

                var gamePathDialog = new VistaFolderBrowserDialog();
                gamePathDialog.Description = "Please select your Ghost Recon: Future Soldier install folder.";
                gamePathDialog.UseDescriptionForTitle = true;
                gamePathDialog.Multiselect = false;
                if (gamePathDialog.ShowDialog(this).GetValueOrDefault())
                {
                    gamePath = Path.Combine(gamePathDialog.SelectedPath);
                    grfsExe = Path.Combine(gamePath, "Future Soldier.exe");

                    if (!File.Exists(grfsExe))
                    {
                        SystemSounds.Exclamation.Play();
                        MessageBox.Show("Please select the location with Future Soldier.exe");
                        File.Delete(firstRunFile);
                        Application.Current.Shutdown();
                    }

                    File.WriteAllText(iniPath, gamePath);

                    GameStore storeWindow = new GameStore();
                    storeWindow.Show();
                }
                else
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("Please select your Ghost Recon: Future Soldier install folder.");
                    File.Delete(firstRunFile);
                    Application.Current.Shutdown();
                }
            }
        }

        private void GetCFG()
        {
            if (File.Exists(iniPath))
            {
                gamePath = File.ReadAllText(iniPath);
            }
            if (File.Exists(iniStore))
            {
                storeID = File.ReadAllText(iniStore);
            }
        }

        private void DumpVersion()
        {
            if (File.Exists(versionFile))
            {
                File.WriteAllText(versionFile, launcherVersion);
            }
            else
            {
                File.Create(versionFile);
                File.WriteAllText(versionFile, launcherVersion);
            }
        }

        private void CheckForUpdates()
        {
            DumpVersion();

            if (File.Exists(versionFile))
            {
                Version localVersion = new Version(File.ReadAllText(versionFile));

                try
                {
                    WebClient webClient = new WebClient();
                    Version onlineVersion = new Version(webClient.DownloadString("https://raw.githubusercontent.com/KilLo445/FS-Launcher/master/A_Files/version.txt"));

                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        updateAvailable = true;
                        InstallUpdate(true, onlineVersion);
                    }
                    else
                    {
                        updateAvailable = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error checking for updates:\n{ex}", "Error");
                }
            }
            else
            {
                updateAvailable = true;
                InstallUpdate(false, Version.zero);
            }
        }
        private void InstallUpdate(bool isUpdate, Version _onlineVersion)
        {
            try
            {
                Process.Start(updater);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show($"Error:\n{ex}");
            }
        }

        public void CheckShiftKey()
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) && File.Exists(firstRunFile))
            {
                MessageBoxResult resetSoftware = MessageBox.Show("Are you sure you want to reset FS Launcher?", "Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (resetSoftware == MessageBoxResult.Yes)
                {
                    try
                    {
                        File.Delete(firstRunFile);

                        Process.Start(fsLauncher);
                        Application.Current.Shutdown();
                    }
                    catch
                    {
                        SystemSounds.Exclamation.Play();
                        MessageBox.Show("Error resetting FS Launcher, please run reset.bat in the cfg folder.");
                        Application.Current.Shutdown();
                    }
                }
            }
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            if (storeID == "0")
            {
                Process.Start(grfsExe);
            }
            if (storeID == "1")
            {
                Process.Start("steam://run/212630");
            }
            if (storeID == "2")
            {
                Process.Start("uplay://launch/53/0");
            }
        }

        private void FirewallButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult firewallMessageBox = System.Windows.MessageBox.Show("Do you want to setup the Firewall Rules in Windows?", "Firewall", System.Windows.MessageBoxButton.YesNo);
            if (firewallMessageBox == MessageBoxResult.Yes)
            {
                ProgressBar1.Visibility = Visibility.Visible;
                
                CreateTemp();

                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFirewallBatCompletedCallback);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                webClient.DownloadFileAsync(new Uri("https://raw.githubusercontent.com/KilLo445/FS-Launcher/master/A_Files/firewall/windows.bat"), firewallBat);
            }
        }

        private void UnlockDLCButton_Click(object sender, RoutedEventArgs e)
        {
            SystemSounds.Exclamation.Play();
            MessageBox.Show("Coming soon", "Unlock DLC");
        }

        private void FirewallButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult firewallMessageBox2 = System.Windows.MessageBox.Show("Do you want to delete the Firewall Rules in Windows?", "Firewall", System.Windows.MessageBoxButton.YesNo);
            if (firewallMessageBox2 == MessageBoxResult.Yes)
            {
                CreateTemp();

                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFirewallBatCompletedCallback2);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                webClient.DownloadFileAsync(new Uri("https://raw.githubusercontent.com/KilLo445/FS-Launcher/master/A_Files/firewall/delete_windows.bat"), firewallBat);
            }
        }

        private void DownloadFirewallBatCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            ProgressBar1.Visibility = Visibility.Hidden;
            ExecuteAsAdmin(firewallBat);
        }
        private void DownloadFirewallBatCompletedCallback2(object sender, AsyncCompletedEventArgs e)
        {
            ExecuteAsAdmin(firewallBat);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CreateTemp()
        {
            Directory.CreateDirectory(fsTemp);
        }

        private void DelTemp()
        {
            if (Directory.Exists(fsTemp))
            {
                Directory.Delete(fsTemp, true);
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBar1.Value = e.ProgressPercentage;
        }

        private void VersionText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult checkUpdateLMB = MessageBox.Show("Do you want to check for updates?", "Check for updates", MessageBoxButton.YesNo);
            if (checkUpdateLMB == MessageBoxResult.Yes)
            {
                CheckForUpdates();

                if (updateAvailable == false)
                {
                    MessageBox.Show("No update is available.", "Check for updates");
                }
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

        protected override void OnClosing(CancelEventArgs e)
        {
            DelTemp();
        }

        public void ExecuteAsAdmin(string fileName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = fileName;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            proc.Start();
        }

        struct Version
        {
            internal static Version zero = new Version(0, 0, 0);

            private short major;
            private short minor;
            private short subMinor;

            internal Version(short _major, short _minor, short _subMinor)
            {
                major = _major;
                minor = _minor;
                subMinor = _subMinor;
            }
            internal Version(string _version)
            {
                string[] versionStrings = _version.Split('.');
                if (versionStrings.Length != 3)
                {
                    major = 0;
                    minor = 0;
                    subMinor = 0;
                    return;
                }

                major = short.Parse(versionStrings[0]);
                minor = short.Parse(versionStrings[1]);
                subMinor = short.Parse(versionStrings[2]);
            }

            internal bool IsDifferentThan(Version _otherVersion)
            {
                if (major != _otherVersion.major)
                {
                    return true;
                }
                else
                {
                    if (minor != _otherVersion.minor)
                    {
                        return true;
                    }
                    else
                    {
                        if (subMinor != _otherVersion.subMinor)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public override string ToString()
            {
                return $"{major}.{minor}.{subMinor}";
            }
        }
    }
}
