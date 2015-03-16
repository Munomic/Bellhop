using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    /// <summary>
    /// Reduced version of Microsoft.Xna.Framework.IDrawable.
    /// </summary>
    public interface IDrawable2D : I2DDisplayModifiers
    {
        int DrawOrder { get; set; }
        bool Visible { get; set; }
        void Draw( SpriteBatch spriteBatch, GameTime gameTime );
        void Draw( SpriteBatch spriteBatch, GameTime gameTime, Matrix transformFromWorldToCamera );
    }
}
