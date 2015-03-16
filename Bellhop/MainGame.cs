using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Xen2D;
using XenAspects;
using DuckyEngine;
using System.Collections.Generic;
using System.Xml.Linq;
#if MONOTOUCH
using MTiRate;
using GoogleAdMobAds;
#endif

namespace Jubble
{
#if MONOTOUCH
    public class LeaderboardViewDelegate : MonoTouch.GameKit.GKLeaderboardViewControllerDelegate
    {
        public bool ShouldExit = false;
        public MonoTouch.UIKit.UIViewController UIViewController = null;

        public override void DidFinish( MonoTouch.GameKit.GKLeaderboardViewController viewController )
        {
            this.UIViewController.DismissModalViewControllerAnimated( true );
            this.ShouldExit = true;
        }
    }
#endif

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        ResizedSpriteBatch _spriteBatch;
        float horizontalOffset = 0.0f;

        PerformanceTimer _updateLoopPerf;
        PerformanceTimer _drawLoopPerf;
        DuckyGameEngine _duckyEngine;

        bool _isAdHidden = false;
        bool _isGameCenterShowing = false;
        float _timeToAdShow = 0.0f;

#if MONOTOUCH
        MonoTouch.UIKit.UIViewController _leaderboardParent = null;
        MonoTouch.GameKit.GKLeaderboardViewController _leaderboardView = null;
        LeaderboardViewDelegate _leaderboardViewDelegate = null;
        MonoTouch.UIKit.UIViewController _gamecenterAuthViewParent = null;

        MonoTouch.UIKit.UIImage _screenCapture = null;

        GADBannerView adView;
        bool adViewOnScreen = false;
#elif ANDROID
        Android.Graphics.Bitmap _screenCapture = null;
#endif

