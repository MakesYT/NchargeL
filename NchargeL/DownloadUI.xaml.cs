using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using log4net;
using Microsoft.WindowsAPICodePack.Dialogs;
using NchargeL.Properties;
using NCLCore;
using Newtonsoft.Json.Linq;
using Notification.Wpf;

namespace NchargeL
{
    /// <summary>
    /// DownloadUI.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadUI : Page
    {
        private static readonly ILog log = LogManager.GetLogger("DownloadUI");
        NotificationManager notificationManager = new NotificationManager();

        public DownloadUI()
        {
            InitializeComponent();
            // logs.UndoLimit = 0;
            GetAllCients();
            string cpu = "";
            UInt64 rom = 0;
            string GPU = "";
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                cpu = (string) mo.Properties["Name"].Value;
            }

            ManagementClass mc1 = new ManagementClass("Win32_PhysicalMemory");
            ManagementObjectCollection moc1 = mc1.GetInstances();
            foreach (ManagementObject mo1 in moc1)
            {
                rom += (UInt64) mo1.Properties["Capacity"].Value / 1024 / 1024;
            }

            ManagementClass m = new ManagementClass("Win32_VideoController");
            ManagementObjectCollection mn = m.GetInstances();
            ManagementObjectSearcher
                mos = new ManagementObjectSearcher("Select * from Win32_VideoController"); //Win32_VideoController 显卡
            int count = 0;
            foreach (ManagementObject mo in mos.Get())
            {
                count++;
                GPU += mo["Name"].ToString() + "\n";
                // DisplayName += "第" + count.ToString() + "张显卡名称：" + mo["Name"].ToString() + "   ";
                if (count == 2) break;
            }

            systeminfo.Text = "系统信息:\nCPU:" + cpu + "\nGPU:" + GPU + "物理内存大小:" + rom + "MB";
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int line = 0;
            if (Settings.Default.GameDir != "")
            {
                ClientDownload clientDownload = new ClientDownload(Main.main.infoManager);
                Main.main.infoManager.clear();

                Main.main.infoManager.PropertyChanged += (oo, ee) =>
                {
                    Info logtmp = (oo as InfoManager).info;

                    Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        line++;
                        if (line > 5)
                        {
                            line = 0;
                            logs.Text = "";
                        }

                        log.Info("消息通知记录:" + logtmp.msg);
                        logs.Text += logtmp.msg + "\n";
                        //logs.Select(logs.Text.Length, 0);
                        // logs.ScrollToEnd();
                    })).Wait();
                };
                if (list.SelectedItem != null)
                {
                    NchargeClient nchargeClient = (NchargeClient) list.SelectedItem;

                    Task.Factory.StartNew(() =>
                        clientDownload.DownloadNchargeClient(nchargeClient, Settings.Default.DownloadSource,
                            Settings.Default.GameDir));
                }
                else notificationManager.Show(NotificationContentSDK.notificationError("请先选择客户端", ""), "WindowArea");
            }
            else
            {
                var dlg = new CommonOpenFileDialog();
                dlg.IsFolderPicker = true;
                //dlg.InitialDirectory = currentDirectory;
                dlg.Title = "选择\".minecraft\"游戏目录";
                while (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (dlg.FileName.EndsWith(".minecraft"))
                    {
                        Settings.Default.GameDir = dlg.FileName;
                        //NCLcore nCLCore = Main.main.newNCLcore(Properties.Settings.Default.DownloadSource, Properties.Settings.Default.GameDir);
                        ClientDownload clientDownload = new ClientDownload(Main.main.infoManager);
                        //clientDownload.init();
                        //int line = 0;
                        Main.main.infoManager.clear();
                        Main.main.infoManager.PropertyChanged += (oo, ee) =>
                        {
                            Info logtmp = (oo as InfoManager).info;
                            Application.Current.Dispatcher.BeginInvoke(new Action(delegate
                            {
                                line++;
                                if (line > 5)
                                {
                                    line = 0;
                                    logs.Text = "";
                                }

                                log.Info("消息通知记录:" + logtmp.msg);
                                logs.Text += logtmp.msg + "\n";
                                //logs.Select(logs.Text.Length, 0);
                                // logs.ScrollToEnd();
                            })).Wait();
                        };
                        if (list.SelectedItem != null)
                        {
                            NchargeClient nchargeClient = (NchargeClient) list.SelectedItem;

                            Task.Factory.StartNew(() =>
                                clientDownload.DownloadNchargeClient(nchargeClient, Settings.Default.DownloadSource,
                                    Settings.Default.GameDir));
                        }
                        else
                            notificationManager.Show(NotificationContentSDK.notificationError("请先选择客户端", ""),
                                "WindowArea");

                        //ForgeInstaller.installForge(Properties.Settings.Default.DownloadSource, "forge", Properties.Settings.Default.GameDir, "1.12.2-14.23.5.2860")

                        break;
                    }
                    else
                    {
                        InfoDialog info = new InfoDialog("选择游戏目录", "您需要选择以.minecraft命名的文件夹");
                        info.ShowDialog();
                    }
                }
            }
        }

        void GetAllCients()
        {
            string re1 = HttpRequestHelper.GetResponseString(
                HttpRequestHelper.CreatePostHttpResponse("http://download.ncserver.top:8000/NCL/clients.json",
                    new Dictionary<String, String>()));
            var jObject = JArray.Parse(re1);
            List<NchargeClient> nchargeClients = new List<NchargeClient>();
            foreach (JObject clientJson in jObject)
            {
                NchargeClient client = new NchargeClient();
                client.name = clientJson["name"].ToString();
                client.time = clientJson["time"].ToString();
                client.Cname = clientJson["Cname"].ToString();
                client.modsize = clientJson["modsize"].ToString();
                client.version = clientJson["version"].ToString();
                client.NchargeVersion = clientJson["NchargeVersion"].ToString();
                client.forgeVersion = clientJson["forgeVersion"].ToString();
                client.mods = clientJson["modscount"].ToString();
                nchargeClients.Add(client);
            }

            list.ItemsSource = nchargeClients;
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedIndex != -1)
            {
                NchargeClient nchargeClient = ((NchargeClient) ((DataGrid) sender).SelectedItem);
                info.Text = "客户端 : " + nchargeClient.name + "(" + nchargeClient.Cname + ")\n" +
                            "MOD总数 : " + nchargeClient.mods + "个\n" +
                            "MOD总大小 : " + nchargeClient.modsize + "\n" +
                            "整合包开启时间 : \n" + nchargeClient.time;
            }
            else info.Text = "当前未选择客户端";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetAllCients();
        }
    }
}