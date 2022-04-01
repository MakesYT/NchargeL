using Enterwell.Clients.Wpf.Notifications;
using log4net;
using Microsoft.WindowsAPICodePack.Dialogs;
using NCLCore;
using Newtonsoft.Json.Linq;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NchargeL
{
    /// <summary>
    /// Launcher.xaml 的交互逻辑
    /// </summary>
    public partial class Manager : Page
    {
      
        NotificationManager notificationManager = new NotificationManager();

        private static readonly ILog log = LogManager.GetLogger("Manager");
        public Manager()
        {

            InitializeComponent();
            //this.list.ItemsSource = Data.clients;//数据源
            //this.list.DisplayMemberPath = "Name";//路径
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            group.Visibility= Visibility.Visible;
            Client client = ((Client)((ListBox)e.OriginalSource).SelectedItem);
            info.Text = "当前选择的客户端:\n" + client.Name ;
            if (client.NchargeVer != null) info.Text += "\nNcharge版本:\n" + client.NchargeVer;
            else info.Text += "\nNcharge版本:\n非Ncharge客户端" ;
            // (ListBoxItem)((ListBox)e.Source).
        }

        private void del_click(object sender, RoutedEventArgs e)
        {
            Client client = ((Client)(list.SelectedItem));
            INotificationMessage msg = null;
            LauncherClient launchc = new LauncherClient();
            string name =null;
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                var progress = new ProgressBar
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 3,
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                    Background = Brushes.Transparent,
                    IsIndeterminate = true,
                    IsHitTestVisible = false,
                    Value = 10
                };
                progress.Value = 10;
                msg = Main.main.Manager.CreateMessage()
                .Accent("#F15B19")
                .Background("#F15B19")
                .HasHeader("")
                .HasMessage("删除"+client.Name+"客户端中.......")
                .WithOverlay(progress)
                .Queue();
                name = Data.clients[list.SelectedIndex].Name;
            })).Wait();
            Task.Factory.StartNew(() => delFolor(Properties.Settings.Default.GameDir + "\\versions\\" + name,msg));
        }
        private void delFolor(string dir, INotificationMessage msg)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            di.Delete(true);
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                Main.main.Manager.Dismiss(msg);
                Data.clients = ClientTools.GetALLClient(Properties.Settings.Default.GameDir);
                notificationManager.Show(NotificationContentSDK.notificationSuccess("客户端列表已更新", ""), "WindowArea");
                Main.main.ManagerUi = new Manager();
                //Main.main.launcher.NCLCore = nCLCore;
                Main.main.FrameWork.Content = Main.main.ManagerUi;
            })).Wait();
        }


        private void logs_Click(object sender, RoutedEventArgs e)
        {

            ExecuteInCmd("start \"\" \"" + Directory.GetCurrentDirectory() + "\\logs\"", "");
            // System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() );
        }
        private void mods_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedIndex >= 0)
            {
               
                ExecuteInCmd("start \"\" \"" + Properties.Settings.Default.GameDir + "\\versions\\"+ Data.clients[list.SelectedIndex].Name+"\\mods\"", "");
            }
            else notificationManager.Show(NotificationContentSDK.notificationWarning("请先选择启动的客户端", ""), "WindowArea");
            // System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() );
        }
        private void GameDir_Click(object sender, RoutedEventArgs e)
        {

            ExecuteInCmd("start \"\" \"" + Properties.Settings.Default.GameDir+ "\"", "");
            // System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() );
        }
        public string ExecuteInCmd(string cmdline, string dir)
        {

            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = false;
                // process.StartInfo.
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                // process.StandardInput.AutoFlush = true;
                process.StandardInput.WriteLine("cd /d " + dir);
                process.StandardInput.WriteLine(cmdline + "&exit");
                process.StandardInput.Close();
                string line;
                while ((line = process.StandardOutput.ReadLine()) != null)
                {
                    log.Debug(line);
                }
                //获取cmd窗口的输出信息  
                // string output = process.StandardOutput.ReadToEnd();
                // process.StandardOutput.
                process.WaitForExit();
                process.Close();

                return "";
            }
        }
        private void GameDir(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            //dlg.InitialDirectory = currentDirectory;
            dlg.Title = "选择\".minecraft\"游戏目录";
            while (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (dlg.FileName.EndsWith(".minecraft"))
                {
                    Properties.Settings.Default.GameDir = dlg.FileName;
                    //NCLcore nCLCore = Main.main.newNCLcore(Properties.Settings.Default.DownloadSource, );
                    Data.clients = ClientTools.GetALLClient(dlg.FileName);
                    notificationManager.Show(NotificationContentSDK.notificationSuccess("客户端列表已更新", ""), "WindowArea");
                    Main.main.launcher = new Launcher();
                    //Main.main.launcher.NCLCore = nCLCore;
                    Main.main.FrameWork.Content = Main.main.launcher;
                    break;
                }
                else
                {
                    InfoDialog info = new InfoDialog("选择游戏目录", "您需要选择以.minecraft命名的文件夹");
                    info.ShowDialog();
                }

            }
        }

        private void JavaDir(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Filters.Add(new CommonFileDialogFilter("javaw.exe", "exe"));
            dlg.IsFolderPicker = false;
            //dlg.InitialDirectory = currentDirectory;
            dlg.Title = "选择\"javaw.exe\"";

            while (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (dlg.FileName.EndsWith("\\javaw.exe"))
                {
                    Properties.Settings.Default.Java = dlg.FileName;

                    break;
                }
                else
                {
                    InfoDialog info = new InfoDialog("选择了" + dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1), "您需要选择以javaw.exe命名的文件");
                    info.ShowDialog();
                }

            }
        }

        private void limitnumber(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }
    }
}
