using log4net;
using NchargeL;
using NCLCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        private string ver = "1.1.5-4";

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
        private void checkUpdate()
        {
            string re1 = HttpRequestHelper.GetResponseString(HttpRequestHelper.CreatePostHttpResponse("http://download.ncserver.top:8000/NCL/config.json", new Dictionary<String, String>()));
            var jObject = JObject.Parse(re1);
            if (jObject["ver"].ToString() == ver)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {

                    InfoDialog warn = new InfoDialog("", "当前版本:" + ver + "与服务器版本匹配,是最新版本");
                    warn.ShowDialog();
                })).Wait();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {

                    InfoDialog warn = new InfoDialog("", "当前版本:" + ver + "与服务器版本不匹配\n最新版本:" + jObject["ver"].ToString() + "请在http://download.ncserver.top:9000/选择启动按钮手动进行更新");
                    warn.ShowDialog();
                })).Wait();
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
            this.Show();
            //long timeStamp = DateTimeOffset.Now.ToUniversalTime().Ticks; 
            log.Info("初始化....");
            // log.Info(NchargeL.Properties.Settings.Default.User._token);
            if (NchargeL.Properties.Settings.Default.First)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                    {

                        InfoDialog warn = new InfoDialog("关于服务器", "本服务器从2019年6月开设以来已经经历了2年零8个月。\n" +
                            "在这段时间中，我们共同经历了很多：\n2019年\n①服务器出现地图生成问题，腐竹不记日夜维修（内部全是活板门的山、总是复活在方块内部……）；" +
                            "\n②服务器系统十分不稳定，腐竹不厌其烦地尝试各种系统\n2020年\n①在腐竹以及全体管理的巡查下，查出一位在地下安放大量RTG导致服务器异常卡顿的玩家，而后此玩家在群聊中出言不逊，被踢出后又在MCMOD中发表虚假言论诋毁服务器，但在腐竹和几名正义感十足的玩家的反驳后，最终销声匿迹；" +
                            "\n②腐竹开始编写本服务器的自研启动器，并且一步一步地更新和修复\n2021年\n①服务器因为腐竹个人原因（具体情况当时在的人应该都知道）被强迫关停近3个月，但在腐竹不屈不挠的争取下，服务器再次开始营业；" +
                            "\n②腐竹下定决心开始编写自研V6启动器，在经历了无数个苦苦编程的夜晚后，终于在年中成功推出V6版本启动器于2021年下半年（11月份左右）开始计划使用C#编写的NET版本启动器已完成大部分构建，最终于2月12日晚推出." +
                            "\n2022年\n全体玩家、全体管理员，以及我们辛勤的腐竹大人，在这近3年的时间里，我们共同见证了服务器一点一点的改进，一点一点的优化，在千难万险后最终取得了现在非凡的成就！" +
                            "\n本服务器一直秉持着不收一分钱纯公益服的理念，尽力为大家创造了一个优良的游戏环境。但时间流逝，每月仅服务器电费就高达100元。而且腐竹投入的大量时间与精力也并无回报。尽管有时会有一些玩家热心捐赠，但也远远无法补足这巨大的支出。服务器从最开始就一直处于只出不进的情况。" +
                            "\n说了那么多，最后还是要感谢大家对我们的支持和鼓励，也希望有更多的玩家能在某些方面去支持我们！谢谢！", 3);
                        // warn.time = 5;
                        warn.ShowDialog();
                    })).Wait();
                NchargeL.Properties.Settings.Default.First = false;
            }

            check64();
            checkUpdate();
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
