﻿using Android.Widget;
using Android.Gms.Ads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Gms;
using MyMCPSHelper;

[assembly: ExportRenderer(typeof(MyMCPSHelper.InClassAdView), typeof(MyMCPSHelper.Droid.InClassAdRenderer))]

namespace MyMCPSHelper.Droid
{
    public class InClassAdRenderer: ViewRenderer<InClassAdView, AdView>
    {
		string adUnitId = string.Empty;
		//Note you may want to adjust this, see further down.
		AdSize adSize = AdSize.Banner;
		AdView adView;
		AdView CreateNativeAdControl()
		{
			if (adView != null)
				return adView;

			// This is a string in the Resources/values/strings.xml that I added or you can modify it here. This comes from admob and contains a / in it
			adUnitId = "ca-app-pub-5192562049043590/3729754153";
			adView = new AdView(Forms.Context);
			adView.AdSize = adSize;
			adView.AdUnitId = adUnitId;

			var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

			adView.LayoutParameters = adParams;

			adView.LoadAd(new AdRequest
						  .Builder()
						  .Build());
			return adView;
		}

        protected override void OnElementChanged(ElementChangedEventArgs<InClassAdView> e)
		{
			base.OnElementChanged(e);
			if (Control == null)
			{
				CreateNativeAdControl();
				SetNativeControl(adView);
			}
		}
    }
}
