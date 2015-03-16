using System;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    public interface IRectangle : IPolygon2D
    {
        float Width { get; }
        float Height { get; }
        Vector2 TopLeft { get; }
        Vector2 TopRight { get; }
        Vector2 BottomLeft { get; }
        Vector2 BottomRight { get; }
    }
}
