﻿using log4net;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NchargeL
{
    /// <summary>
    /// Launcher.xaml 的交互逻辑
    /// </summary>
    public partial class Launcher : Page
    {
        public LoginUi LoginUi = new LoginUi();
        NotificationManager notificationManager = new NotificationManager();
        
        private static readonly ILog log = LogManager.GetLogger("Launcher");
        public Launcher()
        {

            InitializeComponent();
            //this.list.ItemsSource = Data.clients;//数据源
            //this.list.DisplayMemberPath = "Name";//路径
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Client client = ((Client)((ListBox)e.OriginalSource).SelectedItem);
            info.Text = "当前选择的客户端:\n" + client.Name+"\nAssets:"+client.assets;
            // (ListBoxItem)((ListBox)e.Source).
        }

        private async void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            // notificationManager.Show(NotificationContentSDK.notificationSuccess( "",logs.Result), "WindowArea");
            if (Data.users.Count > 0)
            {//当前有账号登录
                if (list.SelectedIndex >= 0)
                {
                    ((Button)sender).IsEnabled = false;
                    // notificationManager.Show(NotificationContentSDK.notificationInformation("正在检查Java合理性", ""), "WindowArea");
                    
                    int javaVer = ClientTools.JavaCheck(Properties.Settings.Default.Java, Data.clients[list.SelectedIndex]);
                    switch (javaVer)
                    {
                        case 0:
                            {
                                //notificationManager.Show(NotificationContentSDK.notificationInformation("正在启动客户端", ""), "WindowArea");

                                Thread thread = new Thread(new ParameterizedThreadStart(StartClient));
                                thread.Start(Data.clients[list.SelectedIndex]);
                                break;
                            }
                        case -1:
                            {
                                notificationManager.Show(NotificationContentSDK.notificationError("Java版本为32位,停止启动", ""), "WindowArea");
                                launch.IsEnabled = true;
                                break;
                            }
                        case 1:
                            {
                                notificationManager.Show(NotificationContentSDK.notificationError("Java版本不正确,请使用Java8", ""), "WindowArea");
                                launch.IsEnabled = true;
                                break;
                            }
                        default:
                            {
                                notificationManager.Show(NotificationContentSDK.notificationInformation("可以尝试使用Java" + javaVer + "来获得更好的体验", ""), "WindowArea");

                                Thread thread = new Thread(new ParameterizedThreadStart(StartClient));
                                thread.Start(Data.clients[list.SelectedIndex]);
                                break;
                            }
                    }


                }
                else notificationManager.Show(NotificationContentSDK.notificationWarning("请先选择启动的客户端", ""), "WindowArea");

            }
            else//无账号跳转登录界面
            {
                notificationManager.Show(NotificationContentSDK.notificationWarning("请先登录账号", ""), "WindowArea");
                Main.main.FrameWork.Content = LoginUi;
            }

            //await StartClient(Data.clients[list.SelectedIndex]).GetAwaiter().GetResult();
            //await Task.Run(() => StartClient(Data.clients[list.SelectedIndex]));
            //log.Debug(logs.Result);
            //notificationManager.Show(NotificationContentSDK.notificationSuccess( "",logs.Result), "WindowArea");
        }
        public void StartClient(object clt)
        {

            LauncherClient launchc = new LauncherClient();
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                                       {
                                           //try
                                           {
                                               var progress = notificationManager.ShowProgressBar("启动游戏", true, true, "WindowArea", false, 1, null, false, true,
                                         new SolidColorBrush(Properties.Settings.Default.BodyColorS),
                                       new SolidColorBrush(Properties.Settings.Default.ForegroundColor));
                                               //   double nowProgress = 0;
                                               Main.main.infoManager.PropertyChanged += (oo, ee) =>
                                    {
                                        Info info = (oo as InfoManager).info;
                                        log.Debug(info.process + " " + info.msg);
                                        // if (info.process > nowProgress)
                                        {
                                            // nowProgress = info.process;

                                            //progress.Cancel.ThrowIfCancellationRequested();


                                            if (info.process == 100)
                                            {
                                                progress.Dispose();
                                            }
                                            else
                                                progress.Report((info.process, info.msg, null, null));

                                        }




                                    };
                                           }
                                           //catch (Exception ex) { log.Error(ex); }
                                       })).Wait();
            bool flag = true;
            while (flag)
            {

                // Task<int> task = Task.Factory.StartNew<int>(() => DownloadString("http://ww.linqpad.net"));
                var re = launchc.StartClient(Main.main.infoManager, (Client)clt, Properties.Settings.Default.DownloadSource, Data.users[0]._name, Data.users[0]._useruuid, Data.users[0]._token, Properties.Settings.Default.Java, Properties.Settings.Default.RAM);
                log.Debug(re);

                //StartClient(clt).GetAwaiter().GetResult().GetAwaiter().GetResult();
                if (re == 1)
                {
                    notificationManager.Show(NotificationContentSDK.notificationInformation("", "客户端已退出"), "WindowArea");
                    flag = false;
                    Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        launch.IsEnabled = true;
                    })).Wait();
                }
                if (re == 2)
                {
                    if (Data.users[0]._password != null)
                    {

                        notificationManager.Show(NotificationContentSDK.notificationWarning("", "用户令牌已失效,正在重新获取"), "WindowArea");


                        Dictionary<String, String> pList = new Dictionary<String, String>();
                        pList.Add("username", Data.users[0]._email);
                        pList.Add("password", Data.users[0]._password);


                        try
                        {
                            string re1 = HttpRequestHelper.GetResponseString(HttpRequestHelper.CreatePostHttpResponse("https://www.ncserver.top:666/api/yggdrasil/authserver/authenticate", pList));
                            var jObject = JObject.Parse(re1);
                            log.Debug(jObject.ToString());
                            Data.users[0]._token = jObject["accessToken"].ToString();
                            Properties.Settings.Default.User = XmlUtil.Serializer(typeof(User), Data.users[0]);

                        }
                        catch (Exception ex)
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                            {
                                WarnDialog warn = new WarnDialog(Data.users[0]._password, "因账号密码已更改,用户令牌无法重新获取,请重新登录");
                                warn.ShowDialog();
                                Main.main.FrameWork.Content = LoginUi;
                            })).Wait();
                            Data.users.Clear();
                            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                            {
                                launch.IsEnabled = true;
                            })).Wait();
                            flag = false;
                            log.Debug(ex.ToString());
                            log.Debug(ex.Message);
                        }
                    }
                    else
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            WarnDialog warn = new WarnDialog(Data.users[0]._password, "用户令牌已失效请重新登陆");
                            warn.ShowDialog();
                            Main.main.FrameWork.Content = LoginUi;
                        })).Wait();
                        Data.users.Clear();
                        Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                        {
                            launch.IsEnabled = true;
                        })).Wait();
                        flag = false;
                    }

                }


            }

            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                var progress = notificationManager.ShowProgressBar("启动游戏", true, true, "WindowArea", false, 1, null, false, true,
         new SolidColorBrush(Properties.Settings.Default.BodyColorS),
       new SolidColorBrush(Properties.Settings.Default.ForegroundColor));
                double nowProgress = 0;
                Main.main.infoManager.PropertyChanged -= (oo, ee) =>
                 {
                     Info info = (oo as InfoManager).info;
                     log.Debug(info.process + " " + info.msg);
                     // if (info.process > nowProgress)
                     {
                         nowProgress = info.process;
                         progress.Cancel.ThrowIfCancellationRequested();
                         progress.Report((info.process, info.msg, null, null));
                         if (info.process == 100)
                         { progress.CancelSource.Cancel(); }
                     }




                 };
            })).Wait();
        }

        private void logs_Click(object sender, RoutedEventArgs e)
        {
            
            ExecuteInCmd("start " + Directory.GetCurrentDirectory() + "\\logs", "");
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
