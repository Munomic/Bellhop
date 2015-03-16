using XenAspects;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    public interface ICircularExtent : IExtent<ShapeCircle>, IComposableObject, ICircle2D
    {
        void Reset( Vector2 center, float radius );
        void Reset( Vector2 center, float radius, Vector2 origin );

        float ReferenceRadius { get; }
    }
}