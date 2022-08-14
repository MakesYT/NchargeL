using Microsoft.WindowsAPICodePack.Dialogs;
using NchargeL.Properties;
using NCLCore;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NchargeL
{
    /// <summary>
    /// LeadingUi.xaml 的交互逻辑
    /// </summary>
    public partial class LeadingUi : Window
    {
        public LeadingUi()
        {
            InitializeComponent();
            Java.Content = ClientTools.JavaVer(Settings.Default.Java );
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            var main = new Main();
            main.Show();
            Close();
        }
        private void GameDir(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            //dlg.InitialDirectory = currentDirectory;
            dlg.Title = "选择\".minecraft\"游戏目录";
            while (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                if (dlg.FileName.EndsWith(".minecraft"))
                {
                    Settings.Default.GameDir = dlg.FileName;
                    
                    break;
                }
                else
                {
                    var warn = new InfoDialog("", "您选择的目录不是以\".minecraft\"结尾的\n是否在此目录下自动创建\".minecraft\"文件夹",
                              false);
                    warn.ShowDialog();
                    if (!(warn.cancelfg))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(dlg.FileName + "\\.minecraft");
                        if (!directoryInfo.Exists)
                            directoryInfo.Create();
                        Settings.Default.GameDir = dlg.FileName + "\\.minecraft";

                        break;
                    }
                    
                }
        }
        private void JavaDir(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Filters.Add(new CommonFileDialogFilter("javaw.exe", "exe"));
            dlg.IsFolderPicker = false;
            //dlg.InitialDirectory = currentDirectory;
            dlg.Title = "选择\"javaw.exe\"";

            while (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                if (dlg.FileName.EndsWith("\\javaw.exe"))
                {
                    Settings.Default.Java = dlg.FileName;

                    break;
                }
                else
                {
                    var info = new InfoDialog("选择了" + dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1),
                        "您需要选择以javaw.exe命名的文件");
                    info.ShowDialog();
                }
            Java.Content = ClientTools.JavaVer(Settings.Default.Java);
        }

        private void limitnumber(object sender, TextCompositionEventArgs e)
        {
            var re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }
    }
}
