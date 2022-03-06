using log4net;
using NCLCore;
using Newtonsoft.Json.Linq;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
namespace NchargeL
{
    /// <summary>
    /// LoginUi.xaml 的交互逻辑
    /// </summary>
    public partial class LoginUi : Page
    {
        private string tempEmail;
        //private Data data=Main.data
        private string tempPassword;
        NotificationManager notificationManager = new NotificationManager();
        private static readonly ILog log = LogManager.GetLogger("Login");
        //Regex reg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        public LoginUi()
        {

            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process proc = new System.Diagnostics.Process();

            proc.StartInfo.FileName = "https://www.ncserver.top:666/auth/register";

            proc.Start();
        }
        private void LoginThread()
        {

            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                text.Content = "登录中";
                tempEmail = email.Text;
                tempPassword = pwd.Password;
            });
            Dictionary<String, String> pList = new Dictionary<String, String>();
            pList.Add("username", tempEmail);
            pList.Add("password", tempPassword);


            try
            {
                string re = HttpRequestHelper.GetResponseString(HttpRequestHelper.CreatePostHttpResponse("https://www.ncserver.top:666/api/yggdrasil/authserver/authenticate", pList));
                var jObject = JObject.Parse(re);
                log.Debug(jObject.ToString());
                bool f = false;
                if (jObject["selectedProfile"] == null)
                {

                    log.Debug(jObject["error"].ToString());
                    log.Debug(jObject["errorMessage"].ToString());
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()

                    {
                        if (jObject["error"].ToString() == "ForbiddenOperationException")
                        {
                            host.IsOpen = false;
                            if (jObject["errorMessage"].ToString() == "Your email isn't verified. Please verify it before logging in.")
                            {
                                Main.main.InfoDialogShow("登录失败", "请先验证邮箱");
                                Process proc = new System.Diagnostics.Process();

                                proc.StartInfo.FileName = "https://www.ncserver.top:666/user";

                                proc.Start();
                            }
                            else
                                Main.main.InfoDialogShow("登录失败", "账号或密码错误");

                        }


                    });
                }
                else//登录成功
                {

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        text.Content = "登录成功,获取信息中";

                        notificationManager.Show(NotificationContentSDK.notificationSuccess("登录成功", ""), "WindowArea");


                    });
                    User user = new User(jObject, tempEmail, tempPassword);
                    Data.users.Add(user);
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                    {
                        if (save.IsChecked == false || save.IsChecked == null)
                        {
                            user._password = null;
                            notificationManager.Show(NotificationContentSDK.notificationSuccess("", "没有保存密码"), "WindowArea");
                        }
                        else notificationManager.Show(NotificationContentSDK.notificationSuccess(user._password, "已保存密码"), "WindowArea");

                        Properties.Settings.Default.User = XmlUtil.Serializer(typeof(User), user);
                        host.IsOpen = false;
                        Main.main.FrameWork.Content = Account.account;


                    }).Wait();
                    // log.Debug(jObject.ToString());
                }



            }
            catch (Exception ex)
            {
                log.Debug(ex.ToString());
                log.Debug(ex.Message);

                this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()

                {
                    host.IsOpen = false;
                    Main.main.InfoDialogShow("登录失败,错误信息如下", ex.ToString());

                });
            }
            //


        }
        private void LoginButoon(object sender, RoutedEventArgs e)
        {
            //log.Debug(email.Text);
            // log.Debug(email.Text.Length<=0) ;
            Login();



        }
        private void Login()
        {
            if (email.Text.Length <= 0 || pwd.Password.Length <= 0)
            {

                Main.main.InfoDialogShow("取消登录操作", "邮箱或密码未输入");

            }
            else if (!Regex.IsMatch(email.Text, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {

                Main.main.InfoDialogShow("取消登录操作", "邮箱格式错误");
            }
            else
            {
                host.IsOpen = true;
                Thread thread = new Thread(LoginThread);

                thread.Start();
            }

        }
        private void Button_KeyUpEnter(object sender, KeyEventArgs e)//回车登录
        {

        }
    }
}
