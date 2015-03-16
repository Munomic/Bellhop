using XenAspects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Xen2D
{
    public sealed class PlaceholderRenderable2D : Renderable2DBase<PlaceholderRenderable2D>
    {
        public static PlaceholderRenderable2D Instance = new PlaceholderRenderable2D();
        protected override void DrawInternal( SpriteBatch spriteBatch, GameTime gameTime, Matrix transformFromWorldToCamera ) { }

        public PlaceholderRenderable2D()
        {
            Reset();
        }
    }
}
