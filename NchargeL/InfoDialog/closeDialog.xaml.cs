﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;
using log4net;
using NchargeL.Properties;

namespace NchargeL;

/// <summary>
///     InfoDialog.xaml 的交互逻辑
/// </summary>
public partial class closeDialog : Window
{
    private static readonly ILog log = LogManager.GetLogger("closeDialog");

    public closeDialog()
    {
        InitializeComponent();

        var storyboard = (Storyboard) FindResource("Storyboard1");
        storyboard.Begin();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Settings.Default.Save();

        //Settings.Default.Properties.Clear();
        StopProcess("wget.exe");
        Close();
        Main.main.Close();
        // System.Environment.Exit(0);
        //  System.Threading.ThreadPool.
        Process.GetCurrentProcess().Kill();
        Application.Current.Shutdown();
        Environment.Exit(0);
    }

    public static void StopProcess(string processName)
    {
        try
        {
            var processes = Process.GetProcesses();
            foreach (var item in processes)
                if (item.ProcessName.Contains(processName.Replace(".exe", "")))
                {
                    item.Kill();
                    log.Info("已杀死" + item.Id);
                    // break;
                }
        }
        catch
        {
        }
    }
}