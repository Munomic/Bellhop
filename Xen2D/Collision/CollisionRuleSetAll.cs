using System;

namespace Xen2D
{
    /// <summary>
    /// A special collision rule set that returns true for any non-zero collidable class 
    /// </summary>
    public class CollisionRuleSetAll : ICollisionRuleSet
    {
        private static ICollisionRuleSet _CheckAllCollisions = new CollisionRuleSetAll();

        public static ICollisionRuleSet Instance
        {
            get { return _CheckAllCollisions; }
        }

        private CollisionRuleSetAll() { }

        public void AddRule( uint classA, uint classB )
        {
            AddRule( classA, classB, 0 );
        }

        public void AddRule( uint classA, uint classB, float cooldown )
        {
            throw new InvalidOperationException( "Cannot modify rules of this Collision Rule Set" );
        }

        public bool RemoveRule( uint classA, uint classB )
        {
            throw new InvalidOperationException( "Cannot modify rules of this Collision Rule Set" );
        }

        public bool CanCollide( uint classA, uint classB, out CollisionRuleEntry collisionRuleEntry )
        {
            collisionRuleEntry = null;
            return CanCollide( classA, classB );
        }

        public bool CanCollide( uint classA, uint classB )
        {
            return ( 0 != ( classA * classB ) );
        }

        public bool ContainsRulesFor( uint collidable )
        {
            return ( 0 != collidable );
        }
    }
}
