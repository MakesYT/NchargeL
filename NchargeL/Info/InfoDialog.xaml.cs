using System.Windows;
using System.Windows.Media.Animation;

namespace NchargeL
{
    /// <summary>
    /// InfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InfoDialog : Window
    {
        public InfoDialog(string infostr, string str)
        {
            InitializeComponent();
            info.Text = infostr;
            text.Text = str;
            var storyboard = (Storyboard)this.FindResource("Storyboard1");
            storyboard.Begin();

        }

    }
}
