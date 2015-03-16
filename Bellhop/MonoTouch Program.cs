using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Microsoft.Xna.Framework;
using Parse;

//using MonoTouch.FacebookConnect;

namespace Jubble
{
    [Register("SDAppDelegate")]
    class Program : ExEnEmTouchApplication
    {
        public override bool WillFinishLaunching(UIApplication application, NSDictionary launchOptions)
        {
//            FBSettings.DefaultAppID = GameGlobals.FacebookAppID;
//            FBSettings.DefaultDisplayName = GameGlobals.FacebookDisplayName;

            // Initialize the Parse client with your Application ID and .NET Key found on
            // your Parse dashboard
            ParseClient.Initialize("9fcrHosyVMPdnGwRBvwcA7pqjdtI0Bar5EcOLNCQ",
                "oT31iO3vfXy8mQ1pOZNSZpmIk7WqaJmO3fHZdPS0");
            return true;
        }

        public override void FinishedLaunching(UIApplication application)
        {
            game = new MainGame();
            game.Run();
        }

        static void Main(string[] args)
        {
            UIApplication.Main(args, null, "SDAppDelegate");
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
            return UIInterfaceOrientationMask.Landscape;
        }

//        public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
//        {
//            // We need to handle URLs by passing them to FBSession in order for SSO authentication
//            // to work.
//            return FBSession.ActiveSession.HandleOpenURL(url);
//        }
//
//        public override void OnActivated (UIApplication application)
//        {
//            // We need to properly handle activation of the application with regards to SSO
//            // (e.g., returning from iOS 6.0 authorization dialog or from fast app switching).
//            FBSession.ActiveSession.HandleDidBecomeActive();
//        }
    }
}
