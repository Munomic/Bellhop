using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common;
using Android.Gms.Ads;
using Microsoft.Xna.Framework;

namespace Jubble
{
    public class CustomAdListner : AdListener
    {
        private bool firstTimeLoad = true;
        public AdView View { get; set; }

        public override void OnAdLoaded()
        {
            // The following is needed to make the Ad actually show up...
            if( firstTimeLoad )
            {
                firstTimeLoad = false;
                var requestBuilder = new AdRequest.Builder();
                View.LoadAd( requestBuilder.Build() );
                if( GameSettings.HidePlaceholderAd != null )
                {
                    GameSettings.HidePlaceholderAd();
                }
            }
            View.BringToFront();
            base.OnAdLoaded();
        }
    }

    [Activity (Label = "Turtle Bucket", MainLauncher = true,
        Theme = ExEnAndroidActivity.DefaultTheme,
        ConfigurationChanges = ExEnAndroidActivity.DefaultConfigChanges)]
    public class TurtleBucketActivity : ExEnAndroidActivity, Android.Gms.Common.IGooglePlayServicesClientConnectionCallbacks, Android.Gms.Common.IGooglePlayServicesClientOnConnectionFailedListener, View.IOnClickListener
    {
        MainGame game;
        Android.Gms.Games.GamesClient.Builder builder;
        Android.Gms.Games.GamesClient gamesClient;
        Android.Gms.Common.ConnectionResult connectResult = null;
        bool shouldResolveOnFail = false;
        const int CUSTOM_INTENT_REQUEST = 4739;
        Action onCustomIntentClose = null;
        Android.App.Backup.BackupManager backupManager = null;

        //For testing, put your application key in m_sig
        // if you are not using the static responses, you will need this to verify purchase requests
        string m_sig = "";
        bool isAdShowing = false;

        Dialog rateDialog = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate( bundle );

            // Add an exception handler for all uncaught exceptions.
            AndroidEnvironment.UnhandledExceptionRaiser += MainActivity_UnhandledExceptionHandler;

            GameSettings.GameVersion = PackageManager.GetPackageInfo( PackageName, 0 ).VersionName;
            SetContentView( Resource.Layout.Main );

            GameSettings.ShowAd = showAd;
            GameSettings.ShowRateDialog = showRateDialog;

            float screenWidth = Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Xdpi;
            float screenHeight = Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Ydpi;
            float size = (float)Math.Sqrt( Math.Pow( screenWidth, 2 ) + Math.Pow( screenHeight, 2 ) );

            if( size >= 6 ) // Tablet devices should have a screen size greater than 6 inches
            {
                GameSettings.IsPadDevice = true;
            }
            else
            {
                if( screenWidth / screenHeight > 1.5f || screenHeight / screenWidth > 1.5f )
                {
                    GameSettings.IsWidescreen = true;
                }
                else
                {
                    GameSettings.IsWidescreen = false;
                }
                GameSettings.IsPadDevice = false;
            }

            GameSettings.AppContext = this.ApplicationContext;
            backupManager = new Android.App.Backup.BackupManager( BaseContext );
            GameSettings.AndroidBackup = () =>
            {
                backupManager.DataChanged();
            };

            builder = new Android.Gms.Games.GamesClient.Builder (this.BaseContext, this, this);
            gamesClient = builder.Create ();
            GameSettings.GamesClient = gamesClient;
            GameSettings.GooglePlayConnect = () => {
                if( connectResult != null )
                {
                    tryResolvingGooglePlay();
                }
            };
            GameSettings.AndroidIntentShow = showIntent;
            GameSettings.AchievementUnlock = (string achievementId) => { 
                if (GameSettings.GamesClient.IsConnected) {
                    GameSettings.GamesClient.UnlockAchievement (achievementId);
                }
            };
            GameSettings.ExitApp = () => {
                this.Finish();
                Android.OS.Process.KillProcess( Android.OS.Process.MyPid() );
            };

