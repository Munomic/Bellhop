
namespace Xen2D
{
    public class CollisionRuleEntry
    {
        private uint _classA;
        private uint _classB;

        /// <summary>
        /// Returns the product of the two collision Ids.
        /// </summary>
        public uint MatchProduct
        {
            get { return _classA * _classB; }
        }

        public bool TrackLifeTime { get; private set; }

        /// <summary>
        /// Minimum time interval (in seconds) that must elapse before these two entities can collide again.  
        /// </summary>
        public float Cooldown { get; set; }

        public CollisionRuleEntry( uint classAId, uint classBId )
            : this( classAId, classBId, true ) { }

        public CollisionRuleEntry( uint classAId, uint classBId, bool trackLifeTime )
            : this( classAId, classBId, trackLifeTime, 1 ) { }

        public CollisionRuleEntry( uint classAId, uint classBId, bool trackLifeTime, float cooldown )
        {
            _classA = classAId;
            _classB = classBId;
            TrackLifeTime = trackLifeTime;
            Cooldown = cooldown;
        }

        public bool Involves( uint classAId, uint classBId )
        {
            if( MatchProduct != ( classAId * classBId ) )
                return false;

            return Involves( classAId );
        }

        public bool Involves( uint id )
        {
            return ( _classA == id ) || ( _classB == id );
        }
    }
}