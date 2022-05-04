using System.Windows;
using System.Windows.Media;
using Notification.Wpf;

namespace NchargeL;

public class NotificationContentSDK
{
    public static NotificationContent notificationSuccess(string title, string Msg)
    {
        var content = new NotificationContent
        {
            Title = title,
            Message = Msg,
            Type = NotificationType.Success,

            Background = (Brush) Application.Current.FindResource("NotificationSuccess"),
            Foreground = (Brush) Application.Current.FindResource("ForegroundColor")
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

            Background = (Brush) Application.Current.FindResource("BodyColor"),
            Foreground = (Brush) Application.Current.FindResource("ForegroundColor")
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

            Background = (Brush) Application.Current.FindResource("NotificationError"),
            Foreground = (Brush) Application.Current.FindResource("ForegroundColor")
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

            Background = (Brush) Application.Current.FindResource("NotificationWarning"),
            Foreground = (Brush) Application.Current.FindResource("ForegroundColor")
        };
        return content;
    }
}