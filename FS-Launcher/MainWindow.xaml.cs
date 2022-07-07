using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Media;
using System.Net;
using System.Windows;
using System.Windows.Input;

using Ookii.Dialogs.Wpf;

namespace FS_Launcher
{
    public partial class MainWindow : Window
    {
        string launcherVersion = "0.0.3";

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
        private string firewallSetup;

        private string iniPath;
        private string iniStore;
        private string iniFirewall;
        private string iniDLC;
        private string iniDLCPath;

        private string dlcInstalled;
        private string dlcBin;
        private string dlcPath;
        private string dlcTemp;
        private string dlcSave;
        private string dlcSaveBak;
        private string dlcSaveTemp;
        private string dlcSavePath;

        bool updateAvailable;

        public MainWindow()
        {
            InitializeComponent();

            DelTemp();

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
            iniFirewall = Path.Combine(cfgPath, "Firewall.ini");
            iniDLC = Path.Combine(cfgPath, "DLC.ini");
            iniDLCPath = Path.Combine(cfgPath, "DLCPath.ini");

            dlcBin = Path.Combine(rootPath, "dlc.bin");
            dlcTemp = Path.Combine(fsTemp, "DLC");
            dlcSaveTemp = Path.Combine(fsTemp, "1.save");

            VersionText.Text = $"v{launcherVersion}";

            FirstRun();
            CheckShiftKey();
            GetCFG();

            grfsExe = Path.Combine(gamePath, "Future Soldier.exe");
            dlcPath = Path.Combine(gamePath, "DLC");
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void FirstRun()
        {
            if (!File.Exists(firstRunFile))
            {
                MessageBox.Show("Please Note:\n\nThis program is still in development, and I am not amazing at coding, so updates and new features may be slow, but they will be released.", "Future Soldier Launcher", MessageBoxButton.OK, MessageBoxImage.Information);

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
            if (File.Exists(iniFirewall))
            {
                firewallSetup = File.ReadAllText(iniFirewall);
            }
            if (File.Exists(iniDLCPath))
            {
                dlcSavePath = File.ReadAllText(iniDLCPath);
                dlcSave = Path.Combine(dlcSavePath, "1.save");
                dlcSaveBak = Path.Combine(dlcSavePath, "1.save.bak");
            }
            if (File.Exists(iniDLC))
            {
                dlcInstalled = File.ReadAllText(iniDLC);
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
                LauncherUpdate updateWindow = new LauncherUpdate();
                updateWindow.Show();
            }
            catch (Exception ex)
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show($"Error: {ex}");
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
                if (firewallSetup != "1")
                {
                    File.WriteAllText(iniFirewall, "1");

                    ProgressBar1.Visibility = Visibility.Visible;

                    CreateTemp();

                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFirewallBatCompletedCallback);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileAsync(new Uri("https://raw.githubusercontent.com/KilLo445/FS-Launcher/master/A_Files/firewall/windows.bat"), firewallBat);
                }
                else
                {
                    MessageBox.Show("The firewall seems to already be setup.");
                }
            }
        }

        private void FirewallButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult firewallMessageBox2 = System.Windows.MessageBox.Show("Do you want to delete the Firewall Rules in Windows?", "Firewall", System.Windows.MessageBoxButton.YesNo);
            if (firewallMessageBox2 == MessageBoxResult.Yes)
            {
                if (firewallSetup != "0")
                {
                    File.WriteAllText(iniFirewall, "0");

                    CreateTemp();

                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFirewallBatCompletedCallback2);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileAsync(new Uri("https://raw.githubusercontent.com/KilLo445/FS-Launcher/master/A_Files/firewall/delete_windows.bat"), firewallBat);
                }
                else
                {
                    MessageBox.Show("The firewall rule does not seem to exist.");
                }
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

