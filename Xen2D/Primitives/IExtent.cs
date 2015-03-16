using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    public interface IExtentTransform
    {
        float Angle { get; set; }
        Vector2 Scale { get; set; }
        Vector2 Anchor { get; set; }
        Vector2 Origin { get; set; }
        void ReAnchor( Vector2 anchor );
    }

    public interface ISpace2D
    {
        Space2DTranslation Transform { get; set; }
        ISpace2D ParentSpace { get; set; }
        Matrix TranslateTo { get; }
        Matrix TranslateFrom { get; }
    }

    public interface IExtent : IExtentTransform, ISpace2D, IPooledObject
    {
        IEvent OnChanged { get; }

        float HighestX { get; }
        float HighestY { get; }
        float LowestX { get; }
        float LowestY { get; }

        float InnerRadius { get; }
        float OuterRadius { get; }

        Vector2 ActualCenter { get; }

        void RecalculateBounds();

        bool ContainsPoint( Vector2 point );
        Vector2 FindClosestPoint( Vector2 point );
        bool Intersects( IExtent otherExtent );
        bool Intersects( ICompositeExtent other );
        bool Intersects( IPolygonExtent other );
        bool Intersects( ICircularExtent other );
        bool Intersects( CollisionMode thisCollisionMode, IExtent otherExtent, CollisionMode otherCollisionMode );
    }

    public interface IExtent<ExtentRegion> : IExtent
        where ExtentRegion : IRegion
    {
        ExtentRegion ReferenceRegion { get; }
    }
}
