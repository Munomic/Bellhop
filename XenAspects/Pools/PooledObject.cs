using System;

namespace XenAspects
{
    public enum PooledObjectState
    {
        Available, // In the pool, ready to be acquired
        Acquired,  // In-use
        Releasing  // Waiting to be reclaimed for the pool
    }

    /// <summary>
    /// Interface for an object that is managed by an object pool and will never be garbage collected.
    /// </summary>
    public interface IPooledObject
    {
        /// <summary>
        /// Event for when this object expires (marked for reclamation by its type pool)
        /// Expired objects should no longer be used, nor should references be maintained to them. 
        /// </summary>
        FireOnceEvent<IPooledObject> OnReleased { get; }

        /// <summary>
        /// Releases the object, which marks it to be recollected by the pool.  
        /// </summary>
        void Release();

        /// <summary>
        /// Gets the state of the pooled object, referring to whether it is in-use, available, or in the processing of being released.
        /// </summary>
        PooledObjectState PoolState { get; }
        
        /// <summary>
        /// Gets or sets the index of this object in the parent object pool.  Negative value indicates that this object is not part of a pool.
        /// </summary>
        /// <remarks>DO NOT USE THIS PROPERTY EVER.</remarks>
        //int PoolIndex { get; set; }
    }

    /// <summary>
    /// Interface for an object that is managed by an object pool and will never be garbage collected.
    /// </summary>
    public interface IPooledObject<T>
    {
        /// <summary>
        /// Event for when this object expires (marked for reclamation by its type pool)
        /// Expired objects should no longer be used, nor should references be maintained to them. 
        /// </summary>
        FireOnceEvent<IPooledObject<T>> OnReleased { get; }

        /// <summary>
        /// Releases the object, which marks it to be recollected by the pool.  
        /// </summary>
        void Release();

        /// <summary>
        /// Gets the state of the pooled object, referring to whether it is in-use, available, or in the processing of being released.
        /// </summary>
        PooledObjectState PoolState { get; }

        /// <summary>
        /// Gets or sets the index of this object in the parent object pool.  Negative value indicates that this object is not part of a pool.
        /// </summary>
        /// <remarks>DO NOT USE THIS PROPERTY EVER.</remarks>
        //int PoolIndex { get; set; }
    }

    public abstract class PooledObject : IPooledObject
    {
        private FireOnceEvent<IPooledObject> _onReleasedEvent = new FireOnceEvent<IPooledObject>();
        private PooledObjectState _poolState = PooledObjectState.Available;
        private int _poolIndex = -1;

        internal int PoolIndex 
        { 
            get { return _poolIndex; } 
            set { _poolIndex = value; } 
        }

        public PooledObjectState PoolState
        {
            get { return _poolState; }
            internal set { _poolState = value; }
        }

        public FireOnceEvent<IPooledObject> OnReleased
        {
            get { return _onReleasedEvent; }
            protected set { _onReleasedEvent = value; }
        }

        public bool IsAvailable { get { return PooledObjectState.Available == _poolState; } }
        public bool IsAcquired { get { return PooledObjectState.Acquired == _poolState; } }
        public bool IsReleasing { get { return PooledObjectState.Releasing == _poolState; } }

        public void Release()
        {
            if( _poolState == PooledObjectState.Acquired )
            {
                _poolState = PooledObjectState.Releasing;
                RaiseOnReleased();
                ReleaseInternal();
                ReturnToPool();
            }
            else
            {
                throw new ObjectPoolException( "Can only release Acquired objects" );
            }
        }

        //Derived classes can override this method to run custom behavior when instances of this object are released.
        protected virtual void ReleaseInternal() { }

        protected abstract void ReturnToPool();

        protected virtual void RaiseOnReleased()
        {
            if( null != OnReleased )
            {
                OnReleased.Notify( this );
            }
        }
    }

    public abstract class PooledObject<T> : PooledObject
        where T : PooledObject, new()
    {
        public static IObjectPool Pool { get { return _pool; } }
        protected static readonly Pool<T> _pool = new Pool<T>( t => ( t.PoolState == PooledObjectState.Acquired ) )
        {
            Initialize = t =>
            {
                t.PoolState = PooledObjectState.Acquired;
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