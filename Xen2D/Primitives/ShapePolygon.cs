using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    public class ShapePolygon : ShapePolygon2DBase<ShapePolygon>
    {
        //public static int MaxSides = 16;

        public static ShapePolygon Acquire( List<Vector2> clockwisePointList )
        {
            var instance = _pool.Acquire();
            instance.Reset( clockwisePointList );
            return instance;
        }

        public ShapePolygon() : base() { }

        public override bool Contains( Vector2 point )
        {
            return ShapeUtility.ContainsForSimplePolygon( point, this );
        }
    }
}
