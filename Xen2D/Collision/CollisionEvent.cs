using System;

namespace Xen2D
{
    /// <summary>
    /// A data structure that indicates a collision has occured between two collidable objects.  
    /// </summary>
    public struct CollisionEventArgs
    {
        ICollidableObject _a, _b;
        public ICollidableObject A { get { return _a; } }
        public ICollidableObject B { get { return _b; } }

        public CollisionEventArgs( ICollidableObject a, ICollidableObject b )
        {
            _a = a;
            _b = b;
        }

        public bool Involves( object obj )
        {
            return ( A == obj ) || ( B == obj );
        }

        public bool InvolvesType( Type type )
        {
            return ( ( A.GetType() == type ) ||
                     ( B.GetType() == type ) );
        }

        public bool InvolvesTypes<TypeA, TypeB>()
        {
            return InvolvesTypes( typeof( TypeA ), typeof( TypeB ) );
        }

        public bool InvolvesTypes( Type typeA, Type typeB )
        {
            return ( ( A.GetType() == typeA ) &&
                     ( B.GetType() == typeB ) )
                     ||
                   ( ( A.GetType() == typeB ) &&
                     ( B.GetType() == typeA ) );
        }

        /// <summary>
        /// Gets the collision participant of the parameterized type.
        /// </summary>
        /// <typeparam name="T">The type of participant to retrieve. </typeparam>
        /// <param name="participant">The output paramtere populated with the return participant.</param>
        /// <returns>True if a participant </returns>
        public bool ContainsParticipantOfType<T>( out T participant )
            where T : class
        {
            participant = null;
            if( A.GetType() == typeof( T ) )
            {
                participant = A as T;
                return true;
            }
            else if( B.GetType() == typeof( T ) )
            {
                participant = B as T;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the participant of the specified type.  If both participants match the expected type, this method will return one of the
        /// two participants.  
        /// </summary>
        /// <typeparam name="T">The type of the participant to retrieve.</typeparam>
        /// <returns>The collision participant of the specified type.</returns>
        public T GetParticipantOfType<T>()
            where T : class
        {
            if( A.GetType() == typeof( T ) )
            {
                return A as T;
            }
            else if( B.GetType() == typeof( T ) )
            {
                return B as T;
            }
            return null;
        }
    }
}


