using log4net;
using NchargeL.Info;
using NCLCore;
using Newtonsoft.Json.Linq;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace NchargeL
{
    public class User
    {

        private static readonly ILog log = LogManager.GetLogger("User");
        public string _name;
        public string _email;
        public string? _password;
        public string _useruuid;
        public string _token;
        //private ImageSourse 

        public byte[] image;
        NotificationManager notificationManager = new NotificationManager();
        //private Color _color;
        public void reloadUser()
        {
            Main.main.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {

                Main.main.hello.Text = _name;
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(image);
                bmp.EndInit();
                Main.main.userImage.Source = bmp;
            });
        }
        public User()
        {

        }
        public BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            //stream.Close();
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;
        }


        public static Bitmap KiResizeImage(Bitmap bmp, int newW, int newH)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);

                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.NearestNeighbor;

                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();

                return b;
            }
            catch
            {
                return null;
            }
        }
        public User(JObject json, string email, string password)
        {

            _name = (string)json["selectedProfile"]["name"];
            _email = email;
            _password = password;
            _token = json["accessToken"].ToString();
            _useruuid = json["selectedProfile"]["id"].ToString();
            Main.main.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {

                Main.main.hello.Text = _name;
            });
            //Home.home.hello.Text = "您好" +_name + "\n欢迎游玩本服务器"; ;
            //log.Debug(json.ToString());
            string re = HttpRequestHelper.GetResponseString(HttpRequestHelper.CreatePostHttpResponse("https://www.ncserver.top:666/api/yggdrasil/sessionserver/session/minecraft/profile/"
                + _useruuid,
                new Dictionary<String, String>()));
            var jObject = JObject.Parse(re);
            log.Debug(jObject.ToString());
            try
            {
                //log.Debug(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(jObject["properties"][0]["value"].ToString())));
                var selectedProfileJson = JObject.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(jObject["properties"][0]["value"].ToString())));
                log.Debug(selectedProfileJson.ToString());
                if (selectedProfileJson["textures"]["SKIN"] != null)
                {


                    System.Drawing.Image downImage = System.Drawing.Image.FromStream(HttpRequestHelper.CreatePostHttpResponse(selectedProfileJson["textures"]["SKIN"]["url"].ToString(), new Dictionary<String, String>()).GetResponseStream());
                    downImage.Save(Environment.CurrentDirectory + "\\all.png");
                    //从网络获取的皮肤图像
                    Bitmap head;
                    Bitmap cover;
                    // _color = Color.Black;
                    {
                        Bitmap bmpDest = new Bitmap(8, 8, PixelFormat.Format32bppRgb);       //目标图片大小

                        Graphics g = Graphics.FromImage(bmpDest);               //创建GDI

                        Rectangle rectDest = new Rectangle(0, 0, 8, 8);//输出的图片开始位置及大小 XY
                        Rectangle rectSource = new Rectangle(8, 8, 8, 8);//要截取的图片开始的位置，及大小

                        g.DrawImage(downImage, rectDest, rectSource, GraphicsUnit.Pixel);
                        head = bmpDest;
                        head = KiResizeImage(head, 112, 112);
                        head.Save(Environment.CurrentDirectory + "\\1.png");
                        //head.Dispose();
                    }//获取头部的图像
                    {
                        Bitmap bmpDest = new Bitmap(8, 8, PixelFormat.Format32bppRgb);       //目标图片大小

                        Graphics g = Graphics.FromImage(bmpDest);               //创建GDI

                        Rectangle rectDest = new Rectangle(0, 0, 8, 8);//目标位置
                        Rectangle rectSource = new Rectangle(40, 8, 8, 8);//原图片

                        g.DrawImage(downImage, rectDest, rectSource, GraphicsUnit.Pixel);
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if (bmpDest.GetPixel(i, j) == Color.FromArgb(255, 0, 0, 0))
                                {
                                    bmpDest.SetPixel(i, j, Color.Transparent);
                                }
                                // log.Debug(bmpDest.GetPixel(i, j) == Color.FromArgb(255,0,0,0));
                            }
                        }
                        cover = bmpDest;
                        cover = KiResizeImage(cover, 128, 128);
                        //Home.home.userCoverImage.Source = ConvertBitmapToBitmapImage(bmpDest);
                        cover.Save(Environment.CurrentDirectory + "\\2.png");
                        //cover.Dispose();
                    }//获取头部覆盖层的图像

                    Bitmap bithead = new Bitmap(114, 114, PixelFormat.Format32bppRgb);
                    Graphics ghead = Graphics.FromImage(bithead);
                    ghead.DrawImage((Image)cover, 0, 0);

                    for (int i = 4; i < 114; i++)
                    {
                        for (int j = 4; j < 114; j++)
                        {
                            //log.Debug(cover.GetPixel(i, j));
                            if (cover.GetPixel(i, j) == ColorTranslator.FromHtml("#ffffff"))// Color.FromArgb(255, 255, 255, 255)白色
                            {
                                bithead.SetPixel(i, j, head.GetPixel(i - 4, j - 4));
                            }
                        }
                    }//将覆盖层的空白颜色替换为头像的图像
                    for (int i = 0; i < bithead.Height; i++)
                    {
                        for (int j = 0; j < bithead.Width; j++)
                            if (bithead.GetPixel(i, j) == ColorTranslator.FromHtml("#ffffff") || bithead.GetPixel(i, j) == ColorTranslator.FromHtml("#000000"))
                            {
                                bithead.SetPixel(i, j, Color.Transparent);
                            }
                    }
                    bithead.MakeTransparent(Color.Transparent);
                    bithead.Save(Environment.CurrentDirectory + "\\3.png");
                    MemoryStream ms = new MemoryStream();
                    bithead.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    image = ms.GetBuffer();//转写为Byte[]方便保存
                    ms.Close();
                    Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        Main.main.userImage.Source = ConvertBitmapToBitmapImage(bithead);

                        notificationManager.Show(NotificationContentSDK.notificationSuccess("头像获取成功", ""), "WindowArea");
                    })).Wait();
                    //Home.home.userImage.Source = ConvertBitmapToBitmapImage(bithead);
                    cover.Dispose();
                    head.Dispose();
                    bithead.Dispose();
                    //log.Debug(selectedProfileJson["textures"]["SKIN"]["url"].ToString());
                }
                else
                {
                    // Main.main.notifier.ShowInformation("未设置皮肤,无法获取头像");
                    notificationManager.Show(NotificationContentSDK.notificationWarning("未设置皮肤,无法获取头像", ""), "WindowArea");
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {

                    notificationManager.Show(NotificationContentSDK.notificationError("头像获取失败", ""), "WindowArea");
                }));
                log.Error(ex.ToString());
            }

            // Convert.FromBase64String(jObject["properties"][0]["value"].ToString());


        }
    }
}
