using Xamarin.Forms;
using System.Linq;
using System;

namespace MyMCPSHelper
{
    public partial class App : Application
    {
        public static AccountManager AccMangr = new AccountManager();

        public static string Name = "com.mittudev.mymcps_helper";
        public static DateTime time;

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
            time = DateTime.Now;
        }

        protected override async void OnResume()
        {
            try{
                var nav = MainPage.Navigation;
                if (nav.ModalStack.LastOrDefault().GetType() != typeof(LoginPage) && DateTime.Now.Subtract(time).TotalMinutes > 2)
                {
                    AccMangr.logout();
                    await nav.PushModalAsync(new LoginPage(true));
                }
            }catch{}
        }
    }
}
