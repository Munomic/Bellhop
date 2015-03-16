using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Jubble
{
    public static class GameSettings
    {
        // TODO: UPDATE THESE VALUES!
#if APPSTORE
        public const string AdmobId = "ca-app-pub-9491727157739208/4647819631"; // Turtle Bucket iOS
#elif MARKETPLACE
        public const string AdmobId = "ca-app-pub-9491727157739208/9078019238"; // Turtle Bucket Android
#else
        public const string AdmobId = "a151db597ed35f9"; // Test App
#endif
        public const uint AppStoreID = 689662575; // TODO: UPDATE THIS!
        public const string FacebookAppID = "537018463069480"; // TODO: UPDATE THIS!
        public const string FacebookDisplayName = "Tapslinger";
#if ANDROID
        public static Android.Content.Context AppContext = null;
        public static Action<string> AchievementUnlock = null;
        public static Action AndroidBackup = null;
        public static Action<bool> ShowAd = null;
        public static Action HidePlaceholderAd = null;
        public static Action<bool> ShowRateDialog = null;
        public static Android.Gms.Games.GamesClient GamesClient = null;
        public static Action GooglePlayConnect = null;
		public static Action<Android.Content.Intent, Action> AndroidIntentShow = null;
		public static Action ExitApp = null;
#endif

        private static string KEY_DEVICEID = "MunomicDeviceId";

        public static string DeviceId
        {
            get
            {
#if MONOTOUCH
#if !APPSTORE
                // Use the UDID for TestFlight
                string result = MonoTouch.UIKit.UIDevice.CurrentDevice.UniqueIdentifier;
#else
                // We need to use our own device id since UDID is deprecated as of March 2012
                string result = MonoTouch.Foundation.NSUserDefaults.StandardUserDefaults.StringForKey( KEY_DEVICEID );
                if (string.IsNullOrEmpty(result))
                {
                    result = Guid.NewGuid().ToString();
                    MonoTouch.Foundation.NSUserDefaults.StandardUserDefaults.SetString( result, KEY_DEVICEID );
                    MonoTouch.Foundation.NSUserDefaults.StandardUserDefaults.Synchronize();
                }
#endif
#else
                string result = Guid.NewGuid().ToString();
#endif

                return result;
            }
        }

#if ANDROID
        private static string versionName = "";
#endif

        public static string GameVersion
        {
            get
            {
#if MONOTOUCH
                string result = MonoTouch.Foundation.NSBundle.MainBundle.ObjectForInfoDictionary( "CFBundleVersion" ).ToString( );
#elif WINDOWS_PHONE
                var nameHelper = new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName);
                string result = nameHelper.Version.ToString();
#elif ANDROID
                string result = versionName;
#else
                string result = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
#endif
                return result;
            }
#if ANDROID
            // This is set from MainActivity.cs
            set
            {
                versionName = value;
            }
#endif
        }

        // Game Variables
#if WINDOWS
        public const int ScreenWidth = 1136;//568;
        public const int ScreenHeight = 640;//320;
#else
        public const int ScreenWidth = 568;
        public const int ScreenHeight = 320;
#endif

        public static bool IsPadDevice = true;
        public static bool IsWidescreen = false;
        public static bool IsMenuMusicPlaying = false;
        public static float MusicVolume = 0.5f;
        public static float SFXVolume = 0.5f;

        public static bool HasSeenTutorial = false;
        public static float BestScore = 0.0f;

        public static void Load()
		{
#if MONOTOUCH
            // Load from iCloud
            MonoTouch.Foundation.NSUbiquitousKeyValueStore store = MonoTouch.Foundation.NSUbiquitousKeyValueStore.DefaultStore;
            BestScore = (float)store.GetDouble( "TurtleBucket_BestScore" );
            HasSeenTutorial = store.GetBool( "TurtleBucket_HasSeenTutorial" );
#elif ANDROID
			// Load from SharedPreferences
			Android.Content.ISharedPreferences store = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences( GameSettings.AppContext );
			BestScore = store.GetFloat( "TurtleBucket_BestScore", 0.0f );
			HasSeenTutorial = store.GetBoolean( "TurtleBucket_HasSeenTutorial", false );
#else
            BestScore = 0.0f;
            HasSeenTutorial = false;
#endif
			GameLog.Log( "Loaded Settings" );
		}

        public static void Unload()
        {
            BestScore = 0.0f;
            HasSeenTutorial = false;
        }

        public static void Commit()
		{
#if MONOTOUCH
            // Commit the changes to iCloud
            MonoTouch.Foundation.NSUbiquitousKeyValueStore store = MonoTouch.Foundation.NSUbiquitousKeyValueStore.DefaultStore;
            store.SetDouble( "TurtleBucket_BestScore", BestScore );
            store.SetBool( "TurtleBucket_HasSeenTutorial", HasSeenTutorial );
            store.Synchronize();
#elif ANDROID
			// Commit the changes to SharedPreferences
			Android.Content.ISharedPreferences store = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences( GameSettings.AppContext );
			Android.Content.ISharedPreferencesEditor storeEditor = store.Edit();
			storeEditor.PutFloat( "TurtleBucket_BestScore", BestScore );
			storeEditor.PutBoolean( "TurtleBucket_HasSeenTutorial", HasSeenTutorial );
			storeEditor.Commit();
#endif
			GameLog.Log( "Saved Settings" );
		}
    }
}