        public MainGame ()
        {
            _duckyEngine = new DuckyGameEngine();

#if MONOTOUCH
            // Set iRate settings
            iRate.SharedInstance.PromptAtLaunch = false;
//            iRate.SharedInstance.AppStoreID = GameGlobals.AppStoreID;

#if !APPSTORE
            GADRequest.Request.TestDevices = new string[] { GADRequest.GAD_SIMULATOR_ID };
#endif
//            if (MonoTouch.UIKit.UIApplication.SharedApplication.RespondsToSelector(new MonoTouch.ObjCRuntime.Selector("setStatusBarHidden: withAnimation:")))
//            {
//                MonoTouch.UIKit.UIApplication.SharedApplication.SetStatusBarHidden( true, MonoTouch.UIKit.UIStatusBarAnimation.Fade );
//            }
//            else
//            {
//                MonoTouch.UIKit.UIApplication.SharedApplication.SetStatusBarHidden( true, true );
//            }
#endif

            //320x480 - iPhone
            //1024x768 - iPad
            Globals.Graphics = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";
            Globals.Content = Content;
#if ANDROID
            Microsoft.Xna.Framework.Content.ContentManager.Load2xSize = true;
#endif
            Globals.Graphics.PreferredBackBufferWidth = GameSettings.ScreenWidth;
            Globals.Graphics.PreferredBackBufferHeight = GameSettings.ScreenHeight;

            Globals.Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            // Are we a pad-style device or a phone-style device?
#if WINDOWS && !WINDOWS_PHONE
            GameSettings.IsPadDevice = false; // Change this to swap between iPad and iPhone
            GameSettings.IsWidescreen = true;
#elif WINDOWS_PHONE
            GameSettings.IsPadDevice = false;
#elif MONOTOUCH
            GameSettings.IsPadDevice = (MonoTouch.UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom
                    == MonoTouch.UIKit.UIUserInterfaceIdiom.Pad);
            if ( !GameSettings.IsPadDevice && 
                ( MonoTouch.UIKit.UIScreen.MainScreen.Bounds.Height * MonoTouch.UIKit.UIScreen.MainScreen.Scale >= 1136 ||
                    MonoTouch.UIKit.UIScreen.MainScreen.Bounds.Width * MonoTouch.UIKit.UIScreen.MainScreen.Scale >= 1136 ) )
            {
                GameSettings.IsWidescreen = true;
            }
#elif ANDROID
            // This is set inside MainActivity.cs
#else
            GameSettings.IsPadDevice = false;
#endif

#if WINDOWS_PHONE
            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
#endif

            // Extend battery life under lock.
            //InactiveSleepTime = TimeSpan.FromSeconds(1);

            _updateLoopPerf = new PerformanceTimer();
            _drawLoopPerf = new PerformanceTimer();

            this.IsFixedTimeStep = false;

#if ANDROID
            Globals.Graphics.DeviceReset += new EventHandler<EventArgs>(OnGraphicsComponentDeviceReset);
            Window.AllowUserResizing = true;
#endif

#if MONOTOUCH
#if ANALYTICS
            // Start Analytics
            if (GameSettings.IsPadDevice)
            {
                GameUtils.GoogleAnalyticsTracker = GoogleAnalytics.GAI.SharedInstance.GetTracker( GameGlobals.iPadGoogleAnalyticsID );
            }
            else
            {
                GameUtils.GoogleAnalyticsTracker = GoogleAnalytics.GAI.SharedInstance.GetTracker( GameGlobals.iPhoneGoogleAnalyticsID );
            }
            Console.WriteLine("[Analytics: Started]");

            // Report Device Info
//            MonoTouch.Foundation.NSError error;
            // Only 5-slots for custom vars. Use wisely!
            Console.WriteLine("UniqueId: " + GameSettings.DeviceId);
            Console.WriteLine("Model: " + MonoTouch.UIKit.UIDevice.CurrentDevice.Model);
            Console.WriteLine("Name: " + MonoTouch.UIKit.UIDevice.CurrentDevice.Name);
            Console.WriteLine("SystemName: " + MonoTouch.UIKit.UIDevice.CurrentDevice.SystemName);
            Console.WriteLine("Version: " + MonoTouch.UIKit.UIDevice.CurrentDevice.SystemVersion);
//            GoogleAnalytics.GANTracker.SharedTracker.SetCustomVariable(1, "UniqueId", GameSettings.DeviceId, out error);
//            GoogleAnalytics.GANTracker.SharedTracker.SetCustomVariable(2, "Model", MonoTouch.UIKit.UIDevice.CurrentDevice.Model, out error);
//            GoogleAnalytics.GANTracker.SharedTracker.SetCustomVariable(3, "Name", MonoTouch.UIKit.UIDevice.CurrentDevice.Name, out error);
//            GoogleAnalytics.GANTracker.SharedTracker.SetCustomVariable(4, "SystemName", MonoTouch.UIKit.UIDevice.CurrentDevice.SystemName, out error);
//            GoogleAnalytics.GANTracker.SharedTracker.SetCustomVariable(5, "SystemVersion", MonoTouch.UIKit.UIDevice.CurrentDevice.SystemVersion, out error);
#endif

#endif
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;

#if ANDROID
            ScreenUtility.InitGraphicsMode( ScreenWidth, ScreenHeight, false );
            resizeScreen ();
#else
            ScreenUtility.InitGraphicsMode( GameSettings.ScreenWidth, GameSettings.ScreenHeight, true );// false );
            resizeScreen( 4.73333f );
#endif

#if MONOTOUCH
//            if (MonoTouch.UIKit.UIDevice.CurrentDevice.SystemVersion.CompareTo("6.0") < 0)
//            {
//                if (!MonoTouch.GameKit.GKLocalPlayer.LocalPlayer.Authenticated)
//                {
//                    MonoTouch.GameKit.GKLocalPlayer.LocalPlayer.Authenticate( gameCenterOldAuthenticateHandler );
//                }
//            }
//            else
//            {
//                if (!MonoTouch.GameKit.GKLocalPlayer.LocalPlayer.Authenticated)
//                {
//                    MonoTouch.GameKit.GKLocalPlayer.LocalPlayer.AuthenticateHandler = gameCenterAuthenticateHandler;
//                }
//            }
#endif

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new ResizedSpriteBatch( Globals.GraphicsDevice );
            
            GameLog.Log( "Loading Content" );

            Globals.Content = Content;
            GameSettings.Load();
            GameParticles.Load();

            if( GameSettings.IsWidescreen )
            {
#if ANDROID
                Android.Content.Res.AssetManager assets = ContentHelpers.GetAssetManager( Globals.Content );
                _duckyEngine.Load( XElement.Load( assets.Open( "Content/GameplayWidescreen.xml" ) ) );
#else
                _duckyEngine.Load( XElement.Load( "Content/GameplayWidescreen.xml" ) );
#endif
            }
            else
            {
#if ANDROID
                Android.Content.Res.AssetManager assets = ContentHelpers.GetAssetManager( Globals.Content );
                _duckyEngine.Load( XElement.Load( assets.Open( "Content/Gameplay.xml" ) ) );
#else
                _duckyEngine.Load( XElement.Load( "Content/Gameplay.xml" ) );
#endif
            }
            _duckyEngine.BackgroundColor = Color.Black;
            _duckyEngine.Scene.Controller = new GameplayDirector();

#if MONOTOUCH
            adView = new GADBannerView (size: GADAdSizeCons.Banner, origin: new System.Drawing.PointF (0, 0)) {
                AdUnitID = GameSettings.AdmobId,
                RootViewController = Globals.Graphics.UIViewController,
                BackgroundColor = MonoTouch.UIKit.UIColor.Clear,
                Opaque = false
            };

            adView.DidReceiveAd += (sender, args) => {
                if( !adViewOnScreen )
                {
                    Globals.Graphics.UIView.AddSubview( adView );
                    adViewOnScreen = true;
                }
            };

            // TODO: UNCOMMENT!!!
//            adView.LoadRequest( GADRequest.Request );
#elif ANDROID
            GameSettings.HidePlaceholderAd = () => {
//                _duckyEngine.Scene[ "Main" ][ "Ad" ].Visible = false;
//                _duckyEngine.Scene[ "Main" ][ "Ad" ].Enabled = false;
                _isAdHidden = true;
            };
            GameSettings.ShowAd(true);
#endif


            GameLog.Log("Game Loaded");
        }

