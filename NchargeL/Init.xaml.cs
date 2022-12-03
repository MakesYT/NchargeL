using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using log4net;
using NchargeL;
using NchargeL.Properties;
using NCLCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Color = System.Windows.Media.Color;

namespace Ncharge;

/// <summary>
///     MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    private static readonly ILog log = LogManager.GetLogger("Init");
    private readonly string ver = "1.4.5-5";


    public MainWindow()
    {
        InitializeComponent();
        Show();
        //long timeStamp = DateTimeOffset.Now.ToUniversalTime().Ticks; 
        log.Info("初始化....");
        var ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //File.WriteAllText(ApplicationData + "\\config.json", jObject.ToString(Formatting.Indented));
        if(!Directory.Exists(ApplicationData+ "\\NchargeL"))
        {
            Directory.CreateDirectory(ApplicationData + "\\NchargeL");
            log.Info("AppData目录已创建");
        }
        if (File.Exists(ApplicationData + "\\NchargeL\\config.json"))
            try
            {
                log.Debug("读取配置信息");
                using var jsonfile = File.OpenText(ApplicationData + "\\NchargeL\\config.json");

                using var reader = new JsonTextReader(jsonfile);
                var jObject = (JObject) JToken.ReadFrom(reader);


                var converter = new ColorConverter();

                // Color color = (Color)converter.ConvertFromString("#FFDFD991");
                Settings.Default.BodyColorS =
                    (Color) System.Windows.Media.ColorConverter.ConvertFromString(
                        jObject["BodyColorS"].ToString());
                Settings.Default.NotificationSuccess =
                    (Color) System.Windows.Media.ColorConverter.ConvertFromString(
                        jObject["NotificationSuccess"].ToString());
                Settings.Default.NotificationError =
                    (Color) System.Windows.Media.ColorConverter.ConvertFromString(
                        jObject["NotificationError"].ToString());
                Settings.Default.NotificationWarning =
                    (Color) System.Windows.Media.ColorConverter.ConvertFromString(
                        jObject["NotificationWarning"].ToString());
                Settings.Default.TextColor =
                    (Color) System.Windows.Media.ColorConverter.ConvertFromString(
                        jObject["TextColor"].ToString());
                Settings.Default.ForegroundColor =
                    (Color) System.Windows.Media.ColorConverter.ConvertFromString(
                        jObject["ForegroundColor"].ToString());
                Settings.Default.BackgroundColor =
                    (Color) System.Windows.Media.ColorConverter.ConvertFromString(
                        jObject["BackgroundColor"].ToString());
                Settings.Default.DownloadSource = jObject["DownloadSource"].ToString();
                Settings.Default.GameDir = jObject["GameDir"].ToString();
                Settings.Default.User = jObject["User"].ToString();
                Settings.Default.Java = jObject["Java"].ToString();
                Settings.Default.RAM = jObject["RAM"].ToObject<int>();
                Settings.Default.First = false;
                Settings.Default.width = jObject["width"].ToObject<int>();
                Settings.Default.height = jObject["height"].ToObject<int>();
            }
            catch (Exception e)
            {
                log.Debug("读取旧配置文件失败" + e);
                var info = new InfoDialog("", "加载配置文件时出现异常,配置文件版本疑似损坏,请尝试重新打开启动器,检查问题是否解决");
                info.ShowDialog();
            }
        check64();
        checkUpdate();
        checkInsider();
        // log.Info(NchargeL.Properties.Settings.Default.User._token);
        if (Settings.Default.First)
        {
            Settings.Default.First = false;

            //进入启动器设置引导
            var info = new InfoDialog("设置引导", "欢迎使用Ncharge服务器的专属启动器NCL\n下面将带领您完成启动器基本设置");
            info.ShowDialog();
            LeadingUi leadingUi = new LeadingUi();
            leadingUi.Show();
            Close();

        }
        else
        {
            var main = new Main();
            main.Show();
            Close();
        }
        
        
        //log.Info(DateTimeOffset.Now.ToUniversalTime().Ticks - timeStamp);
    }

    private void check64()
    {
        if (!Environment.Is64BitOperatingSystem)
        {
            var error = new ErrorDialog("", "您当前使用的系统为32位,Java内存无法超过1G,\r\n无法启动本服务器的客户端,按确定退出启动器");
            error.ShowDialog();
            Application.Current.Shutdown();
            //MessageBox.Show("您当前使用的系统为32位,Java内存无法超过1G,\r\n无法启动本服务器的客户端,按确定退出启动器", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(0);
        }
    }

    private void checkUpdate()
    {
      //  var re1 = HttpRequestHelper.GetResponseString(
       //   HttpRequestHelper.CreatePostHttpResponse("https://download.ncserver.top:8000/NCL/config.json", new Dictionary<string, string>()));
       var re1= HttpRequestHelper.getHttpTool("https://download.ncserver.top:8000/NCL/config.json");
       
        var jObject = JObject.Parse(re1.Result);
        if (jObject["ver"].ToString() == ver)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                var warn = new InfoDialog("", "当前版本:" + ver + "与服务器版本匹配,是最新版本");
                warn.ShowDialog();
            })).Wait();
        }
        else
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                var warn = new InfoDialog("",
                    "当前版本:" + ver + "与服务器版本不匹配\n最新版本:" + jObject["ver"] +
                    "请在http://download.ncserver.top:9000/选择启动按钮手动进行更新");
                warn.ShowDialog();
            })).Wait();
            Environment.Exit(0);
        }
    }

    private void checkInsider()
    {
        if (ver.IndexOf("alpha") != -1)
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                var warn = new WarnDialog("", "您当前使用的是Alpha内测版本\r\n存在未知bug,如遇bug请反馈");
                warn.ShowDialog();
            })).Wait();
        //MessageBox.Show("您当前使用的是Alpha内测版本\r\n存在未知bug,如遇bug请反馈", "警告", MessageBoxButton.OK, MessageBoxImage.Question);
        else if (ver.IndexOf("beta") != -1)
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                var warn = new WarnDialog("", "您当前使用的是Beta公测版本\r\n可能存在bug,如遇bug请反馈");
                warn.ShowDialog();
            })).Wait();
        //MessageBox.Show("您当前使用的是Beta公测版本\r\n可能存在bug,如遇bug请反馈", "警告", MessageBoxButton.OK, MessageBoxImage.Question);
    }


    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }


    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}