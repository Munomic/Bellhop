using System.Collections.Generic;
using XenAspects;

namespace Xen2D
{
    public interface ICollisionLifetimeTracker : IPooledObjectNList<ICollisionLifetimeEntry>
    {
        bool Remove( ICollidableObject a, ICollidableObject b );
        bool Contains( ICollidableObject a, ICollidableObject b );
        bool Contains( ICollidableObject a, ICollidableObject b, out ICollisionLifetimeEntry entry );
        bool Add( ICollidableObject a, ICollidableObject b, float timeOfCollision );
    }

    public class CollisionLifetimeTracker : PooledObjectNList<ICollisionLifetimeEntry>, ICollisionLifetimeTracker
    {
        public const int DefaultCollisionEntriesCapacity = 256;
        Dictionary<long, ICollisionLifetimeEntry> _entries;

        public CollisionLifetimeTracker() : this( DefaultCollisionEntriesCapacity ) { }

        public CollisionLifetimeTracker( int collisionEntryCapacity )
        {
            _entries = new Dictionary<long, ICollisionLifetimeEntry>( collisionEntryCapacity );
        }

        public bool Add( ICollidableObject a, ICollidableObject b, float timeOfCollision )
        {
            int hash = CollisionLifetimeEntry.GetCombinedHash( a, b );
            if( _entries.ContainsKey( hash ) )
            {
                //There is already a collision recorded for this entry, update the latest time at which it occured instead.  
                _entries[ hash ].TimeOfCollision = timeOfCollision;
                return true;
            }
            else
            {
                var toAdd = CollisionLifetimeEntry.Acquire( a, b, timeOfCollision );
                _entries.Add( hash, toAdd );
                return base.Add( toAdd );
            }
        }

        public override bool Remove( ICollisionLifetimeEntry toRemove )
        {
            _entries.Remove( toRemove.GetCombinedHash() );
            return base.Remove( toRemove );
        }

        public bool Remove( ICollidableObject a, ICollidableObject b )
        {
            return Remove( a.GetHashCode() ^ b.GetHashCode() );
        }

        private bool Remove( int combinedHash )
        {
            if( _entries.ContainsKey( combinedHash ) )
            {
                ICollisionLifetimeEntry toRemove = _entries[ combinedHash ];
                _entries.Remove( combinedHash );
                return base.Remove( toRemove );
            }
            return false;
        }

        /// <summary>
        /// Checks to see if this class contains a collision record for the two specified collidable objects.  
        /// </summary>
        /// <param name="a">The first collision participant.  Order does not matter.</param>
        /// <param name="b">The second collision participant.  Order does not matter.</param>
        /// <returns>True if these two entities have collided in the past, false otherwise.</returns>
        public bool Contains( ICollidableObject a, ICollidableObject b )
        {
            if( null == a || null == b )
                return false;

            return _entries.ContainsKey( a.GetHashCode() ^ b.GetHashCode() );
        }

        /// <summary>
        /// Checks to see if this class contains a collision record for the two specified collidable objects.  
        /// </summary>
        /// <param name="a">The first collision participant.  Order does not matter.</param>
        /// <param name="b">The second collision participant.  Order does not matter.</param>
        /// <param name="entry">The out parameter containing the collision record details.</param>
        /// <returns>True if these two entities have collided in the past, false otherwise.</returns>
        public bool Contains( ICollidableObject a, ICollidableObject b, out ICollisionLifetimeEntry entry )
        {
            entry = null;
            if( null == a || null == b )
                return false;

            int hashCode = a.GetHashCode() ^ b.GetHashCode();
            if( _entries.ContainsKey( hashCode ) )
            {
                entry = _entries[ hashCode ];
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
