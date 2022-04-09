using System;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using log4net;

namespace NchargeL
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger("App");

        protected override void OnStartup(StartupEventArgs e)
        {
            ServicePointManager.DefaultConnectionLimit = 10240;
            base.OnStartup(e);
            //UI线程的异常捕捉

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }


        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Exprint(e);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Current.Dispatcher.BeginInvoke(new Action(delegate
            // {


            ErrorDialog error = new ErrorDialog("", "（1）发生了一个错误！请联系腐竹！" + Environment.NewLine
                                                                        + e.ExceptionObject.ToString());
            error.ShowDialog();
            // }));
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Exprint(e);
        }

        private void Exprint(DispatcherUnhandledExceptionEventArgs e)
        {
            Current.Dispatcher.BeginInvoke(new Action(delegate
            {
                try
                {
                    e.Handled = true;
                    if (e.Exception.InnerException == null)
                    {
                        log.Error("（1）发生了一个错误！请联系腐竹！" + Environment.NewLine
                                                      + "（2）错误源：" + e.Exception.Source + Environment.NewLine
                                                      + "（3）详细信息：" + e.Exception.Message + Environment.NewLine);
                        ErrorDialog error = new ErrorDialog("", "（1）发生了一个错误！请联系腐竹！" + Environment.NewLine
                            + "（2）错误源：" + e.Exception.Source + Environment.NewLine
                            + "（3）详细信息：" + e.Exception.Message + Environment.NewLine);
                        error.ShowDialog();

                        //MessageBox.Show("（1）发生了一个错误！请联系腐竹！" + Environment.NewLine
                        //                   + "（2）错误源：" + e.Exception.Source + Environment.NewLine
                        //                   + "（3）详细信息：" + e.Exception.Message + Environment.NewLine);
                        //+ "（4）报错区域：" + e.Exception.StackTrace);
                    }
                    else
                    {
                        log.Error("（1）发生了一个错误！请联系腐竹！" + Environment.NewLine
                                                      + "（2）错误源：" + e.Exception.InnerException.Source +
                                                      Environment.NewLine
                                                      + "（3）错误信息：" + e.Exception.Message + Environment.NewLine
                                                      + "（4）详细信息：" + e.Exception.InnerException.Message +
                                                      Environment.NewLine
                                                      + "（5）报错区域：" + e.Exception.InnerException.StackTrace);
                        ErrorDialog error = new ErrorDialog("", "（1）发生了一个错误！请联系腐竹！" + Environment.NewLine
                            + "（2）错误源：" + e.Exception.InnerException.Source + Environment.NewLine
                            + "（3）错误信息：" + e.Exception.Message + Environment.NewLine
                            + "（4）详细信息：" + e.Exception.InnerException.Message + Environment.NewLine
                            + "（5）报错区域：" + e.Exception.InnerException.StackTrace);
                        error.ShowDialog();
                        //MessageBox.Show("（1）发生了一个错误！请联系腐竹！" + Environment.NewLine
                        //                    + "（2）错误源：" + e.Exception.InnerException.Source + Environment.NewLine
                        //                    + "（3）错误信息：" + e.Exception.Message + Environment.NewLine
                        //                    + "（4）详细信息：" + e.Exception.InnerException.Message + Environment.NewLine
                        //                    + "（5）报错区域：" + e.Exception.InnerException.StackTrace);
                    }
                }
                catch (Exception e2)
                {
                    //此时程序出现严重异常，将强制结束退出
                    ErrorDialog error = new ErrorDialog("", "程序发生致命错误，将终止，请联系腐竹！");
                    error.ShowDialog();
                    //MessageBox.Show("程序发生致命错误，将终止，请联系腐竹！");
                }
            })).Wait();
        }
    }
}