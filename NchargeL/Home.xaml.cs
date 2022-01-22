using log4net;
using Notification.Wpf;
using System.Windows.Controls;


namespace NchargeL
{
    /// <summary>
    /// Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home : Page
    {
        public NotificationManager notificationManager = new NotificationManager();
        private static readonly ILog log = LogManager.GetLogger("Home");
        public static Home home;

        public Home()
        {
            home = this;

            InitializeComponent();

            // ImageBrush imageBrush = new ImageBrush();
            //imageBrush.ImageSource = new BitmapImage(new Uri("C:\\ProgramData\\Microsoft\\User Account Pictures\\user.png", UriKind.Relative));
            //userImage.Source = new BitmapImage(new Uri("C:\\ProgramData\\Microsoft\\User Account Pictures\\user.png"));
        }



    }
}
