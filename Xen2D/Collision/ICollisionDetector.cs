using System.Collections.Generic;
using XenAspects;

namespace Xen2D
{
    public interface ICollisionDetector
    {
        /// <summary>
        /// The set of collision rules that defines which types of entities can collide with one another
        /// </summary>
        ICollisionRuleSet CollisionRules { get; set; }

        /// <summary>
        /// The collection of objects to check collision amongst.
        /// </summary>
        PooledObjectNList<ICollidableObject> CollisionObjects { get; }

        /// <summary>
        /// Checks for collisions amongst the specified collision objects.
        /// </summary>
        void CheckCollisions();

        /// <summary>
        /// Event that gets fired whenever a collision is detected.  
        /// </summary>
        Event<CollisionEventArgs> OnCollision { get; }
    }
}


