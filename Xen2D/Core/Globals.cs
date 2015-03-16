using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    public static class Globals
    {
        private static GraphicsDeviceManager _graphics = null;
        public static GraphicsDeviceManager Graphics
        {
            get
            {
                if( null == _graphics )
                {
                    throw new Exception( "Trying to use uninitialized graphics device manager" );
                }
                return _graphics;
            }
            set
            {
                _graphics = value;
            }
        }

        private static ContentManager _contentManager = null;
        public static ContentManager Content
        {
            get
            {
                if( null == _contentManager )
                {
                    throw new Exception( "Trying to use uninitialized content manager" );
                }
                return _contentManager;
            }
            set
            {
                _contentManager = value;
            }
        }

        private static GraphicsDevice _graphicsDevice = null;
        public static GraphicsDevice GraphicsDevice
        {
            get
            {
                if( null == _graphicsDevice )
                {
                    _graphicsDevice = Graphics.GraphicsDevice;
                }
                return _graphicsDevice;
            }
            set
            {
                _graphicsDevice = value;
            }
        }

#if SILVERLIGHT || MONOTOUCH || ANDROID
        private static GameTime _lastUpdate = new GameTime();
#else
        private static GameTime _lastUpdate = new GameTime( new TimeSpan( 0 ), new TimeSpan( 0 ) );
#endif
        public static GameTime LastUpdate
        {
            get { return _lastUpdate; }
            set { 
                _lastUpdate = value;
                LastUpdateElapsedSeconds = (float)_lastUpdate.ElapsedGameTime.TotalSeconds;
                TotalGameTimeSeconds = (float)TotalGameTime.TotalSeconds;
                TotalGameTime = LastUpdate.TotalGameTime;
            }
        }

        public static TimeSpan TotalGameTime { get; private set; }
        public static float TotalGameTimeSeconds { get; private set; }
        public static float LastUpdateElapsedSeconds { get; private set; }
    }
}
