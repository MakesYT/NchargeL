using log4net;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;

namespace NchargeL
{
    /// <summary>
    /// InfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class closeDialog : Window
    {
        private static readonly ILog log = LogManager.GetLogger("closeDialog");
        public closeDialog()
        {
            InitializeComponent();

            var storyboard = (Storyboard)this.FindResource("Storyboard1");
            storyboard.Begin();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            StopProcess("wget.exe");
            this.Close();
            Main.main.Close();
            // System.Environment.Exit(0);
            //  System.Threading.ThreadPool.
            Process.GetCurrentProcess().Kill();
            Application.Current.Shutdown();
            Environment.Exit(0);
        }
        public static void StopProcess(string processName)
        {
            try
            {
                Process[] processes = System.Diagnostics.Process.GetProcesses();
                foreach (Process item in processes)
                {
                    if (item.ProcessName.Contains(processName.Replace(".exe", "")))
                    {
                        item.Kill();
                        log.Info("已杀死"+item.Id);
                       // break;
                    }
                }
            }
            catch
            {

            }
        }
    }
}
