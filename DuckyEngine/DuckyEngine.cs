using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Xen2D;
using XenAspects;
using Microsoft.Xna.Framework.Media;

namespace DuckyEngine
{
    // Stage, Director
    // Actors, Props, Decorations
    // Poses
    // FX
    // Triggers, Events, Flags
    public enum DuckySceneEntityType
    {
        Actor, // Collides & Triggers Events
        Prop, // Collides. No events
        Decoration, // Doesn't collide. No events
    }

    public class DuckySceneEntity
    {
        public DuckySceneEntityType Type;
        public IExtent Bounds;
        public float Mass = 0.0f;
        public Vector2 Velocity = Vector2.Zero;
        public UInt16 CollisionGroup;
        public string Name;
        public DuckyElement Element;
        public Action<DuckySceneEntity, Vector2> OnCollision { get; set; }
        public Vector2 Position
        {
            get
            {
                return Bounds.Anchor;
            }
            set
            {
                Bounds.Anchor = value;
            }
        }

        public DuckySceneEntity(
            string name,
            DuckySceneEntityType type,
            IExtent bounds,
            float mass,
            UInt16 collisionGroup,
            DuckyElement element = null
            )
        {
            this.Name = name;
            this.Type = type;
            this.Bounds = bounds;
            this.Mass = mass;
            this.CollisionGroup = collisionGroup;
            this.Element = element;
        }

