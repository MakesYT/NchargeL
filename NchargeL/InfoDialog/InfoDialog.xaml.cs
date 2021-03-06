using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

namespace NchargeL;

/// <summary>
///     InfoDialog.xaml 的交互逻辑
/// </summary>
public partial class InfoDialog : Window
{
    private readonly int time;
    public bool cancelfg;

    public InfoDialog(string infostr, string str, bool cancelfg)
    {
        InitializeComponent();
        info.Text = infostr;
        text.Text = str;
        var storyboard = (Storyboard) FindResource("Storyboard1");
        storyboard.Begin();
        if (time != 0)
        {
            close.IsEnabled = false;
            close.Content = "关闭(" + time + ")";
            var thread2 = new Thread(timeLaterColse);
            thread2.Name = "Test2";
            thread2.Start();
        }

        cancel.Visibility = Visibility.Visible;
    }

    public InfoDialog(string infostr, string str)
    {
        InitializeComponent();
        info.Text = infostr;
        text.Text = str;
        var storyboard = (Storyboard) FindResource("Storyboard1");
        storyboard.Begin();
        if (time != 0)
        {
            close.IsEnabled = false;
            close.Content = "关闭(" + time + ")";
            var thread2 = new Thread(timeLaterColse);
            thread2.Name = "Test2";
            thread2.Start();
        }
    }

    public InfoDialog(string infostr, string str, int time)
    {
        InitializeComponent();
        info.Text = infostr;
        text.Text = str;
        var storyboard = (Storyboard) FindResource("Storyboard1");
        storyboard.Begin();
        this.time = time;
        if (time != 0)
        {
            close.IsEnabled = false;
            close.Content = "关闭(" + time + ")";
            var thread2 = new Thread(timeLaterColse);
            thread2.Name = "Test2";
            thread2.Start();
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    public void timeLaterColse()
    {
        for (var i = 0; i < time; i++)
        {
            Thread.Sleep(1200);
            Application.Current.Dispatcher
                .BeginInvoke(new Action(delegate { close.Content = "关闭(" + (time - i) + ")"; })).Wait();
        }

        Application.Current.Dispatcher.BeginInvoke(new Action(delegate
        {
            close.Content = "关闭";
            close.IsEnabled = true;
        }));
    }

    private void Cancel_OnClick_Click(object sender, RoutedEventArgs e)
    {
        cancelfg = true;
        Close();
    }
}