            game = new MainGame();
            game.Start( this );
        }

        protected override void OnResume ()
        {
            // Publish App Install
            Xamarin.FacebookBinding.Settings.PublishInstallAsync( GameSettings.AppContext, GameSettings.FacebookAppID );
            base.OnResume();
        }

        protected override void OnStart ()
        {
            shouldResolveOnFail = true;
            gamesClient.Connect ();
            base.OnStart ();
        }

        protected override void OnStop ()
        {
            gamesClient.Disconnect ();
            base.OnStop ();
        }

        void Android.Gms.Common.IGooglePlayServicesClientConnectionCallbacks.OnConnected (Bundle p0)
        {
            shouldResolveOnFail = false;
            // Sync score to google play
            gamesClient.SubmitScore( "CgkI_5Ku-dIaEAIQAA", (long)( GameSettings.BestScore * 1000 ) );
            GameLog.Log ("Connected to Google Play!");
        }

        void Android.Gms.Common.IGooglePlayServicesClientConnectionCallbacks.OnDisconnected ()
        {
            GameLog.Log ("Disconnected from Google Play");
        }

        void Android.Gms.Common.IGooglePlayServicesClientOnConnectionFailedListener.OnConnectionFailed (Android.Gms.Common.ConnectionResult p0)
        {
            if( p0.HasResolution )
            {
                connectResult = p0;
                if( shouldResolveOnFail )
                {
                    tryResolvingGooglePlay ();
                }
            }
            GameLog.Log ("OnConnectionFailed: " + p0.ToString());
        }

        void tryResolvingGooglePlay()
        {
            GameLog.Log ("Trying to resolve Google Play connection");
            if( !gamesClient.IsConnected && !gamesClient.IsConnecting )
            {
                if( connectResult != null )
                {
                    connectResult.StartResolutionForResult (this, connectResult.ErrorCode);
                }
            }
        }

        string getSHA1CertFingerprint()
        {
            try
            {
                IList<Android.Content.PM.Signature> sigs = GameSettings.AppContext.PackageManager.GetPackageInfo(
                                                               GameSettings.AppContext.PackageName, Android.Content.PM.PackageInfoFlags.Signatures ).Signatures;
                if( sigs.Count == 0 )
                {
                    return "No Signature";
                }
                else if( sigs.Count > 1 )
                {
                    return "Multiple Signatures";
                }

                Java.Security.MessageDigest md = Java.Security.MessageDigest.GetInstance( "SHA" );
                byte[] digest = md.Digest( sigs[ 0 ].ToByteArray() );
                String hexString = "";
                for( int i = 0; i < digest.Length; i++ )
                {
                    if( i > 0 )
                    {
                        hexString += ":";
                    }
                    hexString += digest[ i ].ToString( "x2" );
                }
                return hexString;
                // md.Update( sigs[ 0 ].ToByteArray() );
                //return Convert.ToBase64String( md.Digest() );
            }
            catch( Exception e )
            {
                return e.Message;
            }
        }

        protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
        {
            if( connectResult != null &&
                requestCode == connectResult.ErrorCode )
            {
                if( resultCode == Result.Ok )
                {
                    shouldResolveOnFail = true;
                    gamesClient.Connect ();
                }
                else
                {
                    GameLog.Log ("Unhandled result code: " + resultCode);
                    GameLog.Log( "Some debug info: " );
                    GameLog.Log( "Package Name: " + GameSettings.AppContext.PackageName );
                    GameLog.Log( "SHA1: " + getSHA1CertFingerprint() );
                    GameLog.Log( "AppID: " + GameSettings.AppContext.Resources.GetString( GameSettings.AppContext.Resources.GetIdentifier( "app_id", "string", GameSettings.AppContext.PackageName ) ) );
                }
            }
            else if( requestCode == CUSTOM_INTENT_REQUEST )
            {
                if( resultCode == Result.Ok ||
                    resultCode == Result.Canceled )
                {
                    onCustomIntentClose ();
                }
            }
            base.OnActivityResult (requestCode, resultCode, data);
        }


