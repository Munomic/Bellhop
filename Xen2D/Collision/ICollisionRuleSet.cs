
namespace Xen2D
{
    public interface ICollisionRuleSet
    {
        /// <summary>
        /// Adds a rule to the ruleset indicating that the two specified collision classes can collide
        /// </summary>
        /// <param name="classA">The first class to participate in the collision</param>
        /// <param name="classB">The second class to participate in the collision</param>
        void AddRule( uint classA, uint classB );

        /// <summary>
        /// Adds a rule to the ruleset indicating that the two specified collision classes can collide
        /// </summary>
        /// <param name="classA">The first class to participate in the collision</param>
        /// <param name="classB">The second class to participate in the collision</param>
        /// <param name="cooldown">The number of seconds that must elapse before the two specified entities can collide again.</param>
        void AddRule( uint classA, uint classB, float cooldown );

        /// <summary>
        /// Removes a rule from the ruleset indicating that the two specified collision classes can collide
        /// </summary>
        /// <param name="classA">The first class to participate in the collision</param>
        /// <param name="classB">The second class to participate in the collision</param>
        /// <returns>Whether the rule was successfully removed.</returns>
        bool RemoveRule( uint classA, uint classB );

        /// <summary>
        /// Checks whether the two specified collidables can collide according to this ruleset.  
        /// </summary>
        /// <param name="classA">The first class to participate in the collision</param>
        /// <param name="classB">The second class to participate in the collision</param>
        /// <param name="collisionRule">The collision rule indicating that the two classes can collide.  
        /// This value is null if no collision rule was found.</param>
        /// <returns>True if the two classes can collide, false otherwise.</returns>
        bool CanCollide( uint classA, uint classB, out CollisionRuleEntry collisionRule );

        /// <summary>
        /// Checks whether the two specified collidables can collide according to this ruleset.  
        /// </summary>
        /// <param name="classA">The first class to participate in the collision</param>
        /// <param name="classB">The second class to participate in the collision</param>
        /// <returns>True if the two classes can collide, false otherwise.</returns>
        bool CanCollide( uint classA, uint classB );

        /// <summary>
        /// Checks if there are any rules defined for the specified collision class.  
        /// </summary>
        /// <param name="collidable">The collision class to check for.</param>
        /// <returns>True if there are collision rules specified for this class, false otherwise.</returns>
        bool ContainsRulesFor( uint collidable );
    }
}


