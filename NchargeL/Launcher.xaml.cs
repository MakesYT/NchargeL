using log4net;
using NchargeL.Info;
using NCLCore;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NchargeL
{
    /// <summary>
    /// Launcher.xaml 的交互逻辑
    /// </summary>
    public partial class Launcher : Page
    {
        public LoginUi LoginUi = new LoginUi();
        public NotificationManager notificationManager = new NotificationManager();
        private static readonly ILog log = LogManager.GetLogger("Launcher");
        public Launcher()
        {

            InitializeComponent();
            //this.list.ItemsSource = Data.clients;//数据源
            //this.list.DisplayMemberPath = "Name";//路径
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Client client = new Client();
            client.Id = Data.clients.Count + 1;
            client.Name = "rrrr";
            Data.clients.Add(client);
            //this.list.ItemsSource = Data.clients;//数据源
            //this.list.DisplayMemberPath = "Name";//路径
           
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private   async void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            // notificationManager.Show(NotificationContentSDK.notificationSuccess( "",logs.Result), "WindowArea");
            if (Data.users.Count > 0)
            {//当前有账号登录
                if (list.SelectedIndex>=0)
                {
                    notificationManager.Show(NotificationContentSDK.notificationInformation("正在启动客户端", ""), "WindowArea");
                    ((Button)sender).IsEnabled = false;
                Thread thread = new Thread(new ParameterizedThreadStart(StartClient));
                thread.Start(Data.clients[list.SelectedIndex]);
                
                }else notificationManager.Show(NotificationContentSDK.notificationWarning("请先选择启动的客户端", ""), "WindowArea");

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
        public  void  StartClient(object clt)
        {
                SDK sDK = new SDK();
           var re=   sDK.StartClient((Client)clt, Data.users[0]._name, Data.users[0]._useruuid, Data.users[0]._token);
            log.Debug(re.Result);
            //StartClient(clt).GetAwaiter().GetResult().GetAwaiter().GetResult();
            notificationManager.Show(NotificationContentSDK.notificationInformation("", "客户端已退出"), "WindowArea");
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                launch.IsEnabled = true; 
            })).Wait();
            

        }

        private void logs_Click(object sender, RoutedEventArgs e)
        {
            SDK sDK = new SDK();
            sDK.ExecuteInCmd("start "+Directory.GetCurrentDirectory()+"\\logs", "");
           // System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() );
        }
    }
}