        internal static bool ProcessCollision( DuckySceneEntity A, DuckySceneEntity B )
        {
            if( A.Type != DuckySceneEntityType.Decoration && B.Type != DuckySceneEntityType.Decoration )
            {
                if( ( A.CollisionGroup & B.CollisionGroup ) > 0 )
                {
                    if( A.Bounds.Intersects( B.Bounds ) )
                    {
                        if( A.Type == DuckySceneEntityType.Actor )
                        {
                            Vector2 collisionPoint = A.Bounds.FindClosestPoint( B.Bounds.ActualCenter );
                            // Calculate resulting force and update accordingly
                            if( B.Mass <= 0.0f )
                            {
                                A.Velocity *= -1;
                            }
                            else
                            {
                                Vector2 velocityChange = B.Velocity * B.Mass / A.Mass; // B's momentum divided by A's mass results in A's delta-Velocity
                                A.Velocity += velocityChange;
                            }
                            if( A.OnCollision != null )
                            {
                                A.OnCollision( B, collisionPoint );
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class DuckySceneDirector : DuckySceneController
    {
        Dictionary<string, DuckySceneEntity> _entities = new Dictionary<string, DuckySceneEntity>();
        List<string> _entityKeys = new List<string>();
        public DuckySceneEntity this[ string entity ]
        {
            get
            {
                if( _entities.ContainsKey( entity ) )
                {
                    return _entities[ entity ];
                }
                return null;
            }
        }
        public List<string> Keys { get { return _entityKeys; } }
        public int Count { get { return _entityKeys.Count; } }
        //IViewport2D _viewport = new Viewport2D();

        public override void Initialize( DuckyScene scene )
        {
            base.Initialize( scene );
        }

        public override void Deinitialize()
        {
            base.Deinitialize();
        }

        protected void AttachEntity( string name, DuckySceneEntityType type, IExtent bounds, float mass, UInt16 collisionGroup )
        {
            _entities.Add( name, new DuckySceneEntity( name, type, bounds, mass, collisionGroup ) );
            _entityKeys.Add( name );
        }

        protected void AttachEntityFromElement( string page, string name, DuckySceneEntityType type, float mass, UInt16 collisionGroup )
        {
            _entities.Add( name, new DuckySceneEntity( name, type, _scene[ page ][ name ].Extent, mass, collisionGroup, _scene[ page ][ name ] ) );
            _entityKeys.Add( name );
        }

        public override void Update( float timeDiff )
        {
            // Calculate collisions among all entities
            if( _entityKeys.Count > 1 )
            {
                for( int i = 0; i < _entityKeys.Count; i++ )
                {
                    for( int j = 1; j < _entityKeys.Count; j++ )
                    {
                        DuckySceneEntity.ProcessCollision( _entities[ _entityKeys[ i ] ], _entities[ _entityKeys[ j ] ] );
                    }
                }
            }

            // Update Positions based on Mass & Velocity
            for( int i = 0; i < _entityKeys.Count; i++ )
            {
                DuckySceneEntity entity = _entities[ _entityKeys[ i ] ];
                if( entity.Mass > 0.0f )
                {
                    entity.Bounds.Anchor += entity.Velocity * timeDiff;
                }
            }
            base.Update( timeDiff );
        }
    }

    public class DuckyGameEngine : DuckyScreen
    {
        public override void Load( XElement scene )
        {
            base.Load( scene );
        }

        public override void Push( XElement scene )
        {
            base.Push( scene );
        }

        public override void Pop()
        {
            base.Pop();
        }

        public override void Update( float timeDiff )
        {
            base.Update( timeDiff );
        }

        public override void Render( ResizedSpriteBatch sb )
        {
            base.Render( sb );
        }
    }

    public class DuckyGame : Game
    {
        ResizedSpriteBatch _spriteBatch;
        float horizontalOffset = 0.0f;

        PerformanceTimer _updateLoopPerf;
        PerformanceTimer _drawLoopPerf;
        DuckyGameEngine _screen = null;
        int _width, _height;

        public DuckyGame( int width, int height )
            : base()
        {
            _screen = new DuckyGameEngine();
            Globals.Graphics = new GraphicsDeviceManager( this );
            Globals.Content = Content;
#if ANDROID
            Microsoft.Xna.Framework.Content.ContentManager.Load2xSize = true;
#endif

            // Extend battery life under lock.
            //InactiveSleepTime = TimeSpan.FromSeconds(1);

            _updateLoopPerf = new PerformanceTimer();
            _drawLoopPerf = new PerformanceTimer();

            _width = width;
            _height = height;

#if ANDROID
            Globals.Graphics.DeviceReset += new EventHandler<EventArgs>(OnGraphicsComponentDeviceReset);
            Window.AllowUserResizing = true;
#endif
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;

#if ANDROID
            ScreenUtility.InitGraphicsMode( _width, _height, false );
            resizeScreen ();
#else
            ScreenUtility.InitGraphicsMode( _width, _height, true );// false );
#endif

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
#if ANDROID
            _spriteBatch = new ResizedSpriteBatch( Globals.GraphicsDevice );
#else
            _spriteBatch = new ResizedSpriteBatch( Globals.GraphicsDevice );
#endif

            DuckyLogger.Log( "Loading Content" );

            _screen.BackgroundColor = Color.Black;
            //_screen.Scene.Controller = new MenuSceneController( _screen, _spriteBatch );

            base.LoadContent();

            DuckyLogger.Log( "Game Loaded" );
        }

        protected override void UnloadContent()
        {
            _screen.Unload();
            MediaPlayer.Stop();

            DuckyLogger.Log( "Game Unloaded" );
        }

        protected override void OnActivated( object sender, EventArgs args )
        {
            DuckyLogger.Log( "Game Resumed" );

            // Coming back from incoming calls, alerts, the task-switching bar being displayed, etc.

            base.OnActivated( sender, args );
        }

        protected override void OnDeactivated( object sender, EventArgs args )
        {
            DuckyLogger.Log( "Game Paused" );

            // Going out to incoming calls, alerts, the task-switching bar being displayed, etc.

            GC.Collect();
            base.OnDeactivated( sender, args );
        }

        protected override void Update( GameTime gameTime )
        {
            _updateLoopPerf.Begin();
            float timeDiff = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            _screen.Update( timeDiff );

            base.Update( gameTime );
            _updateLoopPerf.End();
            _updateLoopPerf.Refresh();
            Globals.LastUpdate = gameTime;
        }

        protected override void Draw( GameTime gameTime )
        {
            _drawLoopPerf.Begin();

            float timeDiff = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            _screen.Render( _spriteBatch );

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

            base.Draw( gameTime );
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
                _screen.Pop();
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
#endif
    }
}
