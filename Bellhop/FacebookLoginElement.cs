using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using Facebook;
using System.Collections.Generic;
using Xen2D;

namespace Jubble
{
	public class FacebookLoginElement
	{
		NSUrl nsUrl;
        UIViewController _parentView;
		UIWebView web;
		FacebookClient _fb;
		
		public FacebookLoginElement (string appId, string extendedPermissions)
		{
			_fb = new FacebookClient ();
			nsUrl = new NSUrl (GetFacebookLoginUrl (appId, extendedPermissions));
			
		}

		public string Url {
			get {
				return nsUrl.ToString ();
			}
		}
		
		static bool NetworkActivity {
			set {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value;
			}
		}
		
		// We use this class to dispose the web control when it is not
		// in use, as it could be a bit of a pig, and we do not want to
		// wait for the GC to kick-in.
		class WebViewController : UIViewController {
			public WebViewController () : base ()
			{
                UIBarButtonItem backButton = new UIBarButtonItem( "Back", UIBarButtonItemStyle.Done, null, null );
                backButton.Clicked += (o, e) => {
                    NavigationController.DismissViewController(true, null);
                };
                this.NavigationItem.LeftBarButtonItem = backButton;
            }
			
			public bool Autorotate { get; set; }
			
			public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
			{
				return Autorotate;
			}
		}

        public void ShowLogin()
		{
			var vc = new WebViewController () {
                Autorotate = true
			};

			web = new UIWebView (UIScreen.MainScreen.Bounds) {
				BackgroundColor = UIColor.White,
                AutoresizingMask = UIViewAutoresizing.All
			};

			// We delete cache and cookies so it does not remember our login information
			DeleteCacheandCookies ();

			web.LoadStarted += (webview, e) => {
				NetworkActivity = true;
			};

			web.LoadFinished += (webview, e) =>  {
				NetworkActivity = false;
				var wb = webview as UIWebView;
				FacebookOAuthResult oauthResult;
				if (!_fb.TryParseOAuthCallbackUrl (new Uri (wb.Request.Url.ToString()), out oauthResult))
				{
					return;
				}
				
				if (oauthResult.IsSuccess)
				{
					// Facebook Granted Token
					var accessToken = oauthResult.AccessToken;
					LoginSucceded(accessToken);
				}
				else
				{
					// user cancelled login
					LoginSucceded(string.Empty);
				}
			};

			web.LoadError += (webview, args) => {
				NetworkActivity = false;
                if( args.Error.Code != (int)NSUrlError.Cancelled )
                {
    				if (web != null)
    					web.LoadHtmlString (
    						String.Format ("<html><center><font size=+5 color='red'>{0}:<br>{1}</font></center></html>",
    					               "An error occurred: ", args.Error.LocalizedDescription), null);
                }
//                else
//                {
//                    web.LoadRequest(NSUrlRequest.FromUrl(nsUrl));
//                }
			};
            vc.NavigationItem.Title = "Facebook Login";
			
			vc.View.AutosizesSubviews = true;
			vc.View.AddSubview (web);

			web.LoadRequest (NSUrlRequest.FromUrl (nsUrl));

            _parentView = new UIViewController();
            _parentView.View.UserInteractionEnabled = false;

            UINavigationController navController = new UINavigationController( vc );
            Globals.Graphics.UIView.AddSubview( _parentView.View );
            _parentView.PresentModalViewController( navController, true );
		}

		/// <summary>
		/// Gets the facebook login URL.
		/// </summary>
		/// <returns>The facebook login URL.</returns>
		/// <param name="appId">Facebook App identifier.</param>
		/// <param name="extendedPermissions">Extended permissions.</param>
		/// <remarks>
		/// For extensive list of available extended permissions refer to 
		/// https://developers.facebook.com/docs/reference/api/permissions/
		/// </remarks>
		private string GetFacebookLoginUrl (string appId, string extendedPermissions)
		{
			var parameters = new Dictionary<string, object>();
			parameters["client_id"] = appId;
			parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
			parameters["response_type"] = "token";
			parameters["display"] = "touch";
			
			// add the 'scope' only if we have extendedPermissions.
			if (!string.IsNullOrEmpty (extendedPermissions))
			{
				// A comma-delimited list of permissions
				parameters["scope"] = extendedPermissions;
			}

			return _fb.GetLoginUrl(parameters).AbsoluteUri;
		}

		private void LoginSucceded(string accessToken)
		{
			var fb = new FacebookClient(accessToken);

			fb.GetTaskAsync ("me?fields=id").ContinueWith (t => {
				if(!t.IsFaulted)
				{
//					var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
//					
//					if (t.Exception != null)
//					{
//						appDelegate.FacebookLoggedIn (false, accessToken, null, t.Exception);
//						dvc.NavigationController.PopViewControllerAnimated (true);
//						return;
//					}
//					
//					var result = (IDictionary<string, object>)t.Result;
//					var id = (string)result["id"];
//					appDelegate.BeginInvokeOnMainThread ( () => {
//						appDelegate.FacebookLoggedIn (true, accessToken, id, null);
//						dvc.NavigationController.PopViewControllerAnimated (true);
//					});
				}
			});
		}

		private void DeleteCacheandCookies ()
		{
			NSUrlCache.SharedCache.RemoveAllCachedResponses ();
			NSHttpCookieStorage storage = NSHttpCookieStorage.SharedStorage;
			
			foreach (var item in storage.Cookies) {
				storage.DeleteCookie (item);
			}
			NSUserDefaults.StandardUserDefaults.Synchronize ();
		}
	}
}

