using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    //Example FontEnumeration definition
    //public enum Fonts : int
    //{
    //    [ContentIdentifier( "fonts\\Arial" )]
    //    Arial,
    //    [ContentIdentifier( "fonts\\TimesNewRoman" )]
    //    TimesNewRoman,
    //    [ContentIdentifier( "textures\\Courier" )]
    //    Courier
    //}

    /// <summary>
    /// This class contains a set of fonts.
    /// The FontCache should be constructed and initialized when the game/level is loaded.
    /// SpriteFont enumeration definitions must inherit from integer and provide ContentIdentifierAttributes
    /// </summary>
    public class SpriteFontCache : XenCache<SpriteFont>
    {
        // SpriteFont Cache
        public SpriteFontCache( Type contentElementEnumeration ) : this( Globals.Content, contentElementEnumeration ) { }
        public SpriteFontCache( ContentManager content, Type contentElementEnumeration ) : base( content, contentElementEnumeration ) { }
    }

    public abstract class SpriteFontCache<T> : XenCache<SpriteFont, T>
    {
        public SpriteFontCache() : base( typeof( T ) ) { }
    }
}