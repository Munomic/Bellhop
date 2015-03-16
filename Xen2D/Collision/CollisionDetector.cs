using System.Collections.Generic;
using XenAspects;

namespace Xen2D
{
    /// <summary>
    /// Base class for collision detectors.
    /// </summary>
    public abstract class CollisionDetector : ICollisionDetector
    {
        private ICollisionLifetimeTracker _collisionLifetimes = new CollisionLifetimeTracker();
        private PooledObjectNList<ICollidableObject> _collisionObjects = new PooledObjectNList<ICollidableObject>();
        private Event<CollisionEventArgs> _onCollision = new Event<CollisionEventArgs>();

        /// <summary>
        /// The set of collision rules that defines which types of entities can collide with one another
        /// </summary>
        public ICollisionRuleSet CollisionRules { get; set; }

        /// <summary>
        /// The collection of objects to check collision amongst.
        /// </summary>
        public PooledObjectNList<ICollidableObject> CollisionObjects { get { return _collisionObjects; } }

        /// <summary>
        /// Event that gets fired whenever a collision is detected.  
        /// </summary>
        public Event<CollisionEventArgs> OnCollision { get { return _onCollision; } }

        public CollisionDetector()
        {
            CollisionRules = CollisionRuleSetAll.Instance;
        }

        /// <summary>
        /// Checks for collisions amongst the specified collision objects.
        /// </summary>
        public void CheckCollisions()
        {
            if( null == CollisionRules )
                return;

            CheckCollisionsInternal();
        }

        public abstract void CheckCollisionsInternal();

        protected void CheckCollisionFor( ICollidableObject primary, ICollidableObject secondary )
        {
            CollisionRuleEntry collisionRule;

            if( CollisionRules.CanCollide( primary.CollisionClass, secondary.CollisionClass, out collisionRule ) )
            {
                ICollisionLifetimeEntry collisionEntry = null;
                //Check the collision lifetime tracker.  If there is already a collision lifetime entry being tracked, do nothing and return.
                if( _collisionLifetimes.Contains( primary, secondary, out collisionEntry ) )
                {
                    //The specified entities have collided before.  Check to see if this collision time is last the cooldown period
                    if( ( collisionRule != null ) &&
                        !collisionEntry.CanCollideAgain( collisionRule.Cooldown, Globals.TotalGameTimeSeconds ) )
                    {
                        return;
                    }
                }

                if( CollisionChecker.AreInCollision( primary, secondary ) )
                {
                    if( collisionRule == null || collisionRule.TrackLifeTime )
                    {
                        _collisionLifetimes.Add( primary, secondary, Globals.TotalGameTimeSeconds );
                    }
                    _onCollision.Notify( new CollisionEventArgs( primary, secondary ) );
                }
            }
        }
    }
}