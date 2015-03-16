using System;
using XenAspects;

namespace Xen2D
{
    /// <summary>
    /// Indicates the portion of the extent that is eligible for collision
    /// </summary>
    public enum CollisionMode
    {
        //Enum              //Eligible collision region:
        None,               //None (this extent does not participate in collision)
        Extent,             //The entire extent 
        CenterPoint,        //The center point (only generates collision if it is contained in another region)
        InnerCenterCircle,  //The inner circle (smallest circle completely contained in extent with center at center point)
    }

    public interface ICollisionExtent
    {
        IExtent CollisionExtent { get; }
        CollisionMode CollisionMode { get; }
    }

    /// <summary>
    /// Interface that describes an object that can be collided
    /// </summary>
    public interface ICollidableObject : IComposableObject, ICollisionExtent
    {
        /// <summary>
        /// Gets the CollisionClass of this object.  If the collision class is 0, it means it does not participate in collision.
        /// </summary>
        uint CollisionClass { get; }
        bool Intersects( ICollidableObject collidableObject );
        bool IsCollidable { get; }
    }

    public abstract class CollidableObject<T> : ComposableObject<T>, ICollidableObject
        where T : ComposableObject, new()
    {
        private uint _collisionClass = CollisionClasses.Default;

        public IExtent CollisionExtent { get; protected set; }
        public CollisionMode CollisionMode { get; protected set; }

        public uint CollisionClass
        {
            get { return _collisionClass; }
            protected set { _collisionClass = value; }
        }

        public bool Intersects( ICollidableObject collidableObject )
        {
            if( null == collidableObject || null == CollisionExtent )
                return false;

            return CollisionExtent.Intersects( CollisionMode, collidableObject.CollisionExtent, collidableObject.CollisionMode );
        }

        public bool IsCollidable
        {
            get{ return ( CollisionClass != CollisionClasses.DoesNotCollide ) && ( CollisionExtent != null ); }
        }
    }
}