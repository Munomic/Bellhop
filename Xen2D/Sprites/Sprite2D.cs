using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XenAspects;

namespace Xen2D
{
    /// <summary>
    /// This base class represents a sprite, which consists of a texture or animation that can draw itself
    /// at a particular location on the screen.
    /// </summary>
    /// <typeparam name="T">The parameterized type.</typeparam>
    public abstract class Sprite2D<T> : Renderable2DBase<T>, ISprite
        where T : ComposableObject, new()
    {
        public virtual ITextureInfo TextureInfo { get; protected set; }

        protected override void DrawInternal( SpriteBatch spriteBatch, GameTime gameTime, Matrix transformFromWorldToCamera )
        {
            spriteBatch.DrawSprite( this, transformFromWorldToCamera );
        }
    }
}
