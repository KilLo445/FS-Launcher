using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FS_Launcher
{
    public partial class Punkbuster : Window
    {
        private string rootPath;
        private string tempPath;
        private string fsTemp;
        private string iconFile;
        private string sys32;

        private string iniPath;
        private string gamePath;

        private string pnkbstrBin;

        private string pbTempPath;
        private string pbAtmp;
        private string pbBtmp;
        private string pbAsys;
        private string pbBsys;

        private string pbsvc;
        private string pbsvcTmp;

        bool pbAInst;
        bool pbBInst;

        public Punkbuster()
        {
            InitializeComponent();

            rootPath = Directory.GetCurrentDirectory();
            tempPath = Path.GetTempPath();
            fsTemp = Path.Combine(tempPath, "FS Launcher");
            iconFile = Path.Combine(rootPath, "PnkBstr.ico");
            sys32 = Environment.SystemDirectory;

            pnkbstrBin = Path.Combine(fsTemp, "punkbuster.bin");

            pbTempPath = Path.Combine(fsTemp, "Punkbuster");
            pbAtmp = Path.Combine(pbTempPath, "PnkBstrA.exe");
            pbBtmp = Path.Combine(pbTempPath, "PnkBstrB.exe");
            pbAsys = Path.Combine(sys32, "PnkBstrA.exe");
            pbBsys = Path.Combine(sys32, "PnkBstrB.exe");

            iniPath = Path.Combine(rootPath, "cfg", "Path.ini");

            if (File.Exists(iniPath))
            {
                gamePath = File.ReadAllText(iniPath);
            }

            pbsvc = Path.Combine(gamePath, "pbsvc.exe");
            pbsvcTmp = Path.Combine(pbTempPath, "pbsvc.exe");

            pbAInst = false;
            pbBInst = false;

            Directory.CreateDirectory(fsTemp);
            Directory.CreateDirectory(pbTempPath);

            DLPunkbuster();
        }

        private void DLPunkbuster()
        {
            ProgressBar1.Visibility = Visibility.Visible;

            WebClient webClient = new WebClient();

            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DLPunkbusterCallback);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            webClient.DownloadFileAsync(new Uri("https://github.com/KilLo445/FS-Launcher/raw/master/A_Files/bins/punkbuster.bin"), pnkbstrBin);
        }

        private void DLPunkbusterCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                ZipFile.ExtractToDirectory(pnkbstrBin, pbTempPath);
                File.Delete(pnkbstrBin);

                if (File.Exists(iconFile))
                {
                    this.Icon = new BitmapImage(new Uri(iconFile, UriKind.Relative));
                }

                ProgressBar1.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error:\n\n{ex}");
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBar1.Value = e.ProgressPercentage;
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mwindow = new MainWindow();
            mwindow.Show();
            this.Close();
        }

        private void CheckSys32_Click(object sender, RoutedEventArgs e)
        {
            pbAInst = false;
            pbBInst = false;

            MessageBoxResult checkAB = MessageBox.Show("Are you sure you want to check the installation of PnkBstrA and PnkBstrB?", "Punkbuster", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (checkAB == MessageBoxResult.Yes)
            {
                if (File.Exists(pbAsys))
                {
                    pbAInst = true;
                }

                if (File.Exists(pbBsys))
                {
                    pbBInst = true;
                }

                if (pbAInst == true && pbBInst == true)
                {
                    MessageBox.Show("PnkBstrA.exe and PnkBstrB.exe seem to be installed correctly!");
                }

                else
                {
                    if (pbAInst == false || pbBInst == false)
                    {
                        MessageBoxResult installPnkBstrAB = MessageBox.Show("One or more of the files have not been found, would you like to install them?", "Punkbuster", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (installPnkBstrAB == MessageBoxResult.Yes)
                        {
                            if (pbAInst == false)
                            {
                                try
                                {
                                    File.Copy(pbAtmp, pbAsys);
                                    pbAInst = true;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Error:\n\n{ex}");
                                }
                            }
                            if (pbBInst == false)
                            {
                                try
                                {
                                    File.Copy(pbBtmp, pbBsys);
                                    pbBInst = true;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Error:\n\n{ex}");
                                }
                            }
                            if (pbAInst == true && pbBInst == true)
                            {
                                MessageBox.Show("Both files sucsessfully installed!", "Punkbuster");
                            }
                            else
                            {
                                MessageBox.Show("One or more of the files failed to install, please try again.", "Punkbuster");
                            }
                        }
                    }
                }
            }
        }
        private void CheckSys32_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBoxResult forcePBinstall = MessageBox.Show("Are you sure you want to force the installation of PnkBstrA and PnkBstrB?", "Punkbuster", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (forcePBinstall == MessageBoxResult.Yes)
            {
                pbAInst = false;
                pbBInst = false;

                if (File.Exists(pbAsys))
                {
                    try
                    {
                        File.Delete(pbAsys);
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error:\n\n{ex}");
                    }
                }

                if (File.Exists(pbBsys))
                {
                    try
                    {
                        File.Delete(pbBsys);
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error:\n\n{ex}");
                    }
                }

                try
                {
                    File.Copy(pbAtmp, pbAsys);
                    pbAInst = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error:\n\n{ex}");
                }

                try
                {
                    File.Copy(pbBtmp, pbBsys);
                    pbBInst = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error:\n\n{ex}");
                }

                if (pbAInst == true && pbBInst == true)
                {
                    MessageBox.Show("Both files sucsessfully installed!", "Punkbuster");
                }
                else
                {
                    MessageBox.Show("One or more of the files failed to install, please try again.", "Punkbuster");
                }
            }  
        }

        private void TestInstall_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(pbsvc))
            {
                Process.Start(pbsvc);
            }
            else
            {
                Process.Start(pbsvcTmp);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (Directory.Exists(pbTempPath))
            {
                Directory.Delete(pbTempPath, true);
            }
        }

        private void PnkBstrLogo_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("https://www.evenbalance.com/");
        }
    }
}
