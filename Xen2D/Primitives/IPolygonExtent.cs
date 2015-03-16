using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    public interface IPolygonExtent : IExtent<ShapeComplexPolygon>, IComposableObject, IPolygon2D
    {
        void Reset( List<Vector2> vertices );
    }
}