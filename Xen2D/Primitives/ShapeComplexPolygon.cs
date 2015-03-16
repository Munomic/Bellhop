using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    /// <summary>
    /// A complex-shape polygon that can have concave properties
    /// </summary>
    public class ShapeComplexPolygon : ShapePolygon2DBase<ShapeComplexPolygon>
    {
        public static ShapeComplexPolygon Acquire( List<Vector2> clockwisePointList )
        {
            var instance = _pool.Acquire();
            instance.Reset( clockwisePointList );
            return instance;
        }

        public ShapeComplexPolygon() : base() { }

        public override bool Contains( Vector2 point )
        {
            return ShapeUtility.ContainsForComplexPolygon( point, this );
        }
    }
}