        private void UnlockDLCButton_Click(object sender, RoutedEventArgs e)
        {
            if (dlcInstalled != "1")
            {
                if (File.Exists(dlcBin))
                {
                    MessageBoxResult unlockDLC = MessageBox.Show("Are you sure you want to unlock the DLC?", "Unlock DLC", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (unlockDLC == MessageBoxResult.Yes)
                    {
                        MessageBox.Show("Please browse to the following location:\n\nC:/Program Files(x86)/Ubisoft/Ubisoft Game Launcher/savegames/USER-ID\n\nThere should be a few folders with random numbers.", "Unlock DLC", MessageBoxButton.OK);

                        var saveFileDialog = new VistaFolderBrowserDialog();
                        saveFileDialog.Description = "Please select your Ubisoft ID folder at C:/Program Files(x86)/Ubisoft/Ubisoft Game Launcher/savegames/USER-ID";
                        saveFileDialog.UseDescriptionForTitle = true;
                        saveFileDialog.Multiselect = false;
                        if (saveFileDialog.ShowDialog(this).GetValueOrDefault())
                        {
                            dlcSavePath = Path.Combine(saveFileDialog.SelectedPath, "53");
                            dlcSave = Path.Combine(dlcSavePath, "1.save");
                            dlcSaveBak = Path.Combine(dlcSavePath, "1.save.bak");
                            File.WriteAllText(iniDLCPath, dlcSavePath);

                            MessageBox.Show("The launcher may freeze while it unlocks the dlc, do not close it!", "Unlock DLC", MessageBoxButton.OK, MessageBoxImage.Information);

                            try
                            {
                                ZipFile.ExtractToDirectory(dlcBin, fsTemp);

                                foreach (string dirPath in Directory.GetDirectories(dlcTemp, "*", SearchOption.AllDirectories))
                                {
                                    Directory.CreateDirectory(dirPath.Replace(dlcTemp, dlcPath));
                                }

                                foreach (string newPath in Directory.GetFiles(dlcTemp, "*.*", SearchOption.AllDirectories))
                                {
                                    File.Copy(newPath, newPath.Replace(dlcTemp, dlcPath), true);
                                }

                                File.Copy(dlcSave, dlcSaveBak);
                                File.Delete(dlcSave);
                                File.Copy(dlcSaveTemp, dlcSave);

                                File.WriteAllText(iniDLC, "1");

                                SystemSounds.Exclamation.Play();
                                MessageBox.Show("DLC Unlocked!");

                                SystemSounds.Exclamation.Play();
                                MessageBoxResult deleteDLCbin = MessageBox.Show("Do you want to delete dlc.bin?", "Unlock DLC", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (deleteDLCbin == MessageBoxResult.Yes)
                                {
                                    try
                                    {
                                        File.Delete(dlcBin);

                                        MessageBox.Show("dlc.bin deleted!");
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"Error:\n\n{ex}");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error:\n\n{ex}");
                            }
                        }
                    }
                }
                else
                {
                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("dlc.bin not found", "Unlock DLC", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show("DLC already seems to be unlocked.");
            }
        }

        private void UnlockDLCButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (dlcInstalled != "0")
            {
                MessageBoxResult deleteDLC = MessageBox.Show("Are you sure you want to delete the DLC?", "DLC", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (deleteDLC == MessageBoxResult.Yes)
                {
                    File.Delete(dlcSave);
                    File.Copy(dlcSaveBak, dlcSave);
                    File.Delete(dlcSaveBak);

                    string dlc1;
                    string dlc2;
                    string dlc3;

                    dlc1 = Path.Combine(dlcPath, "dlc1");
                    dlc2 = Path.Combine(dlcPath, "dlc2");
                    dlc3 = Path.Combine(dlcPath, "dlc3");

                    Directory.Delete(dlc1, true);
                    Directory.Delete(dlc2, true);
                    Directory.Delete(dlc3, true);

                    File.WriteAllText(iniDLC, "0");

                    SystemSounds.Exclamation.Play();
                    MessageBox.Show("DLC deleted!");
                }
            }
        }

        private void PunkbusterButton_Click(object sender, RoutedEventArgs e)
        {
            Punkbuster pnkbstrWindow = new Punkbuster();
            pnkbstrWindow.Show();
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
