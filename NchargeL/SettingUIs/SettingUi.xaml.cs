using System.Windows;
using System.Windows.Controls;
using NchargeL.SettingUIs;

namespace NchargeL
{
    /// <summary>
    /// SettingUi.xaml 的交互逻辑
    /// </summary>
    public partial class SettingUi : Page
    {
        ColorUi colorUi = new ColorUi();

        public SettingUi()
        {
            InitializeComponent();
            FrameWork.Content = colorUi;
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
        }
    }
}