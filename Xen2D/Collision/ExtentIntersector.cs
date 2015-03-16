using System;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    public static class ExtentHelper
    {
        public static float GetCenterToCenterDistance( IExtent a, IExtent b )
        {
            return ( a.ActualCenter - b.ActualCenter ).Length();
        }
    }

    public static class ExtentIntersector
    {
        private static ShapeCircle _tempCircleA = new ShapeCircle();
        private static ShapeCircle _tempCircleB = new ShapeCircle();

        public static bool IsDistanceBetweenCentersGreaterThanOuterRadiiSum( IExtent a, IExtent b )
        {
            float distanceFromCenterToCenter = ExtentHelper.GetCenterToCenterDistance( a, b );

            return ( distanceFromCenterToCenter > ( a.OuterRadius + b.OuterRadius ) );
        }

        public static bool IsDistanceBetweenCentersLessThanOrEqualInnerRadiiSum( IExtent a, IExtent b )
        {
            float distanceFromCenterToCenter = ExtentHelper.GetCenterToCenterDistance( a, b );

            return ( distanceFromCenterToCenter <= ( a.InnerRadius + b.InnerRadius ) );
        }

        public static bool IsSimpleIntersectionCase( IExtent a, IExtent b, out bool intersects )
        {
            bool returnVal = false;
            intersects = false;
            if( IsDistanceBetweenCentersGreaterThanOuterRadiiSum( a, b ) )
            {
                returnVal = true;
                intersects = false;
            }
            else if( IsDistanceBetweenCentersLessThanOrEqualInnerRadiiSum( a, b ) )
            {
                returnVal = true;
                intersects = true;
            }

            return returnVal;
        }

        public static bool AreInIntersectionCircleVsCircle( ICircularExtent a, IExtent b )
        {
            ICircularExtent circle = b as ICircularExtent;
            if( null != circle )
                return a.Intersects( circle );

            IPolygonExtent polygon = b as IPolygonExtent;
            if( null != polygon )
            {
                _tempCircleB.Reset( polygon );
                return a.Intersects( _tempCircleB );
            }

            throw new InvalidOperationException( "Extent must be a circle or polygon" );
        }

        public static bool AreInIntersectionCircleVsCircle( IPolygonExtent a, IExtent b )
        {
            ICircularExtent circle = b as ICircularExtent;
            if( null != circle )
            {
                _tempCircleA.Reset( a );
                return circle.Intersects( _tempCircleA );
            }

            IPolygonExtent polygon = b as IPolygonExtent;
            if( null != polygon )
            {
                _tempCircleB.Reset( polygon );
                return a.Intersects( _tempCircleB );
            }

            throw new InvalidOperationException( "Extent must be a circle or polygon" );
        }

        public static bool AreInIntersectionCircleVsExtent( ICircularExtent a, IExtent b )
        {
            ICircularExtent circle = b as ICircularExtent;
            if( null != circle )
                return a.Intersects( circle );

            IPolygonExtent polygon = b as IPolygonExtent;
            if( null != polygon )
                return a.Intersects( polygon );

            throw new InvalidOperationException( "Extent must be a circle or polygon" );
        }

        public static bool AreInIntersectionCircleVsExtent( IPolygonExtent a, IExtent b )
        {
            _tempCircleA.Reset( a );
            ICircularExtent circle = b as ICircularExtent;
            if( null != circle )
                return circle.Intersects( _tempCircleA );

            IPolygonExtent polygon = b as IPolygonExtent;
            if( null != polygon )
                return polygon.Intersects( _tempCircleA );

            throw new InvalidOperationException( "Extent must be a circle or polygon" );
        }

        public static bool AreInIntersectionExtentVsPoint( IExtent a, IExtent b )
        {
            return a.ContainsPoint( b.ActualCenter );
        }

        public static bool AreInIntersectionExtentVsCircle( IPolygonExtent a, IExtent b )
        {
            _tempCircleB.Reset( b );
            return a.Intersects( _tempCircleB );
        }

        public static bool AreInIntersectionExtentVsExtent( IPolygonExtent a, IExtent b )
        {
            ICompositeExtent composite = b as ICompositeExtent;
            if( null != composite )
            {
                return a.Intersects( composite );
            }

            composite = a as ICompositeExtent;
            if( null != composite )
            {
                return b.Intersects( composite );
            }

            ICircularExtent circle = b as ICircularExtent;
            if( null != circle )
            {
                _tempCircleB.Reset( b );
                return circle.Intersects( _tempCircleB );
            }

            IPolygonExtent polygon = b as IPolygonExtent;
            if( null != polygon )
            {
                return a.Intersects( polygon );
            }

            throw new InvalidOperationException( "Extent must be a circle or polygon" );
        }
    }
}
