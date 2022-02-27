using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace NchargeL
{
    /// <summary>
    /// InfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class closeDialog : Window
    {
        public closeDialog()
        {
            InitializeComponent();

            var storyboard = (Storyboard)this.FindResource("Storyboard1");
            storyboard.Begin();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            this.Close();
            Main.main.Close();
            Application.Current.Shutdown();
            Environment.Exit(0);
        }
    }
}
