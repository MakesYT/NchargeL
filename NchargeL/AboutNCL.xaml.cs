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

namespace NchargeL
{
    /// <summary>
    /// AboutNCL.xaml 的交互逻辑
    /// </summary>
    public partial class AboutNCL : Page
    {
        public AboutNCL()
        {
            InitializeComponent();
        }

		private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void java_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void Button_Click_61(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
		}
	}
}
