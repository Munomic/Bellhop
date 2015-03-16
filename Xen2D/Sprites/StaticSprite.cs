using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    /// <summary>
    /// This class represents a static sprite that consists of a single texture.  
    /// </summary>
    /// <typeparam name="T">The parameterized type.</typeparam>
    public class StaticSprite : Sprite2D<StaticSprite>
    {
        public static StaticSprite Acquire( Texture2D texture )
        {
            return Acquire( texture, Vector2.Zero );
        }

        public static StaticSprite AcquireAndCenter( Texture2D texture )
        {
            return Acquire( texture, new Vector2( texture.Width / 2, texture.Height / 2 ) );
        }

        public static StaticSprite Acquire( Texture2D texture, Vector2 origin )
        {
            Debug.Assert( null != texture );
            var sprite = Acquire();
            sprite.Reset( texture );
            sprite.RenderingExtent.ReAnchor( origin );
            return sprite;
        }

        protected CachedTextureDescriptor TextureDescriptor { get; set; }

        public StaticSprite() 
        {
            TextureDescriptor = new CachedTextureDescriptor();
            TextureInfo = TextureDescriptor;
        }

        //TODO: what does it mean to recycle instances of StaticSprite?  What happens to the texture?
        protected override void ResetDirectState()
        {
            base.ResetDirectState();
            RenderingExtent.Reset();
        }

        public void Reset( Texture2D texture )
        {
            TextureDescriptor.Reset( texture );
            RenderingExtent.Reset( TextureDescriptor.SourceRectangle.Width, TextureDescriptor.SourceRectangle.Height );
        }
    }
}
