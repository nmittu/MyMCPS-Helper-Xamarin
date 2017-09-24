using CoreGraphics;
using Google.MobileAds;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MyMCPSHelper.InClassAdView), typeof(MyMCPSHelper.iOS.InClassAdRenderer))]
namespace MyMCPSHelper.iOS
{
    public class InClassAdRenderer: ViewRenderer
    {
		const string AdmobID = "ca-app-pub-5192562049043590/1047750499";

		BannerView adView;
		bool viewOnScreen;

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement == null)
				return;

			if (e.OldElement == null)
			{
				adView = new BannerView(AdSizeCons.Banner)
				{
					AdUnitID = AdmobID,
					RootViewController = GetVisibleViewController()
				};

				adView.AdReceived += (sender, args) =>
				{
					if (!viewOnScreen) this.AddSubview(adView);
					viewOnScreen = true;
				};

				Request request = Request.GetDefaultRequest();
				request.TestDevices = new string[] { Request.SimulatorId.ToString() };

				adView.LoadRequest(request);
				base.SetNativeControl(adView);
			}
		}
		UIViewController GetVisibleViewController()
		{
			var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;

			if (rootController.PresentedViewController == null)
				return rootController;

			if (rootController.PresentedViewController is UINavigationController)
			{
				return ((UINavigationController)rootController.PresentedViewController).VisibleViewController;
			}

			if (rootController.PresentedViewController is UITabBarController)
			{
				return ((UITabBarController)rootController.PresentedViewController).SelectedViewController;
			}

			return rootController.PresentedViewController;
		}
    }
}