        public override bool OnKeyDown (Keycode keyCode, KeyEvent e)
        {
            if( keyCode == Keycode.Back ||
                keyCode == Keycode.Menu )
            {
                return true;
            }
            else
            {
                return base.OnKeyDown (keyCode, e);
            }
        }

        public override bool OnKeyUp (Keycode keyCode, KeyEvent e)
        {
            if( keyCode == Keycode.Back )
            {
                game.OnBackButtonPressed ();
                return true;
            }
            else if( keyCode == Keycode.Menu )
            {
                game.OnMenuButtonPressed ();
                return true;
            }
            else
            {
                return base.OnKeyUp (keyCode, e);
            }
        }

        protected override void Dispose (bool disposing)
        {
            // Remove the exception handler.
            AndroidEnvironment.UnhandledExceptionRaiser -= MainActivity_UnhandledExceptionHandler;
            base.Dispose( disposing );
        }

        void MainActivity_UnhandledExceptionHandler(object sender, RaiseThrowableEventArgs e)
        {
        }

        void showIntent(Android.Content.Intent intent, Action onIntentClose)
        {
            // Save out progress before going going to the intent
            //          GameSettings.Commit();

            onCustomIntentClose = onIntentClose;
            StartActivityForResult (intent, CUSTOM_INTENT_REQUEST);
        }

        void showAd(bool shouldShow)
        {
            if( shouldShow )
            {
                var ad = new AdView( GameSettings.AppContext );
                ad.AdSize = AdSize.SmartBanner;
                ad.AdUnitId = GameSettings.AdmobId;
                ad.AdListener = new CustomAdListner() { View = ad };
                this.RunOnUiThread( () => {
                    this.AddView( ad );
                    var requestBuilder = new AdRequest.Builder();
                    ad.LoadAd( requestBuilder.Build() );
                } );
            }
        }

        void showRateDialog(bool forceShow = false)
        {
            // Load from SharedPreferences
            Android.Content.ISharedPreferences store = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(GameSettings.AppContext);
            if (!forceShow && store.GetBoolean ("RatedApp", false))
                return;

            this.RunOnUiThread (() => {
                rateDialog = new Dialog (this);
                rateDialog.SetTitle ("Rate Turtle Bucket!");

                LinearLayout layout = new LinearLayout (this);
                layout.Orientation = Orientation.Vertical;

                TextView textView = new TextView (this);
                textView.SetText ("If you enjoyed playing Turtle Bucket, please take a moment to rate it. Thanks for your support!", TextView.BufferType.Normal);
                textView.SetWidth (240);
                textView.SetPadding (4, 0, 4, 10);
                layout.AddView (textView);

                Button buttonYes = new Button (this);
                buttonYes.SetText ("Rate Turtle Bucket", TextView.BufferType.Normal);
                buttonYes.SetOnClickListener (this);
                buttonYes.Tag = "Yes";
                layout.AddView (buttonYes);

                Button buttonNo = new Button (this);
                buttonNo.SetText ("Not now", TextView.BufferType.Normal);
                buttonNo.SetOnClickListener (this);
                buttonNo.Tag = "No";
                layout.AddView (buttonNo);

                rateDialog.SetContentView (layout);
                rateDialog.Show ();
            });
        }

        void View.IOnClickListener.OnClick (View v)
        {
            if (v.Tag.ToString () == "Yes") {
                this.StartActivity (new Intent (Intent.ActionView, Android.Net.Uri.Parse ("market://details?id=com.munomic.turtlebucket.app")));
                Android.Content.ISharedPreferences store = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences (GameSettings.AppContext);
                Android.Content.ISharedPreferencesEditor storeEditor = store.Edit ();
                storeEditor.PutBoolean ("RatedApp", true);
                storeEditor.Commit ();
            }
            rateDialog.Dismiss ();
        }
    }
}


