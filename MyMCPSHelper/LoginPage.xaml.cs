using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Xamarin.Forms;

namespace MyMCPSHelper {
    public partial class LoginPage : ContentPage {
        bool justPop = false;

        public LoginPage() : this(false) {}

        public LoginPage(bool pop){
            justPop = pop;

			InitializeComponent();

			Tuple<String, String> account = App.AccMangr.getAccount();
			if (account != null)
			{
				StudentId.Text = account.Item1;
				Password.Text = account.Item2;

                Login(LoginButton, null);
			}
        }

        void Login(object sender, System.EventArgs e) {
            spinner.IsVisible = true;
            spinner.IsRunning = true;
            ((Button)sender).IsEnabled = false;
            Task.Run(() => {
                String res = App.AccMangr.Login(StudentId.Text, Password.Text);
				if (res == "true"){
                    Device.BeginInvokeOnMainThread(() => {
                        spinner.IsRunning = false;
                        ((Button)sender).IsEnabled = true;
                        if (justPop){
                            Navigation.PopModalAsync();
                        }
                        else{
                            Navigation.PushModalAsync(new NavigationPageNoBack(new ClassesPage(), "Classes"));
                        }
                        App.AccMangr.saveAccount(StudentId.Text, Password.Text);
                    });
                }else if(res == "Multiple Accounts"){
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        spinner.IsRunning = false;
                        ((Button)sender).IsEnabled = true;
                        if (justPop){
                            Navigation.PopModalAsync();
                        }else{
                            Navigation.PushModalAsync(new NavigationPageNoBack(new StudentsPage(), "Students"));
                        }
                        App.AccMangr.saveAccount(StudentId.Text, Password.Text);
                    });
                }else{
                    Device.BeginInvokeOnMainThread(() => {
                        spinner.IsRunning = false;
                        ((Button) sender).IsEnabled = true;
                        DisplayAlert("Error", res, "Ok"); 
                    });
				}
            });
        }
    }

    class NavigationPageNoBack: NavigationPage{
        string title;
        public NavigationPageNoBack(Page p, string title) : base(p) { this.title = title; }
        protected override bool OnBackButtonPressed()
        {
            if(((NavigationPage)Navigation.ModalStack.LastOrDefault()).CurrentPage.Title == title){
                return true;
            }
            return base.OnBackButtonPressed();
        }
    }
}
