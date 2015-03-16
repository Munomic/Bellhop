using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    public interface ITextureInfo
    {
        Texture2D Asset { get; }
        Rectangle SourceRectangle { get; }
    }
}
