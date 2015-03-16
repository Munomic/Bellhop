using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    public abstract class PolygonExtentBase<DerivedType> : ExtentBase<DerivedType, ShapeComplexPolygon>, IPolygonExtent
        where DerivedType : ComposableObject, new()
    {
        private Vector2[] _actualVertices = new Vector2[ ShapePolygon.MaxSides ];

        public Vector2[] Vertices { get { return _actualVertices; } }

        public Vector2 Center { get { return ActualCenter; } }

        public void Reset( List<Vector2> vertices )
        {
            base.Reset();
            ReferenceRegion.Reset( vertices );
            RecalculateBounds();
        }

        public override void RecalculateBounds()
        {
            Matrix transform = TranslateFrom;

            bool firstIteration = true;

            OuterRadiusVector = VectorUtility.Zero;
            float outerRadius = 0;
            Vector2 tempOuterRadius;

            ActualCenter = Vector2.Transform( ReferenceRegion.Center, transform );

            for( int i = 0; i < NumSides; i++ )
            {
                _actualVertices[ i ] = Vector2.Transform( ReferenceRegion.Vertices[ i ], transform );
                if( firstIteration )
                {
                    LowestX = _actualVertices[ i ].X;
                    LowestY = _actualVertices[ i ].Y;
                    HighestX = _actualVertices[ i ].X;
                    HighestY = _actualVertices[ i ].Y;
                    firstIteration = false;
                }
                else
                {
                    LowestX = MathHelper.Min( LowestX, _actualVertices[ i ].X );
                    LowestY = MathHelper.Min( LowestY, _actualVertices[ i ].Y );
                    HighestX = MathHelper.Max( HighestX, _actualVertices[ i ].X );
                    HighestY = MathHelper.Max( HighestY, _actualVertices[ i ].Y );
                }

                tempOuterRadius = ( _actualVertices[ i ] - ActualCenter );
                if( tempOuterRadius.Length() > outerRadius )
                {
                    outerRadius = tempOuterRadius.Length();
                    OuterRadiusVector = tempOuterRadius;
                }
            }

            InnerRadiusVector = ShapeUtility.FindClosestPointOnPolygon( ActualCenter, _actualVertices, NumSides ) - ActualCenter;
        }

        public override Vector2 FindClosestPoint( Vector2 point )
        {
            return ShapeUtility.FindClosestPointOnPolygon( point, this );
        }

        public override bool Contains( Vector2 point )
        {
            return ShapeUtility.ContainsForComplexPolygon( point, this );
        }

        public override bool Intersects( ICompositeExtent other )
        {
            return other.Intersects( this );
        }

        public override bool Intersects( IPolygonExtent other )
        {
            return ShapeUtility.Intersects( this, other );
        }

        public override bool Intersects( ICircularExtent other )
        {
            return ShapeUtility.Intersects( this, other );
        }

        public override bool Intersects( IPolygon2D other )
        {
            return ShapeUtility.Intersects( this, other );
        }

        public override bool Intersects( ICircle2D other )
        {
            return ShapeUtility.Intersects( this, other );
        }

        protected override bool IntersectsImpl( CollisionMode thisCollisionMode, IExtent otherExtent, CollisionMode otherCollisionMode )
        {
            switch( CollisionChecker.GetCollisionInteractionType( thisCollisionMode, otherCollisionMode ) )
            {
                case CollisionInteractionType.ExtentVsExtent:
                    return ExtentIntersector.AreInIntersectionExtentVsExtent( this, otherExtent );
                case CollisionInteractionType.ExtentVsInnerCircle:
                    return ExtentIntersector.AreInIntersectionExtentVsCircle( this, otherExtent );
                case CollisionInteractionType.InnerCircleVsExtent:
                    return ExtentIntersector.AreInIntersectionCircleVsExtent( this, otherExtent );
                case CollisionInteractionType.InnerCircleVsInnerCircle:
                    return ExtentIntersector.AreInIntersectionCircleVsCircle( this, otherExtent );
                case CollisionInteractionType.ExtentVsPoint:
                case CollisionInteractionType.InnerCircleVsPoint:
                    return ExtentIntersector.AreInIntersectionExtentVsPoint( this, otherExtent );
                case CollisionInteractionType.PointVsCircle:
                case CollisionInteractionType.PointVsExtent:
                    return ExtentIntersector.AreInIntersectionExtentVsPoint( otherExtent, this );
                case CollisionInteractionType.PointVsPoint:
                case CollisionInteractionType.Invalid:
                default:
                    return false;
            }
        }
    }

    public class PolygonExtent : PolygonExtentBase<PolygonExtent>
    {
        public static PolygonExtent Acquire( List<Vector2> vertices )
        {
            var instance = Acquire();
            instance.Reset( vertices );
            return instance;
        }
    }
}