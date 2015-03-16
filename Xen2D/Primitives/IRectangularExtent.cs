using XenAspects;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    public interface IRectangularExtent : IPolygonExtent
    {
        void Reset( Vector2 size );
        void Reset( Vector2 size, Vector2 origin );
        void Reset( float width, float height );
        void Reset( float width, float height, Vector2 origin );
        void Reset( float topLeft, float topRight, float width, float height );
        void Reset( Vector2 anchor, float width, float height );
        void Reset( Vector2 anchor, float width, float height, Vector2 origin );

        Vector2 ReferenceTopLeft { get; }
        Vector2 ReferenceTopRight { get; }
        Vector2 ReferenceBottomLeft { get; }
        Vector2 ReferenceBottomRight { get; }

        Vector2 ActualTopLeft { get; }
        Vector2 ActualTopRight { get; }
        Vector2 ActualBottomLeft { get; }
        Vector2 ActualBottomRight { get; }

        float ReferenceWidth { get; }
        float ReferenceHeight { get; }
    }
}