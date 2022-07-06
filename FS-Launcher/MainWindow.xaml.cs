using System;
using System.Diagnostics;
using System.IO;
using System.Media;
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

        private string fsLauncher;

        private string iniPath;
        private string iniStore;

        private string gamePath;
        private string gameStoreID;

        bool updateAvailable;

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

            fsLauncher = Path.Combine(rootPath, "FS-Launcher.exe");

            iniPath = Path.Combine(cfgPath, "Path.ini");
            iniStore = Path.Combine(cfgPath, "Store.ini");

            VersionText.Text = $"v{launcherVersion}";

            FirstRun();
            CheckShiftKey();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CheckForUpdates();
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void VersionText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Coming soon.", "Check for updates");
        }

        private void CloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
