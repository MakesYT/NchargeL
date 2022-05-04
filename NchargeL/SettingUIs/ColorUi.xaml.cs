using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NchargeL.Properties;
using Notification.Wpf;

namespace NchargeL.SettingUIs;

/// <summary>
///     Color.xaml 的交互逻辑
/// </summary>
public partial class ColorUi : Page
{
    private static NotificationManager notificationManager = new();
    public static ColorUi colorUi;

    public List<PickerColor> colList = new()
    {
        new PickerColor {Id = 0, Name = "      主题颜色       ", Color = Settings.Default.BodyColorS},
        new PickerColor {Id = 1, Name = "      字体颜色       ", Color = Settings.Default.TextColor},
        new PickerColor {Id = 2, Name = "      背景颜色       ", Color = Settings.Default.BackgroundColor},
        new PickerColor {Id = 3, Name = "      前景颜色       ", Color = Settings.Default.ForegroundColor},
        new PickerColor {Id = 4, Name = "    提示成功颜色     ", Color = Settings.Default.NotificationSuccess},
        new PickerColor {Id = 5, Name = "    提示警告颜色     ", Color = Settings.Default.NotificationWarning},
        new PickerColor {Id = 6, Name = "    提示失败颜色     ", Color = Settings.Default.NotificationError}
    };

    public ColorUi()
    {
        colorUi = this;
        InitializeComponent();
        list.ItemsSource = colList; //数据源
        list.DisplayMemberPath = "Name"; //路径
        //为TextBox设置Binding
        list.SelectedIndex = 0;
        //list.ItemsSource=Update;
    }


    private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
    {
    }

    public class PickerColor : INotifyPropertyChanged

    {
        public Color _Color;

        public Color Color
        {
            get => _Color;

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
                            Settings.Default.BodyColorS = value;
                            break;
                        }
                        case 1:
                        {
                            Settings.Default.TextColor = value;
                            break;
                        }
                        case 2:
                        {
                            Settings.Default.BackgroundColor = value;
                            break;
                        }
                        case 3:
                        {
                            Settings.Default.ForegroundColor = value;
                            break;
                        }
                        case 4:
                        {
                            Settings.Default.NotificationSuccess = value;
                            break;
                        }
                        case 5:
                        {
                            Settings.Default.NotificationWarning = value;
                            break;
                        }
                        case 6:
                        {
                            Settings.Default.NotificationError = value;
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
}