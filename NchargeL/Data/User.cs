using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using log4net;
using NCLCore;
using Newtonsoft.Json.Linq;
using Notification.Wpf;

namespace NchargeL;

public class User
{
    private static readonly ILog log = LogManager.GetLogger("User");

    private readonly NotificationManager notificationManager = new();
    public string _email;
    public string _name;
    public string? _password;

    public string _token;

    public string _useruuid;
    //private ImageSourse 

    public byte[]? image;

    public User()
    {
    }

    public User(JObject json, string email, string password)
    {
        _name = (string) json["selectedProfile"]["name"];
        _email = email;
        _password = password;
        _token = json["accessToken"].ToString();
        _useruuid = json["selectedProfile"]["id"].ToString();
        Main.main.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            (ThreadStart) delegate { Main.main.hello.Text = _name; });
        //Home.home.hello.Text = "您好" +_name + "\n欢迎游玩本服务器"; ;
        //log.Debug(json.ToString());
        var re = HttpRequestHelper.GetResponseString(HttpRequestHelper.CreatePostHttpResponse(
            "https://www.ncserver.top:666/api/yggdrasil/sessionserver/session/minecraft/profile/"
            + _useruuid,
            new Dictionary<string, string>()));
        var jObject = JObject.Parse(re);
        log.Debug(jObject.ToString());
        try
        {
            //log.Debug(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(jObject["properties"][0]["value"].ToString())));
            var selectedProfileJson =
                JObject.Parse(
                    Encoding.UTF8.GetString(
                        Convert.FromBase64String(jObject["properties"][0]["value"].ToString())));
            log.Debug(selectedProfileJson.ToString());
            if (selectedProfileJson["textures"]["SKIN"] != null)
            {
                var downImage = Image.FromStream(HttpRequestHelper
                    .CreatePostHttpResponse(selectedProfileJson["textures"]["SKIN"]["url"].ToString(),
                        new Dictionary<string, string>()).GetResponseStream());
                downImage.Save(Environment.CurrentDirectory + "\\all.png");
                //从网络获取的皮肤图像
                Bitmap head;
                Bitmap cover;
                // _color = Color.Black;
                {
                    var bmpDest = new Bitmap(8, 8, PixelFormat.Format32bppRgb); //目标图片大小

                    var g = Graphics.FromImage(bmpDest); //创建GDI

                    var rectDest = new Rectangle(0, 0, 8, 8); //输出的图片开始位置及大小 XY
                    var rectSource = new Rectangle(8, 8, 8, 8); //要截取的图片开始的位置，及大小

                    g.DrawImage(downImage, rectDest, rectSource, GraphicsUnit.Pixel);
                    head = bmpDest;
                    head = KiResizeImage(head, 104, 104);
                    head.Save(Environment.CurrentDirectory + "\\1.png");
                    //head.Dispose();
                } //获取头部的图像
                {
                    var bmpDest = new Bitmap(8, 8, PixelFormat.Format32bppRgb); //目标图片大小

                    var g = Graphics.FromImage(bmpDest); //创建GDI

                    var rectDest = new Rectangle(0, 0, 8, 8); //目标位置
                    var rectSource = new Rectangle(40, 8, 8, 8); //原图片

                    g.DrawImage(downImage, rectDest, rectSource, GraphicsUnit.Pixel);
                    for (var i = 0; i < 8; i++)
                    for (var j = 0; j < 8; j++)
                        if (bmpDest.GetPixel(i, j) == Color.FromArgb(255, 0, 0, 0))
                            bmpDest.SetPixel(i, j, Color.Transparent);
                    // log.Debug(bmpDest.GetPixel(i, j) == Color.FromArgb(255,0,0,0));

                    cover = bmpDest;
                    cover = KiResizeImage(cover, 128, 128);
                    //Home.home.userCoverImage.Source = ConvertBitmapToBitmapImage(bmpDest);
                    cover.Save(Environment.CurrentDirectory + "\\2.png");
                    //cover.Dispose();
                } //获取头部覆盖层的图像

                var bithead = new Bitmap(114, 114, PixelFormat.Format32bppRgb);
                var ghead = Graphics.FromImage(bithead);
                ghead.DrawImage(cover, 0, 0);

                for (var i = 8; i < 111; i++)
                for (var j = 8; j < 111; j++)
                    //log.Debug(cover.GetPixel(i, j));
                    if (cover.GetPixel(i, j) ==
                        ColorTranslator.FromHtml("#ffffff")) // Color.FromArgb(255, 255, 255, 255)白色
                        bithead.SetPixel(i, j, head.GetPixel(i - 8, j - 8));

                for (var i = 0; i < bithead.Height; i++)
                for (var j = 0; j < bithead.Width; j++)
                    if (bithead.GetPixel(i, j) == ColorTranslator.FromHtml("#ffffff") ||
                        bithead.GetPixel(i, j) == ColorTranslator.FromHtml("#000000"))
                        bithead.SetPixel(i, j, Color.Transparent);

                bithead.MakeTransparent(Color.Transparent);
                bithead.Save(Environment.CurrentDirectory + "\\3.png");
                var ms = new MemoryStream();
                bithead.Save(ms, ImageFormat.Png);
                image = ms.GetBuffer(); //转写为Byte[]方便保存
                ms.Close();
                Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    Main.main.userImage.Source = ConvertBitmapToBitmapImage(bithead);

                    notificationManager.Show(NotificationContentSDK.notificationSuccess("头像获取成功", ""),
                        "WindowArea");
                })).Wait();
                //Home.home.userImage.Source = ConvertBitmapToBitmapImage(bithead);
                cover.Dispose();
                head.Dispose();
                bithead.Dispose();
                //log.Debug(selectedProfileJson["textures"]["SKIN"]["url"].ToString());
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

    //private Color _color;
    public void reloadUser()
    {
        Main.main.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate
        {
            Main.main.hello.Text = _name;
            if (image != null)
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(image);
                bmp.EndInit();

                Main.main.userImage.Source = bmp;
            }
        });
    }

    public BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
    {
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        //stream.Close();
        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = stream;
        image.EndInit();

        return image;
    }


    public static Bitmap KiResizeImage(Bitmap bmp, int newW, int newH)
    {
        try
        {
            var b = new Bitmap(newW, newH);
            var g = Graphics.FromImage(b);

            // 插值算法的质量
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height),
                GraphicsUnit.Pixel);
            g.Dispose();

            return b;
        }
        catch
        {
            return null;
        }
    }
}