using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    /// <summary>
    /// Base class for 2D polygons
    /// </summary>
    public abstract class ShapePolygon2DBase<DerivedType> : ComposableObject<DerivedType>, IPolygon2D
        where DerivedType : ComposableObject, new()
    {
        public static int MaxSides = 64;

        protected Vector2[] _pointList = new Vector2[ MaxSides ];
        protected Vector2 _center;
        protected Vector2 _innerRadius;
        protected Vector2 _outerRadius;
        protected IEvent _onReset = new Event();

        public int NumSides { get; set; }

        public IEvent OnReset { get { return _onReset; } }

        /// <summary>
        /// Gets the set of vertices describing this polygon.  This is a fixed size collection, so use NumSides when enumerating instead of foreach.
        /// </summary>
        public Vector2[] Vertices
        {
            get { return _pointList; }
        }

        public Vector2 Center
        { 
            get{ return _center; }
        }

        /// <summary>
        /// Gets the vector from the center to the closest point on an edge or vertex
        /// </summary>
        public Vector2 InnerRadiusVector
        {
            get { return _innerRadius; }
        }

        /// <summary>
        /// Gets the distance from the center to the closest point on an edge or vertex
        /// </summary>
        public float InnerRadius
        {
            get { return _innerRadius.Length(); }
        }

        /// <summary>
        /// Gets the vector from the center to the furthest vertex
        /// </summary>
        public Vector2 OuterRadiusVector
        {
            get { return _outerRadius; }
        }

        /// <summary>
        /// Gets the distance from the center to the furthest vertex
        /// </summary>
        public float OuterRadius
        {
            get { return _outerRadius.Length(); }
        }

        public void Reset( List<Vector2> clockwisePointList )
        {
            if( clockwisePointList.Count > MaxSides )
                throw new ArgumentException( "Too many sides" );

            if( clockwisePointList.Count < 3 )
                throw new ArgumentException( "You must have at least 3 points" );

            NumSides = clockwisePointList.Count;

            for( int i = 0; i < NumSides; i++ )
            {
                _pointList[ i ] = clockwisePointList[ i ];
            }

            ResetInternal();
        }

        public void Reset( Vector2[] clockwisePointList, int numPoints )
        {
            if( numPoints > MaxSides )
                throw new ArgumentException( "Too many sides" );

            if( numPoints < 3 )
                throw new ArgumentException( "You must have at least 3 points" );

            NumSides = numPoints;

            for( int i = 0; i < NumSides; i++ )
            {
                _pointList[ i ] = clockwisePointList[ i ];
            }

            ResetInternal();
        }

        private void ResetInternal()
        {
            _center = ShapeUtility.CalculateCenterPoint( this );
            _innerRadius = ShapeUtility.CalculateInnerRadius( this );
            _outerRadius = ShapeUtility.CalculateOuterRadius( this );

            _onReset.Notify();
        }

        public abstract bool Contains( Vector2 point );

        public Vector2 FindClosestPoint( Vector2 point )
        {
            return ShapeUtility.FindClosestPointOnPolygon( point, this );
        }

        public bool Intersects( IPolygon2D other )
        {
            return ShapeUtility.Intersects( this, other );
        }

        public bool Intersects( ICircle2D other )
        {
            return ShapeUtility.Intersects( this, other );
        }
    }
}
