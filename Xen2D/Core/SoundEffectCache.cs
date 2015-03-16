using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Xen2D
{
    //Example SongEnumeration definition
    //public enum Sounds : int
    //{
    //    [ContentIdentifier( "SFX\\bounce" )]
    //    Bounce,
    //    [ContentIdentifier( "SFX\\shoot" )]
    //    Shoot,
    //    [ContentIdentifier( "SFX\\click" )]
    //    Click
    //}

    /// <summary>
    /// This class contains a set of songs.
    /// The SoundEffectCache should be constructed and initialized when the game/level is loaded.
    /// SoundEffect enumeration definitions must inherit from integer and provide ContentIdentifierAttributes
    /// </summary>
    public class SoundEffectCache : XenCache<SoundEffect>
    {
        // SoundEffect Cache
        public SoundEffectCache( Type contentElementEnumeration ) : this( Globals.Content, contentElementEnumeration ) { }
        public SoundEffectCache( ContentManager content, Type contentElementEnumeration ) : base( content, contentElementEnumeration ) { }
    }

    public abstract class SoundEffectCache<T> : XenCache<SoundEffect, T>
    {
        public SoundEffectCache() : base( typeof( T ) ) { }
    }
}