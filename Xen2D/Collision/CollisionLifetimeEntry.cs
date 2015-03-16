using System;
using XenAspects;

namespace Xen2D
{
    public interface ICollisionLifetimeEntry : IPooledObject
    {
        int GetCombinedHash();
        float TimeOfCollision { get; set; }
        bool CanCollideAgain( float cooldown, float currentGameTime );
    }

    /// <summary>
    /// This class is responsible for recording the fact that two entities have collided.  
    /// </summary>
    public sealed class CollisionLifetimeEntry : PooledObject<CollisionLifetimeEntry>, ICollisionLifetimeEntry
    {
        public static int InvalidCombinedHash = -1;

        public static int GetCombinedHash( ICollidableObject a, ICollidableObject b )
        {
            if( null == a || null == b )
                return InvalidCombinedHash;

            return ( a.GetHashCode() ^ b.GetHashCode() );
        }

        public static CollisionLifetimeEntry Acquire( ICollidableObject a, ICollidableObject b, float timeOfCollision )
        {
            var instance = Acquire();
            instance.Reset( a, b, timeOfCollision );
            return instance;
        }

        private ICollidableObject _a;
        private ICollidableObject _b;
        private Action<IPooledObject> _onTrackedCollidablesReleased = null;

        public float TimeOfCollision { get; set; }

        public bool CanCollideAgain( float cooldown, float currentGameTime )
        {
            return ( ( currentGameTime - TimeOfCollision ) > cooldown );
        }

        public CollisionLifetimeEntry()
        {
            _onTrackedCollidablesReleased = new Action<IPooledObject>( OnTrackedCollidableReleased );
        }

        protected override void ReleaseInternal()
        {
            Detach();
            base.ReleaseInternal();
        }

        private void Reset( ICollidableObject a, ICollidableObject b, float timeOfCollision )
        {
            Attach( a, b );
            TimeOfCollision = timeOfCollision;
        }

        private void Detach()
        {
            _a.OnReleased.Remove( _onTrackedCollidablesReleased );
            _b.OnReleased.Remove( _onTrackedCollidablesReleased );

            _a = null;
            _b = null;
        }

        private void Attach( ICollidableObject a, ICollidableObject b )
        {
            if( null == a )
                throw new ArgumentNullException( "a" );

            if( null == b )
                throw new ArgumentNullException( "b" );

            _a = a;
            _b = b;

            _a.OnReleased.Add( _onTrackedCollidablesReleased );
            _b.OnReleased.Add( _onTrackedCollidablesReleased );
        }

        private void OnTrackedCollidableReleased( IPooledObject subject )
        {
            //Whenever either of the tracked collidable is released, also release its entry.
            Release();
        }

        public int GetCombinedHash()
        {
            return GetCombinedHash( _a, _b );
        }
    }
}
