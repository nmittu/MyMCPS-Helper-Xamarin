using Xamarin.Forms;

namespace MyMCPSHelper
{
    public partial class App : Application
    {
        public static AccountManager AccMangr = new AccountManager();

        public static string Name = "com.mittudev.mymcps_helper.MyMCPS-Helper";

        public App()
        {
            InitializeComponent();

            MainPage = new LoginPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            AccMangr.logout();
        }

        protected override async void OnResume()
        {
            var nav = MainPage.Navigation;
            await nav.PushModalAsync(new LoginPage(true));
        }
    }
}
