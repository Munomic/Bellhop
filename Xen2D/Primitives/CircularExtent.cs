using System;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    public class CircularExtent : ExtentBase<CircularExtent, ShapeCircle>, ICircularExtent
    {
        public static CircularExtent Acquire( Vector2 center, float radius )
        {
            var instance = Acquire();
            instance.Reset( center, radius );
            return instance;
        }

        public float ReferenceRadius
        {
            get { return ReferenceRegion.Radius; }
        }

        public float Radius { get; private set; }

        public Vector2 Center
        {
            get { return ActualCenter; }
        }

        public override void RecalculateBounds()
        {
            Matrix transform = TranslateFrom;

            Vector2 pointOnEdge = ReferenceRegion.Center;
            pointOnEdge.X += ReferenceRegion.Radius;

            ActualCenter = Vector2.Transform( ReferenceRegion.Center, transform );

            pointOnEdge = Vector2.Transform( pointOnEdge, transform );

            InnerRadiusVector = pointOnEdge - ActualCenter;
            OuterRadiusVector = InnerRadiusVector;

            Radius = InnerRadiusVector.Length();

            LowestX = ActualCenter.X - Radius;
            LowestY = ActualCenter.Y - Radius;
            HighestX = ActualCenter.X + Radius;
            HighestY = ActualCenter.Y + Radius;
        }


        protected override bool IntersectsImpl( CollisionMode thisCollisionMode, IExtent otherExtent, CollisionMode otherCollisionMode )
        {
            switch( CollisionChecker.GetCollisionInteractionType( thisCollisionMode, otherCollisionMode ) )
            {
                case CollisionInteractionType.ExtentVsInnerCircle:
                case CollisionInteractionType.InnerCircleVsInnerCircle:
                    return ExtentIntersector.AreInIntersectionCircleVsCircle( this, otherExtent );
                case CollisionInteractionType.ExtentVsExtent:
                case CollisionInteractionType.InnerCircleVsExtent:
                    return ExtentIntersector.AreInIntersectionCircleVsExtent( this, otherExtent );
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

        public override bool Intersects( ICompositeExtent other )
        {
            return other.Intersects( this );
        }

        public override bool Intersects( IPolygonExtent other )
        {
            return ShapeUtility.Intersects( other, this );
        }

        public override bool Intersects( ICircularExtent other )
        {
            return ShapeUtility.Intersects( other, this );
        }

        public override bool Intersects( IPolygon2D other )
        {
            return ShapeUtility.Intersects( other, this );
        }

        public override bool Intersects( ICircle2D other )
        {
            return ShapeUtility.Intersects( other, this );
        }

        public override Vector2 FindClosestPoint( Vector2 point )
        {
            return ShapeUtility.FindClosestPointOnCircle( point, this );
        }

        public override bool Contains( Vector2 point )
        {
            return ShapeUtility.ContainsForCircle( point, this );
        }

        public void Reset( Vector2 center, float radius )
        {
            Reset( center, radius, center );
        }

        public virtual void Reset( Vector2 center, float radius, Vector2 origin )
        {
            base.Reset();

            ReferenceRegion.Reset( center, radius );
            Origin = origin;
            Anchor = center;
            RecalculateBounds();
        }

        protected override void BeforeSetScaleTemplate( Vector2 scale )
        {
            //Scaling a circle by different X and Y would cause it to stop being a circle.
            if( scale.X != scale.Y )
                throw new InvalidOperationException( "Cannot set scale vector on circle with different X and Y values" );

            base.BeforeSetScaleTemplate( scale );
        }
    }
}