        protected override void UnloadContent()
        {
            GameSettings.Unload();
            _duckyEngine.Unload();
            MediaPlayer.Stop();

            GameLog.Log( "Game Unloaded" );
        }

        protected override void OnActivated( object sender, EventArgs args )
        {
            GameLog.Log("Game Re-Activated");

            //// Coming back from incoming calls, alerts, the task-switching bar being displayed, etc.
            //if( _gameScene != null )
            //{
            //    _gameScene.Resume();
            //}

            GameLog.Log("Refreshing Cloud Data");

            // Refresh cloud data
            GameSettings.Unload();
            GameSettings.Load();

            GameLog.Log("Cloud Data Refreshed");

            GameLog.Log("Scene Resumed");

            base.OnActivated( sender, args );
        }

        protected override void OnDeactivated( object sender, EventArgs args )
        {
            GameLog.Log("Game Deactivated");

            //// Going out to incoming calls, alerts, the task-switching bar being displayed, etc.
            //if( _gameScene != null )
            //{
            //    _gameScene.Pause();
            //}

            GC.Collect();
            base.OnDeactivated( sender, args );
        }

#if MONOTOUCH

        // These will notify you if your game is being backgrounded or foregrounded (iOS multi-tasking)
        protected override void OnEnterForeground( object sender, EventArgs args )
        {
            GameLog.Log("Game Entering Foreground");

            //if( _gameScene != null )
            //{
            //    _gameScene.Resume();
            //}

            GameLog.Log("Refreshing Cloud Data");

            // Refresh cloud data
            GameSettings.Unload();
            GameSettings.Load();

            GameLog.Log("Cloud Data Refreshed");
        }

        protected override void OnEnterBackground( object sender, EventArgs args )
        {
            GameLog.Log("Game Entering Background");

            //if( _gameScene != null )
            //{
            //    _gameScene.Pause();
            //}
        }
        
//        protected void gameCenterOldAuthenticateHandler( MonoTouch.Foundation.NSError error )
//        {
//            if (error != null)
//            {
//                // TODO: Handle Error gracefully
//            }
//            else
//            {
//                GameLog.Log("Game Center Authenticated");
//            }
//        }

//        protected void gameCenterAuthenticateHandler( MonoTouch.UIKit.UIViewController viewController, MonoTouch.Foundation.NSError error )
//        {
//            if (viewController != null)
//            {
//                _gamecenterAuthViewParent = new MonoTouch.UIKit.UIViewController();
//                _gamecenterAuthViewParent.View.UserInteractionEnabled = false;
//                Globals.Graphics.UIView.AddSubview( _gamecenterAuthViewParent.View );
//                _gamecenterAuthViewParent.PresentModalViewController( viewController, true );
//            }
//            else if (error != null)
//            {
//                // TODO: Handle Error gracefully
//            }
//            else
//            {
//                GameLog.Log("Game Center Authenticated");
//            }
//        }

        public static void ReportCompleteHandler( MonoTouch.Foundation.NSError error )
        {
            if (error != null)
            {
                // TODO: Handle error gracefully
            }
        }

#endif

        protected override void Update( GameTime gameTime )
        {
            _updateLoopPerf.Begin();
            float timeDiff = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            if( !_isGameCenterShowing )
            {
                _duckyEngine.Update( timeDiff );

                GameParticles.Splash.Update( timeDiff );
                GameParticles.Smoke.Update( timeDiff );
                GameParticles.Flame.Update( timeDiff );
                GameParticles.FlameYellow.Update( timeDiff );
            }
            else
            {
#if MONOTOUCH
                if (_leaderboardViewDelegate.ShouldExit)
                {
                    _leaderboardParent.RemoveFromParentViewController();
                    _leaderboardViewDelegate = null;
                    _leaderboardView = null;
                    _leaderboardParent = null;
                    _isGameCenterShowing = false;
                }
#endif
            }

            base.Update( gameTime );
            _updateLoopPerf.End();
            _updateLoopPerf.Refresh();
            Globals.LastUpdate = gameTime;
        }

