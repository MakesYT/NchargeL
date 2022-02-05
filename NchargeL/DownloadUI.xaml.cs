using NCLCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static NCLCore.SDK;

namespace NchargeL
{
    /// <summary>
    /// DownloadUI.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadUI : Page
    {
        public DownloadUI()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NCLcore nCLCore = new NCLcore(Properties.Settings.Default.DownloadSource, Properties.Settings.Default.GameDir);
            nCLCore.clientDownload.DownloadOfficialClient("1.16.5");
        }
    }
}
