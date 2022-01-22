using NchargeL.Info;
using NCLCore;
using Notification.Wpf;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace NchargeL
{
    /// <summary>
    /// Launcher.xaml 的交互逻辑
    /// </summary>
    public partial class Launcher : Page
    {
        public NotificationManager notificationManager = new NotificationManager();
        public Launcher()
        {

            InitializeComponent();
            //this.list.ItemsSource = Data.clients;//数据源
            //this.list.DisplayMemberPath = "Name";//路径
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            notificationManager.Show(NotificationContentSDK.notificationSuccess(Data.clients.Count.ToString(), ""), "WindowArea");
            Client client =new Client();
            client.Name = "TEST";
            Data.clients.Add(client);
            //this.list.ItemsSource = Data.clients;
            notificationManager.Show(NotificationContentSDK.notificationSuccess(list.Items.Count.ToString(), ""), "WindowArea");
        }
    }
}
