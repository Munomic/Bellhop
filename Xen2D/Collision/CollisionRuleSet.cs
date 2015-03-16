using System.Collections.Generic;

namespace Xen2D
{
    /// <summary>
    /// Describes a single collision rule, stating that two collision classes can collide.  If a collision class is not
    /// defined in at least one collision rule, then it will never participate in collision.  
    /// 
    /// Note: When specifying collision rules, use fewer collision classes for performance reasons.
    /// </summary>
    public class CollisionRuleSet : ICollisionRuleSet
    {
        List<CollisionRuleEntry> _rules = new List<CollisionRuleEntry>();

        public void AddRule( uint classA, uint classB )
        {
            AddRule( classA, classB, 1 );
        }

        public void AddRule( uint classA, uint classB, float cooldown )
        {
            AddRule( classA, classB, true, cooldown );
        }

        public void AddRule( uint classA, uint classB, bool trackLifeTime, float cooldown )
        {
            bool add = true;
            foreach( CollisionRuleEntry rule in _rules )
            {
                if( rule.Involves( classA, classB ) )
                {
                    //entry already exists, don't need to add it again.
                    add = false;
                }
            }
            if( add )
            {
                _rules.Add( new CollisionRuleEntry( classA, classB, trackLifeTime, cooldown ) );
            }
        }

        public bool RemoveRule( uint classA, uint classB )
        {
            CollisionRuleEntry toRemove = null;
            foreach( CollisionRuleEntry rule in _rules )
            {
                if( rule.Involves( classA, classB ) )
                {
                    toRemove = rule;
                }
            }
            if( null != toRemove )
            {
                _rules.Remove( toRemove );
                return true;
            }
            return false;
        }

        public bool CanCollide( uint classA, uint classB )
        {
            foreach( CollisionRuleEntry rule in _rules )
            {
                if( rule.Involves( classA, classB ) )
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanCollide( uint classA, uint classB, out CollisionRuleEntry collisionRule )
        {
            collisionRule = null;

            if( ( 0 == classA ) || ( 0 == classB ) )
                return false;

            foreach( CollisionRuleEntry rule in _rules )
            {
                if( rule.Involves( classA, classB ) )
                {
                    collisionRule = rule;
                    return true;
                }
            }
            return false;
        }

        public bool ContainsRulesFor( uint collidable )
        {
            foreach( CollisionRuleEntry rule in _rules )
            {
                if( rule.Involves( collidable ) )
                {
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            _rules = new List<CollisionRuleEntry>();
        }
    }
}
