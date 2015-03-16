using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    public static class ShapeUtility
    {
        /// <summary>
        /// Calculates the point on an edge closest to another point
        /// </summary>
        /// <param name="point">Focus point</param>
        /// <param name="edgeStart">Start of edge</param>
        /// <param name="edgeEnd">End of edge</param>
        /// <returns>Closest point on the edge</returns>
        public static Vector2 CalculateClosestPointOnEdge( Vector2 point, Vector2 edgeStart, Vector2 edgeEnd )
        {
            Vector2 edgeVector = edgeEnd - edgeStart;
            Vector2 pointVector = point - edgeStart;
            float projectionAmount = Vector2.Dot( pointVector, edgeVector );
            projectionAmount /= edgeVector.LengthSquared(); // Amount of edgeVector that is the projection
            if( projectionAmount <= 0.0f )
            {
                // Outside of the vector bounds
                return edgeStart;
            }
            else if( projectionAmount >= 1.0f )
            {
                return edgeEnd;
            }
            return projectionAmount * edgeVector + edgeStart;
        }

        /// <summary>
        /// Calculates the average of all vertices on a polygon
        /// </summary>
        /// <param name="shape">The polygon.</param>
        /// <returns>The center point of the polygon.</returns>
        public static Vector2 CalculateCenterPoint( IPolygon2D shape )
        {
            Vector2 pointSum = VectorUtility.Zero;
            for( int i = 0; i < shape.NumSides; i++ )
            {
                pointSum += shape.Vertices[ i ];
            }
            Vector2 pointAverage = pointSum / shape.NumSides;
            return pointAverage;
        }

        /// <summary>
        /// Calculates the vector pointing to the nearest point on the polygon if the center is at 0,0.
        /// </summary>
        /// <param name="shape">The polygon.</param>
        /// <returns>The vector pointing to the nearest point on the polygon if the center is at 0,0.</returns>
        public static Vector2 CalculateInnerRadius( IPolygon2D shape )
        {
            return shape.FindClosestPoint( shape.Center ) - shape.Center;
        }

        /// <summary>
        /// Calculates the vector pointing to the furthest vertex of the specified polygon if the polygon is centered at 0,0.
        /// </summary>
        /// <param name="shape">The polygon.</param>
        /// <returns>The vector pointing to the furthest vertex of the specified polygon if the polygon is centered at 0,0.</returns>
        public static Vector2 CalculateOuterRadius( IPolygon2D shape )
        {
            Vector2 max = VectorUtility.Zero;
            Vector2 vector;
            for( int i = 0; i < shape.NumSides; i++ )
            {
                vector = shape.Vertices[ i ] - shape.Center;
                if( vector.Length() > max.Length() )
                    max = vector;
            }

            return max;
        }

        /// <summary>
        /// Finds the closest on the on specified circle.
        /// </summary>
        /// <param name="point">The point to which we want to find the closest point on the circle.</param>
        /// <param name="circle">The circle on which to find the closest point.</param>
        /// <returns>The point on the circle that is closest to the specified point.</returns>
        public static Vector2 FindClosestPointOnCircle( Vector2 point, ICircle2D circle )
        {
            Vector2 vector = point - circle.Center;
            vector.Normalize();
            vector *= circle.Radius;
            return circle.Center + vector;
        }

        /// <summary>
        /// Finds the closets point on the polygon to the specified point.
        /// </summary>
        /// <param name="point">The point to find the closest point to.</param>
        /// <param name="polygon">The polygon on which to find the closets point.</param>
        /// <returns>The point on the polygon that is closest to the specified point.</returns>
        public static Vector2 FindClosestPointOnPolygon( Vector2 point, IPolygon2D polygon )
        {
            return FindClosestPointOnPolygon( point, polygon.Vertices, polygon.NumSides );
        }

        /// <summary>
        /// Finds the closets point on the polygon to the specified point.
        /// </summary>
        /// <param name="point">The point to find the closest point to.</param>
        /// <param name="vertices">The vertices describing the polygon.</param>
        /// <param name="numVertices">The number of actual vertices in the vertices array.</param>
        /// <returns>The point on the polygon that is closest to the specified point.</returns>
        public static Vector2 FindClosestPointOnPolygon( Vector2 point, Vector2[] vertices, int numVertices )
        {
            // Find the closest point
            Vector2 closestPoint = ShapeUtility.CalculateClosestPointOnEdge( point, vertices[ 0 ], vertices[ 1 ] );
            float closestDistance = ( closestPoint - point ).LengthSquared();
            for( int i = 1; i < numVertices; i++ )
            {
                Vector2 closestPointOfEdge =
                    ( i == numVertices - 1 ?
                        ShapeUtility.CalculateClosestPointOnEdge( point, vertices[ i ], vertices[ 0 ] ) :
                        ShapeUtility.CalculateClosestPointOnEdge( point, vertices[ i ], vertices[ i + 1 ] ) );
                float currentDistance = ( closestPointOfEdge - point ).LengthSquared();
                if( currentDistance < closestDistance )
                {
                    closestPoint = closestPointOfEdge;
                    closestDistance = currentDistance;
                }
            }
            return closestPoint;
        }

        public static bool AreEdgesIntersecting( Vector2 edgeAStart, Vector2 edgeAEnd, Vector2 edgeBStart, Vector2 edgeBEnd )
        {
            Vector3 AStart1 = new Vector3( edgeBStart - edgeAStart, 0.0f ),
                    AStart2 = new Vector3( edgeBEnd - edgeAStart, 0.0f ),
                    AEnd1 = new Vector3( edgeBStart - edgeAEnd, 0.0f ),
                    AEnd2 = new Vector3( edgeBEnd - edgeAEnd, 0.0f );

            // If the X-Product is in opposite directions, they're intersecting!
            bool AStartDirection = ( Vector3.Cross( AStart2, AStart1 ).Z > 0.0f );
            bool AEndDirection = ( Vector3.Cross( AEnd2, AEnd1 ).Z > 0.0f );

            return AStartDirection != AEndDirection;
        }

        public static Vector2? FindYIntersectionPoint( Vector2 edgeStart, Vector2 edgeEnd, float value )
        {
            // Edge is categorized to "on-or-above" and "below" for the edge cases
            if( ( edgeStart.Y <= value && value < edgeEnd.Y ) ||
                ( edgeEnd.Y <= value && value < edgeStart.Y ) )
            {
                float interpValue = (value - edgeStart.Y) / (edgeEnd.Y - edgeStart.Y);
                return Vector2.Lerp( edgeStart, edgeEnd, interpValue );
            }

            return null;
        }

        /// <summary>
        /// Checks to see whether the specified point lies in the specified simple polygon.  Simple polygon here 
        /// means that the polygon is convex.  
        /// </summary>
        /// <param name="point">The point to check containment for.</param>
        /// <param name="polygon">The polygon to check containment for.</param>
        /// <returns>True if the point lies within the polygon, false otherwise.</returns>
        public static bool ContainsForSimplePolygon( Vector2 point, IPolygon2D polygon )
        {
            return ContainsForSimplePolygon( point, polygon.Vertices, polygon.NumSides );
        }

        /// <summary>
        /// Checks to see whether the specified point lies in the specified simple polygon.  Simple polygon here 
        /// means that the polygon is convex.  
        /// </summary>
        /// <param name="point">The point to check containment for.</param>
        /// <param name="vertices">The vertices describing the polygon in clockwise order.</param>
        /// <param name="numVertices">The number of vertices in the vertex collection.</param>
        /// <returns>True if the point lies within the polygon, false otherwise.</returns>
        public static bool ContainsForSimplePolygon( Vector2 point, Vector2[] vertices, int numVertices )
        {
            Vector3 checkPoint = new Vector3( point.X, point.Y, 0.0f );
            for( int i = 0; i < numVertices; i++ )
            {
                Vector3 nextPoint =
                    ( i == numVertices - 1 ?
                        new Vector3( vertices[ 0 ].X, vertices[ 0 ].Y, 0.0f ) :
                        new Vector3( vertices[ i + 1 ].X, vertices[ i + 1 ].Y, 0.0f ) );
                Vector3 currentPoint = new Vector3( vertices[ i ].X, vertices[ i ].Y, 0.0f );
                Vector3 polygonEdge = nextPoint - currentPoint;
                Vector3 pointVector = checkPoint - currentPoint;
                // Check that the point is on the inside of the polygon
                //   (i.e. Cross-Product of pointVector x polygonEdge points away from screen)
                if( Vector3.Cross( pointVector, polygonEdge ).Z < 0.0f )
                {
                    return false;
                }
            }
            return true; // Point was on the inside of all edges
        }

        /// <summary>
        /// Checks to see if the specified point is contained in the specified circle.
        /// </summary>
        /// <param name="point">The point to check containment for.</param>
        /// <param name="circle">The circle to check if the point lies within.</param>
        /// <returns>True if the specified point is within the circle.  False otherwise.</returns>
        public static bool ContainsForCircle( Vector2 point, ICircle2D circle )
        {
            float centerDistanceSquared = ( circle.Center - point ).LengthSquared();
            return ( centerDistanceSquared <= circle.Radius * circle.Radius );
        }

        /// <summary>
        /// Checks to see whether the specified point lies in the specified complex polygon.  Complex polygon here 
        /// means that the polygon may contain convex vertices.  
        /// </summary>
        /// <param name="point">The point to check containment for.</param>
        /// <param name="polygon">The polygon to check containment for.</param>
        /// <returns>True if the point lies within the polygon, false otherwise.</returns>
        public static bool ContainsForComplexPolygon( Vector2 point, IPolygon2D polygon )
        {
            return ContainsForComplexPolygon( point, polygon.Vertices, polygon.NumSides );
        }

        /// <summary>
        /// Checks to see whether the specified point lies in the specified complex polygon.  Complex polygon here 
        /// means that the polygon may contain convex vertices.  
        /// </summary>
        /// <param name="point">The point to check containment for.</param>
        /// <param name="vertices">The vertices describing the polygon in clockwise order.</param>
        /// <param name="numVertices">The number of vertices in the vertex collection.</param>
        /// <returns>True if the point lies within the polygon, false otherwise.</returns>
        public static bool ContainsForComplexPolygon( Vector2 point, Vector2[] vertices, int numVertices )
        {
            //Vector3 checkPoint = new Vector3( point.X, point.Y, 0.0f );
            int leftSide = 0, rightSide = 0;
            for( int i = 0; i < numVertices; i++ )
            {
                int nextIndex = ( i + 1 ) % numVertices;
                Vector2? intersection = ShapeUtility.FindYIntersectionPoint( vertices[ i ], vertices[ nextIndex ], point.Y );
                if( intersection != null )
                {
                    if( intersection.Value.X < point.X )
                    {
                        leftSide++;
                    }
                    else
                    {
                        rightSide++;
                    }
                }
            }

            // The point is inside if both the number of intersecting points on the left and right sides of the point are odd
            return ( ( ( leftSide & 1 ) == 1 ) && ( ( rightSide & 1 ) == 1 ) );
        }

        /// <summary>
        /// Checks to see if two polygons intersect
        /// </summary>
        /// <param name="a">The first polygon.</param>
        /// <param name="b">The second polygon.</param>
        /// <returns>True if the two polygons intersect.  False otherwise.</returns>
        public static bool Intersects( IPolygon2D a, IPolygon2D b )
        {
            // See if this polygon's points are contained in the other polygon
            for( int i = 0; i < a.NumSides; i++ )
            {
                if( b.Contains( a.Vertices[ i ] ) )
                    return true;
            }
            // See if this polygon contains the other polygon's points
            for( int i = 0; i < b.NumSides; i++ )
            {
                if( a.Contains( b.Vertices[ i ] ) )
                    return true;
            }
            // See if any of the polygon edges intersect each other
            for( int i = 0; i < a.NumSides; i++ )
            {
                Vector2 edgeStart = a.Vertices[ i ];
                Vector2 edgeEnd = ( i == a.NumSides - 1 ? a.Vertices[ 0 ] : a.Vertices[ i + 1 ] );
                for( int j = 0; j < b.NumSides; j++ )
                {
                    Vector2 otherEdgeStart = a.Vertices[ j ];
                    Vector2 otherEdgeEnd = ( j == a.NumSides - 1 ? a.Vertices[ 0 ] : a.Vertices[ j + 1 ] );
                    if( ShapeUtility.AreEdgesIntersecting( edgeStart, edgeEnd, otherEdgeStart, otherEdgeEnd ) )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks for intersection between two circles.
        /// </summary>
        /// <param name="a">The first circle.</param>
        /// <param name="b">The second circle.</param>
        /// <returns>True if the two specified circles intersect or if one is entirely contained in the other.  False otherwise.</returns>
        public static bool Intersects( ICircle2D a, ICircle2D b )
        {
            float centerDistanceSquared = ( a.Center - b.Center ).LengthSquared();
            float radiusSum = a.Radius + b.Radius;
            return ( centerDistanceSquared <= radiusSum * radiusSum );
        }

        /// <summary>
        /// Checks for intersection between a polygon and a circle.
        /// </summary>
        /// <param name="polygon">The circle.</param>
        /// <param name="circle">The polygon.</param>
        /// <returns>True if the two shapes intersect.  False othwerise.</returns>
        public static bool Intersects( IPolygon2D polygon, ICircle2D circle )
        {
            // See if the closest point of each edge to the circle's center is inside the center
            Vector2 circleCenter = circle.Center;
            for( int i = 0; i < polygon.NumSides; i++ )
            {
                Vector2 closestPointOfEdge =
                    ( i == polygon.NumSides - 1 ?
                        ShapeUtility.CalculateClosestPointOnEdge( circleCenter, polygon.Vertices[ i ], polygon.Vertices[ 0 ] ) :
                        ShapeUtility.CalculateClosestPointOnEdge( circleCenter, polygon.Vertices[ i ], polygon.Vertices[ i + 1 ] ) );
                if( circle.Contains( closestPointOfEdge ) )
                {
                    return true;
                }
            }

            // See if this polygon contains the other circle
            if( polygon.Contains( circle.Center ) )
            {
                return true;
            }
            return false;
        }
    }
}
