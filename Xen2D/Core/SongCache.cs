using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Xen2D
{
    //Example SongEnumeration definition
    //public enum Songs : int
    //{
    //    [ContentIdentifier( "music\\song1" )]
    //    BackgroundMusic,
    //    [ContentIdentifier( "music\\song2" )]
    //    Battle,
    //    [ContentIdentifier( "music\\song3" )]
    //    Credits
    //}

    /// <summary>
    /// This class contains a set of songs.
    /// The SongCache should be constructed and initialized when the game/level is loaded.
    /// Song enumeration definitions must inherit from integer and provide ContentIdentifierAttributes
    /// </summary>
    public class SongCache : XenCache<Song>
    {
        // Song Cache
        public SongCache( Type contentElementEnumeration ) : this( Globals.Content, contentElementEnumeration ) { }
        public SongCache( ContentManager content, Type contentElementEnumeration ) : base( content, contentElementEnumeration ) { }
    }

    public abstract class SongCache<T> : XenCache<Song, T>
    {
        public SongCache() : base( typeof( T ) ) { }
    }
}