        protected override void Draw( GameTime gameTime )
        {
            _drawLoopPerf.Begin( );

            float timeDiff = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            _duckyEngine.Render( _spriteBatch );

#if ANDROID
            // Draw the outer edges
            _spriteBatch.Begin();
            _spriteBatch.DrawLine(
                Color.Black,
                new Vector2( horizontalOffset * 0.5f, -horizontalOffset * 0.5f ),
                new Vector2( horizontalOffset * 0.5f, GraphicsDevice.Viewport.Height ),
                (int)horizontalOffset,
                Matrix.Identity
                );
            _spriteBatch.DrawLine(
                Color.Black,
                new Vector2( GraphicsDevice.Viewport.Width - horizontalOffset * 0.5f, -horizontalOffset * 0.5f ),
                new Vector2( GraphicsDevice.Viewport.Width - horizontalOffset * 0.5f, GraphicsDevice.Viewport.Height ),
                (int)horizontalOffset,
                Matrix.Identity
                );
            _spriteBatch.End();
#endif

            //_spriteBatch.Begin();
            //_spriteBatch.DrawDecimal( ContentReference.Sansation, PerformanceTimer.AverageFramerate, 2, Vector2.One * 15.0f, Color.Red );
            //_spriteBatch.End();

            base.Draw(gameTime);
            _drawLoopPerf.End();

#if !WINDOWS
//            if( _isGameOver && _screenCapture == null )
//            {
//                _screenCapture = GameUtils.CaptureScreen();
//            }
#endif

            PerformanceTimer.FrameRefresh();
            _drawLoopPerf.Refresh();

            // Let the next 'frame' begin now because Draw() is throttled internally
            PerformanceTimer.FrameTick();
        }

#if ANDROID
        public void OnBackButtonPressed()
        {
//            _gameScene.OnBackButtonPressed ();
            if( _isGameStarted )
            {
                StartMenu();
            }
            else if( _duckyEngine.Count > 1 )
            {
                _duckyEngine.Pop();
            }
            else
            {
                GameSettings.ExitApp();
            }
        }

        public void OnMenuButtonPressed()
        {
//            _gameScene.OnMenuButtonPressed ();
        }

        /// <summary>
        /// Resize the game graphics when the window size changes
        /// </summary>
        void OnGraphicsComponentDeviceReset(object sender, EventArgs e)
        {
            resizeScreen ();
        }

        void resizeScreen()
        {
            float scale = 
                Math.Min( (float)GraphicsDevice.Viewport.Height / (float)ScreenHeight,
                    (float)GraphicsDevice.Viewport.Width / (float)ScreenWidth );
            // The offset used to center the drawn images on the screen
            Vector2 offset = 
                new Vector2( ( GraphicsDevice.Viewport.Width - ScreenWidth * scale ) / 2,
                    ( GraphicsDevice.Viewport.Height - ScreenHeight * scale ) / 2 );
            horizontalOffset = ( GraphicsDevice.Viewport.Width - ScreenWidth * scale ) / 2;

            Microsoft.Xna.Framework.Input.Touch.TouchPanel.DisplayToTouchScaling = Vector2.One / scale;
            Microsoft.Xna.Framework.Input.Touch.TouchPanel.DisplayToTouchOffset = -offset;

            SpriteBatchEx.ScreenSizeTransform = Matrix.CreateScale (scale) * Matrix.CreateTranslation (offset.X, offset.Y, 0.0f);
        }
#else
        void resizeScreen( float scaleFactor = 1.0f )
        {
            float scale = 
                Math.Min( (float)GraphicsDevice.Viewport.Height / (float)GameSettings.ScreenHeight,
                    (float)GraphicsDevice.Viewport.Width / (float)GameSettings.ScreenWidth );
            // The offset used to center the drawn images on the screen
            Vector2 offset =
                new Vector2( ( GraphicsDevice.Viewport.Width - GameSettings.ScreenWidth * scale ) / 2,
                    ( GraphicsDevice.Viewport.Height - GameSettings.ScreenHeight * scale ) / 2 );
            horizontalOffset = ( GraphicsDevice.Viewport.Width - GameSettings.ScreenWidth * scale ) / 2;

            SpriteBatchEx.ScreenSizeTransform = Matrix.CreateScale( scale * scaleFactor ) * Matrix.CreateTranslation( offset.X, offset.Y, 0.0f );
        }
#endif
    }
}
