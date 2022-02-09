using Notification.Wpf;
using System.Windows.Media;

namespace NchargeL.Info
{
    public class NotificationContentSDK
    {
        public static NotificationContent notificationSuccess(string title, string Msg)
        {
            var content = new NotificationContent
            {
                Title = title,
                Message = Msg,
                Type = NotificationType.Success,

                Background = (Brush)App.Current.FindResource("NotificationSuccess"),
                Foreground = (Brush)App.Current.FindResource("ForegroundColor"),
            };
            return content;
        }
        public static NotificationContent notificationInformation(string title, string Msg)
        {
            var content = new NotificationContent
            {
                Title = title,
                Message = Msg,
                Type = NotificationType.Information,

                Background = (Brush)App.Current.FindResource("BodyColor"),
                Foreground = (Brush)App.Current.FindResource("ForegroundColor"),
            };
            return content;
        }
        public static NotificationContent notificationError(string title, string Msg)
        {
            var content = new NotificationContent
            {
                Title = title,
                Message = Msg,
                Type = NotificationType.Error,

                Background = (Brush)App.Current.FindResource("NotificationError"),
                Foreground = (Brush)App.Current.FindResource("ForegroundColor"),
            };
            return content;
        }
        public static NotificationContent notificationWarning(string title, string Msg)
        {
            var content = new NotificationContent
            {
                Title = title,
                Message = Msg,
                Type = NotificationType.Warning,

                Background = (Brush)App.Current.FindResource("NotificationWarning"),
                Foreground = (Brush)App.Current.FindResource("ForegroundColor"),
            };
            return content;
        }
    }
}
