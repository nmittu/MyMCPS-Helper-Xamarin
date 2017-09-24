using System;
using System.Collections.Generic;
using System.Linq;
using Google.MobileAds;

using Foundation;
using UIKit;

namespace MyMCPSHelper.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            MobileAds.Configure("ca-app-pub-5192562049043590~3171173344");

            return base.FinishedLaunching(app, options);
        }
    }
}
