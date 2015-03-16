using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    /// <summary>
    /// A single pixel texture.  
    /// </summary>
    public class SinglePixel : ITextureInfo
    {
        private static SinglePixel _whitePixel = new SinglePixel(Color.White);
        public static SinglePixel WhitePixel
        {
            get { return _whitePixel; }
        }

        public virtual Rectangle SourceRectangle { get; protected set; }
        public virtual Texture2D Asset { get; protected set; }

        private Color[] _colorData = new Color[ 1 ];
        public Color Color
        {
            get { return _colorData[ 0 ]; }
            set
            {
                _colorData[ 0 ] = value;
#if !MONOTOUCH && !ANDROID
                Asset.SetData( _colorData );
#endif
            }
        }

        public SinglePixel() : this( Color.Black ) { }

        /// <summary>
        /// Creates a single pixel texture of the specific color.
        /// </summary>
        /// <param name="color">The color of the pixel.</param>
        public SinglePixel( Color color )
        {
            SourceRectangle = new Rectangle( 0, 0, 1, 1 );
#if MONOTOUCH || ANDROID
			Asset = Texture2D.SinglePixelFromColor( color, "SinglePixelColor" );
#else
            Asset = new Texture2D( Globals.Graphics.GraphicsDevice, 1, 1 );
#endif
            Color = color;
        }
    }
}
