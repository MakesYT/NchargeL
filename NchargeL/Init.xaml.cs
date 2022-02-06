using log4net;
using NchargeL;
using System;
using System.Windows;
using System.Windows.Input;

namespace Ncharge
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger("Init");
        private string ver = "1.0.0-net-alpha-only-owner";

        private void check64()
        {
            if (!Environment.Is64BitOperatingSystem)
            {
                ErrorDialog error = new ErrorDialog("", "您当前使用的系统为32位,Java内存无法超过1G,\r\n无法启动本服务器的客户端,按确定退出启动器");
                error.ShowDialog();
                Application.Current.Shutdown();
                //MessageBox.Show("您当前使用的系统为32位,Java内存无法超过1G,\r\n无法启动本服务器的客户端,按确定退出启动器", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }
        private void checkInsider()
        {
            if (ver.IndexOf("alpha") != -1)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {

                    WarnDialog warn = new WarnDialog("", "您当前使用的是Alpha内测版本\r\n存在未知bug,如遇bug请反馈");
                    warn.ShowDialog();
                })).Wait();
                //MessageBox.Show("您当前使用的是Alpha内测版本\r\n存在未知bug,如遇bug请反馈", "警告", MessageBoxButton.OK, MessageBoxImage.Question);

            }
            else if (ver.IndexOf("beta") != -1)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {

                    WarnDialog warn = new WarnDialog("", "您当前使用的是Beta公测版本\r\n可能存在bug,如遇bug请反馈");
                    warn.ShowDialog();
                })).Wait();
                //MessageBox.Show("您当前使用的是Beta公测版本\r\n可能存在bug,如遇bug请反馈", "警告", MessageBoxButton.OK, MessageBoxImage.Question);

            }
        }


        public MainWindow()
        {
            InitializeComponent();

            //long timeStamp = DateTimeOffset.Now.ToUniversalTime().Ticks; 
            log.Info("初始化....");
            // log.Info(NchargeL.Properties.Settings.Default.User._token);

            check64();

            checkInsider();
            Main main = new Main();
            main.Show();
            this.Close();
            //log.Info(DateTimeOffset.Now.ToUniversalTime().Ticks - timeStamp);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            closeDialog close = new closeDialog();
            close.ShowDialog();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
