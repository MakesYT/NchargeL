using System.Windows.Controls;

namespace NchargeL
{
    /// <summary>
    /// Account.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class Account : Page
    {
        public static Account account;

        public Account()
        {
            InitializeComponent();
            account = this;
        }
    }
}