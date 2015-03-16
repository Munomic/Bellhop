using System;
using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    public class ShapeCircle : ComposableObject<ShapeCircle>, ICircle2D
    {
        public static ShapeCircle Acquire( Vector2 center, float radius )
        {
            var instance = Acquire();
            instance.Reset( center, radius );
            return instance;
        }

        Vector2 _center;
        float _radius;

        public int NumSides
        {
            get { return 1; }
        }

        public ShapeCircle() : base() { }

        public void Reset( Vector2 center, float radius )
        {
            _center = center;
            _radius = radius;
        }

        /// <summary>
        /// Resets this shape to become the inner center circle of the specified extent.
        /// </summary>
        /// <param name="extent">The extent.</param>
        public void Reset( IExtent extent )
        {
            Reset( extent.ActualCenter, extent.InnerRadius );
        }

        public Vector2 Center
        {
            get { return _center; }
            set { _center = value; }
        }

        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public bool Contains( Vector2 point )
        {
            return ShapeUtility.ContainsForCircle( point, this );
        }

        #region IShapeBase Members

        public bool Intersects( ICircle2D other )
        {
            float centerDistance = ( _center - other.Center ).Length();
            float radiusSum = _radius + other.Radius;
            return ( centerDistance <= radiusSum );
        }

        public bool Intersects( IPolygon2D other )
        {
            return ShapeUtility.Intersects( other, this );
        }

        #endregion

        public Vector2 FindClosestPoint( Vector2 point )
        {
            return ShapeUtility.FindClosestPointOnCircle( point, this );
        }

        #region IEquatable<ShapeCircle> Members

        public bool Equals( ShapeCircle other )
        {
            return ( _center == other._center ) && ( _radius == other._radius );
        }

        #endregion
    }
}
