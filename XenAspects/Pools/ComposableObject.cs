using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace XenAspects
{
    /// <summary>
    /// Interface for an object that is can be composed or reset to automatically acquire/release
    /// composed parts from/to their respective pools
    /// </summary>
    public interface IComposableObject : IPooledObject
    {
        /// <summary>
        /// Resets the object to its initial state, as though it had just been acquired from the pool.
        /// Use this method to avoid an expire/acquire cycle.  
        /// </summary>
        void Reset();
    }

    /// <summary>
    /// A composable object is a pooled object whose state is split amongst two categories:
    /// -Direct State:
    ///     These fields are direct children of the object and are attached through reuse between acquire/expire to and from the pool.
    ///     Prefer these to be value types and contained in a struct.  
    /// -Composed State:
    ///     These fields are automatically acquired from their respective pools when an instance of this object is acquired.
    ///     These fields are returned to their respective pools when this object is expired/released.  
    /// </summary>
    public abstract class ComposableObject : PooledObject, IComposableObject
    {
        protected bool _declareCompositionInitialized = false;
        protected List<IComposedField> _composableFields = new List<IComposedField>();

        protected void DeclareComposition( params IComposedField[] composedFields )
        {
            Debug.Assert( _declareCompositionInitialized == false, "cannot declare composition of an object twice" );

            foreach( var field in composedFields )
            {
                _composableFields.Add( field );
            }
            _declareCompositionInitialized = true;
        }

        protected override void ReleaseInternal()
        {
            foreach( var field in _composableFields )
            {
                field.ReleaseField();
            }
        }

        /// <summary>
        /// Resets the object to its initial state, as though it had just been acquired from the pool.
        /// Use this method to avoid an expire/acquire cycle.  
        /// </summary>
        public virtual void Reset()
        {
            //Resetting the object is a means to bypass Release/Acquire cycle, so it must cause a release to occur.
            RaiseOnReleased();

            foreach( var field in _composableFields )
            {
                field.AcquireField();
            }
            ResetDirectState();
        }

        /// <summary>
        /// Resets the direct state attached to this instance.
        /// </summary>
        protected virtual void ResetDirectState()
        {
            //Derived classes override this method to reset direct state (usually by re-initializing a struct which contains all the private members).  
        }
    }

    public abstract class ComposableObject<T> : ComposableObject 
        where T : ComposableObject, new()
    {
        public static IObjectPool<T> Pool { get { return _pool; } }
        protected static readonly Pool<T> _pool = new Pool<T>( t => ( t.PoolState == PooledObjectState.Acquired ) )
        {
            //This is where the ComposableObject differs from a regular PooledObject.  
            //Initialize will acquire all the composed fields or reset them as well as reset the direct state.
            Initialize = t =>
            {
                t.PoolState = PooledObjectState.Acquired;
                t.Reset();
            },
            Deinitialize = t =>
            {
                t.PoolState = PooledObjectState.Available;
            }
        };

        public static T Acquire()
        {
            return _pool.New();
        }

        protected override void ReturnToPool() 
        {
            _pool.Release( this );
        }
    }
}