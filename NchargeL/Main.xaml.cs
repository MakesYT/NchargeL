using log4net;
using Microsoft.WindowsAPICodePack.Dialogs;
using NchargeL.Info;
using NCLCore;
using Notification.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using static NCLCore.SDK;

namespace NchargeL
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        public NotificationManager notificationManager = new NotificationManager();
        public static Main main;

        private static readonly ILog log = LogManager.GetLogger("Main");
        // public Data data = new Data();
        public Home Home = new Home();
        public Account Account = new Account();
        public LoginUi LoginUi = new LoginUi();
        private SettingUi settingUi = new SettingUi();
        public Launcher launcher=new Launcher();
        public AboutNCL aboutNCL = new AboutNCL();
        public DownloadUI  downloadUI = new DownloadUI();
        public Main()
        {
            InitializeComponent();
            Data.init();//初始化列表

            main = this;
            hello.Text = Environment.UserName;

            FrameWork.Content = new Frame() { Content = Home };
            if (Data.users.Count > 0)
            {
                Data.users[0].reloadUser();
            }
        }
        public void InfoDialogShow(string infostr, string str)
        {
            info.Text = infostr;
            text.Text = str;
            host.IsOpen = true;
            var storyboard = (Storyboard)this.FindResource("Storyboard1");
            storyboard.Begin();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //log.Info(e.LeftButton.ToString().Equals("Pressed"));
            if (e.LeftButton.ToString().Equals("Pressed"))
                DragMove();
        }//鼠标拖拽兼容

        private void Close(object sender, RoutedEventArgs e)
        {
            closeDialog close = new closeDialog();
            close.ShowDialog();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //log.Debug(Application.Current.MainWindow.Content.ToString());
            //NavigationService.GetNavigationService(this).Navigate(new L());
            // FrameWork.Content = L;
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //var a = Application.Current.Resources.MergedDictionaries;



        }
        private void HomeClick(object sender, RoutedEventArgs e)
        {
            //UserContent = Home;
            FrameWork.Content = Home;
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation withAnimation = new DoubleAnimation();
            withAnimation.From = can.ActualWidth;
            withAnimation.To = 150;
            withAnimation.Duration = TimeSpan.FromSeconds(0.2);
            can.BeginAnimation(Button.WidthProperty, withAnimation);

        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation withAnimation = new DoubleAnimation();
            withAnimation.From = can.ActualWidth;
            withAnimation.To = 50;
            withAnimation.Duration = TimeSpan.FromSeconds(0.2);
            can.BeginAnimation(Button.WidthProperty, withAnimation);
        }

        private void ViaClick(object sender, RoutedEventArgs e)//头像按钮事件
        {
            if (Data.users.Count > 0)
            {//当前有账号登录
                FrameWork.Content = Account;
            }
            else//无账号跳转登录界面
            {
                FrameWork.Content = LoginUi;
            }
            //NavigationService.GetNavigationService(this).Navigate(new LoginUi());
        }

        private void Setting(object sender, RoutedEventArgs e)
        {
            FrameWork.Content = settingUi;
        }

        private async void LauncherButton(object sender, RoutedEventArgs e)//启动游戏按钮
        {
            //launcher=new Launcher();
            //Properties.Settings.Default.GameDir = @"D:\\IDEAJava\\6th\\V6\\out\\artifacts\\V6_jar\\.minecraft";
            if (Properties.Settings.Default.GameDir != "")
            {
                NCLcore nCLCore = new NCLcore(Properties.Settings.Default.DownloadSource, Properties.Settings.Default.GameDir);
                Data.clients = nCLCore.Clients;
                notificationManager.Show(NotificationContentSDK.notificationSuccess("客户端列表已更新", ""), "WindowArea");
                launcher = new Launcher();
                launcher.NCLCore = nCLCore;
                FrameWork.Content = launcher;
            }
            else
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
                        NCLcore nCLCore = new NCLcore(Properties.Settings.Default.DownloadSource, dlg.FileName);
                        Data.clients = nCLCore.Clients;
                        notificationManager.Show(NotificationContentSDK.notificationSuccess("客户端列表已更新", ""), "WindowArea");
                        launcher = new Launcher();
                        launcher.NCLCore= nCLCore;
                        FrameWork.Content = launcher;
                        break;
                    }
                    else
                    {
                        InfoDialog info = new InfoDialog("选择游戏目录", "您需要选择以.minecraft命名的文件夹");
                        info.ShowDialog();
                    }

                }

            }

        }

        private void Minimze(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void AboutButton(object sender, RoutedEventArgs e)
        {

            FrameWork.Content = aboutNCL;
        }

        private void ManageClient(object sender, RoutedEventArgs e)
        {
            ErrorDialog error = new ErrorDialog("", "（1）发生了一个错误！请联系腐竹！" + Environment.NewLine
                                + "                                                                        1");
            error.ShowDialog();
        }

        private void DownloadUiButton(object sender, RoutedEventArgs e)
        {
            FrameWork.Content = downloadUI;
        }
    }
}
