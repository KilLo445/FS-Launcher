using System;
using System.Diagnostics;
using System.Windows;

namespace FS_Launcher
{
    /// <summary>
    /// Interaction logic for Firewall.xaml
    /// </summary>
    public partial class Firewall : Window
    {
        public Firewall()
        {
            InitializeComponent();
        }
        public void ExecuteAsAdmin(string fileName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = fileName;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            proc.Start();
        }
    }
}
