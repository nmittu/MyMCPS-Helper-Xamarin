using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyMCPSHelper.HistoryViews
{
    public partial class TermsView : ContentPage
    {
        private static List<HistoricTerm> tref = null;
        public TermsView()
        {
            InitializeComponent();

            Title = "Terms";

			Task.Run(() => {
                var terms = App.AccMangr.loadHistory();
                tref = terms;

				Device.BeginInvokeOnMainThread(() => {
					spinner.IsRunning = false;
					spinner.IsVisible = false;
                    TermsList.ItemsSource = terms;
				});
			});
        }
    }
}
