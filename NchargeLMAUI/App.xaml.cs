namespace NchargeLMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Routing.RegisterRoute("settings", typeof(views.SettingsPage));
            
            //MainPage = new MainPage();
        }
        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            Shell.Current.GoToAsync("///settings");
        }
    }
}