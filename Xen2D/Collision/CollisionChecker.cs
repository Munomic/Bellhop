
namespace Xen2D
{
    /// <summary>
    /// This class is responsible for checking collision between two given ICollisionProperties
    /// </summary>
    public static class CollisionChecker
    {
        public static CollisionInteractionType GetCollisionInteractionType( ICollidableObject a, ICollidableObject b )
        {
            CollisionInteractionType type = CollisionInteractionType.Invalid;

            if( ( null == a ) || ( null == b ) )
                return type;

            if( a == b )
                return type;

            return GetCollisionInteractionType( a.CollisionMode, b.CollisionMode );
        }

        public static CollisionInteractionType GetCollisionInteractionType( CollisionMode a, CollisionMode b )
        {
            CollisionInteractionType type = CollisionInteractionType.Invalid;

            switch( a )
            {
                case CollisionMode.CenterPoint:
                    switch( b )
                    {
                        case CollisionMode.CenterPoint:
                            type = CollisionInteractionType.PointVsPoint;
                            break;
                        case CollisionMode.InnerCenterCircle:
                            type = CollisionInteractionType.PointVsCircle;
                            break;
                        case CollisionMode.Extent:
                            type = CollisionInteractionType.PointVsExtent;
                            break;
                        default:
                            break;
                    }
                    break;

                case CollisionMode.InnerCenterCircle:
                    switch( b )
                    {
                        case CollisionMode.CenterPoint:
                            type = CollisionInteractionType.InnerCircleVsPoint;
                            break;
                        case CollisionMode.InnerCenterCircle:
                            type = CollisionInteractionType.InnerCircleVsInnerCircle;
                            break;
                        case CollisionMode.Extent:
                            type = CollisionInteractionType.InnerCircleVsExtent;
                            break;
                        default:
                            break;
                    }
                    break;

                case CollisionMode.Extent:
                    switch( b )
                    {
                        case CollisionMode.CenterPoint:
                            type = CollisionInteractionType.ExtentVsPoint;
                            break;
                        case CollisionMode.InnerCenterCircle:
                            type = CollisionInteractionType.ExtentVsInnerCircle;
                            break;
                        case CollisionMode.Extent:
                            type = CollisionInteractionType.ExtentVsExtent;
                            break;
                        default:
                            break;
                    }
                    break;

                default:
                    type = CollisionInteractionType.Invalid;
                    break;
            }

            return type;
        }

        public static bool AreInCollision( ICollidableObject a, ICollidableObject b )
        {
            if( ( null == a ) || ( null == b ) )
                return false;

            if( ( !a.IsCollidable ) || ( !b.IsCollidable ) )
                return false;

            CollisionInteractionType collisionType = GetCollisionInteractionType( a, b );
            if( collisionType == CollisionInteractionType.Invalid )
                return false;

            return a.Intersects( b );
        }
    }
}