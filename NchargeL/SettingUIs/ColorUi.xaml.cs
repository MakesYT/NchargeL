using Notification.Wpf;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NchargeL.SettingUIs
{
    /// <summary>
    /// Color.xaml 的交互逻辑
    /// </summary>
    public partial class ColorUi : Page
    {
        static NotificationManager notificationManager = new NotificationManager();
        public static ColorUi colorUi;
        public List<PickerColor> colList = new List<PickerColor>()
            {
                new PickerColor() { Id = 0, Name = "      主题颜色       ", Color = Properties.Settings.Default.BodyColorS },
                new PickerColor() { Id = 1, Name = "      字体颜色       ",Color = Properties.Settings.Default.TextColor},
                new PickerColor() { Id = 2, Name = "      背景颜色       ",Color = Properties.Settings.Default.BackgroundColor},
                new PickerColor() { Id = 3, Name = "      前景颜色       ",Color = Properties.Settings.Default.ForegroundColor},
                new PickerColor() { Id = 4, Name = "    提示成功颜色     ",Color = Properties.Settings.Default.NotificationSuccess},
                new PickerColor() { Id = 5, Name = "    提示警告颜色     ",Color = Properties.Settings.Default.NotificationWarning},
                new PickerColor() { Id = 6, Name = "    提示失败颜色     ",Color = Properties.Settings.Default.NotificationError},
            };
        public ColorUi()
        {
            colorUi = this;
            InitializeComponent();
            this.list.ItemsSource = colList;//数据源
            this.list.DisplayMemberPath = "Name";//路径
                                                 //为TextBox设置Binding
            list.SelectedIndex = 0;
            //list.ItemsSource=Update;

        }

        public class PickerColor : INotifyPropertyChanged

        {
            public Color _Color;
            public Color Color
            {
                get { return _Color; }

                set

                {
                    _Color = value;

                    if (PropertyChanged != null)
                    {
                        // notificationManager.Show(NotificationContentSDK.notificationSuccess(value.ToString(), Name), "WindowArea");
                        PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Color"));
                        switch (Id)
                        {
                            case 0:
                                {
                                    Properties.Settings.Default.BodyColorS = value;
                                    break;
                                }
                            case 1:
                                {
                                    Properties.Settings.Default.TextColor = value;
                                    break;
                                }
                            case 2:
                                {
                                    Properties.Settings.Default.BackgroundColor = value;
                                    break;
                                }
                            case 3:
                                {
                                    Properties.Settings.Default.ForegroundColor = value;
                                    break;
                                }
                            case 4:
                                {
                                    Properties.Settings.Default.NotificationSuccess = value;
                                    break;
                                }
                            case 5:
                                {
                                    Properties.Settings.Default.NotificationWarning = value;
                                    break;
                                }
                            case 6:
                                {
                                    Properties.Settings.Default.NotificationError = value;
                                    break;
                                }
                        }

                    }

                }
            }
            public string Name { get; set; }

            public int Id { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
        }


        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
