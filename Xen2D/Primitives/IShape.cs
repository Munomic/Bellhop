using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Xen2D
{
    /// <summary>
    /// Region represents a path or a set composite of shapes (maybe intersection or union of shapes?)
    /// </summary>
    public interface IRegion
    {
        bool Contains( Vector2 point );
        Vector2 FindClosestPoint( Vector2 point );
    }

    public interface IShape : IRegion
    {
        int NumSides { get; }
        Vector2 Center { get; }

        bool Intersects( IPolygon2D other );
        bool Intersects( ICircle2D other );
    }

    public interface IPolygon2D : IShape
    {
        Vector2[] Vertices { get; }
    }

    public interface ICircle2D : IShape
    {
        float Radius { get; }
    }
}
