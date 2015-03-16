using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    /// <summary>
    /// Utility class for adjusting screen game windows resolution and full screen state.  
    /// </summary>
    public static class ScreenUtility
    {
        public static bool InitGraphicsMode( int width, int height, bool isFullScreen )
        {
            GraphicsDeviceManager gdm = Globals.Graphics;
            
#if MONOTOUCH || ANDROID
            gdm.PreferredBackBufferWidth = width;
            gdm.PreferredBackBufferHeight = height;
            gdm.IsFullScreen = isFullScreen;
            gdm.ApplyChanges();
            return true;
#else
            if( isFullScreen == false )
            {
                if( ( width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width )
                    && ( height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height ) )
                {
                    gdm.PreferredBackBufferWidth = width;
                    gdm.PreferredBackBufferHeight = height;
                    gdm.IsFullScreen = isFullScreen;
                    gdm.ApplyChanges();
                    return true;
                }
            }
            else
            {
#if !SILVERLIGHT
                foreach( DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes )
                {
                    if( ( dm.Width == width ) && ( dm.Height == height ) )
                    {
#endif
                        gdm.PreferredBackBufferWidth = width;
                        gdm.PreferredBackBufferHeight = height;
                        gdm.IsFullScreen = isFullScreen;
                        gdm.ApplyChanges();
                        return true;
#if !SILVERLIGHT
                    }
                }
#endif
            }
            return false;
#endif
        }
        
#if !MONOTOUCH && !ANDROID
        public static DisplayMode CurrentDisplayMode
        {
            get
            {
                return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            }
        }
#endif
    }
}
