using System;
using System.Diagnostics;

namespace XenAspects
{
    public interface IObjectPool
    {
        void CleanUp();
        void InitPool( int initialSize );
        void InitPoolAndLock( int initialSize );
        void Flush();
        bool Locked { get; set; }
        int Capacity { get; }
        int NumAvailableInstances { get; }
    }

    public interface IObjectPool<T> : IObjectPool
    {
        T Acquire();
    }

    public class Pool<T> : IObjectPool<T> 
        where T : PooledObject, new()
    {
        public static int DefaultResizeAmount = 20;

        public delegate T Allocate();

        private bool _initialized = false;
        private bool _locked = false;
        private T[] _items;
        private readonly Predicate<T> _validate;
        private readonly Allocate _allocate;

        public Action<T> Initialize { get; set; }
        public Action<T> Deinitialize { get; set; }

        private int AvailableCount { get; set; }

        /// <summary>
        /// Gets or sets whether this pool will throw an exception when a new instance allocates memory.
        /// </summary>
        public bool Locked
        {
            get { return _locked; }
            set { _locked = value; }
        }

        public int ResizeAmount { get; set; }
        public int Capacity { get { return _items.Length; } }
        public int NumAvailableInstances { get { return AvailableCount; } }

        public T this[ int index ]
        {
            get
            {
                index += AvailableCount;

                if( index < AvailableCount || index >= _items.Length )
                    throw new ObjectPoolException( "The index must be less than or equal to AcquiredCount" );
                return _items[ index ];
            }
        }

        public Pool( Predicate<T> validateFunc ) : this( 0, validateFunc ) { }
        public Pool( int initialSize, Predicate<T> validateFunc ) : this( initialSize, validateFunc, null ) { }
        public Pool( Predicate<T> validateFunc, Allocate allocateFunc ) : this( 0, validateFunc, allocateFunc ) { }
        public Pool( int initialSize, Predicate<T> validateFunc, Allocate allocateFunc )
        {
            ResizeAmount = DefaultResizeAmount;
            if( initialSize < 0 )
                throw new ArgumentOutOfRangeException( "initialSize must be non-negative" );
            if( validateFunc == null )
                throw new ArgumentNullException( "validateFunc is null" );

            if( initialSize == 0 )
                initialSize = ResizeAmount;

            _items = new T[ initialSize ];
            _validate = validateFunc;
            AvailableCount = _items.Length;

            //NOTE: _allocate is no longer assigned a default ConstructorInfo via reflection.  This is because on .net CF, reflection for 
            //obtaining private constructors is not allowed.  Thus, all pooled objects need to have a public parameterless constructor.  
            //Pro: pool resizing is much faster.
            //Con: user may accidentally directly instantiate objects via constructor.
            _allocate = allocateFunc ?? null;

            ObjectPoolMonitor.Register( this );
        }

        public void Flush()
        {
            if( Locked )
            {
                throw new ObjectPoolException( "Cannot flush pool while locked" );
            }

            for( int i = 0; i < _items.Length; i++ )
            {
                SetItemAtIndex( i, null );
            }
            _items = null;
            _initialized = false;
        }

        public void InitPool( int initialSize )
        {
            InitPool( initialSize, DefaultResizeAmount );
        }

        public void InitPool( int initialSize, int resizeAmount )
        {
            if( _initialized )
            {
                throw new ObjectPoolException( "pool is already initialized" );
            }
            ResizeAmount = resizeAmount;
            _items = new T[ initialSize ];

            for( int i = 0; i < _items.Length; i++ )
            {
                SetItemAtIndex( i, ( null != _allocate ) ? _allocate() : new T() );
            }
            AvailableCount = _items.Length;
        }

        public void InitPoolAndLock( int initialSize )
        {
            InitPoolAndLock( initialSize, DefaultResizeAmount );
        }

        public void InitPoolAndLock( int initialSize, int resizeAmount )
        {
            InitPool( initialSize, resizeAmount );
            Locked = true;
        }

        public void CleanUp()
        {
            for( int i = AvailableCount; i < _items.Length; i++ )
            {
                ReclaimItemAtIndex( i );
            }
        }

        /// <summary>
        /// Reclaims an item at the specified index, swapping so that it is within the block of available instances.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool ReclaimItemAtIndex( int index )
        {
            T obj = _items[ index ];
            if( _validate( obj ) )
            {
                return false;
            }

            if( index != AvailableCount )
            {
                SetItemAtIndex( index, _items[ AvailableCount ] );
                SetItemAtIndex( AvailableCount, obj );
            }

            if( Deinitialize != null )
            {
                Deinitialize( obj );
            }

            AvailableCount++;

            return true;
        }

        private void SetItemAtIndex( int index, T item )
        {
            if( null == item )
            {
                _items[ index ].PoolIndex = -1;
                _items[ index ] = null;
            }
            else
            {
                _items[ index ] = item;
                item.PoolIndex = index;
            }
        }

        public T Acquire()
        {
            return New();
        }

        public T New()
        {
            if( AvailableCount == 0 )
            {
                if( Locked )
                {
                    throw new ObjectPoolException( "Pool is trying to resize but cannot because it is locked" );
                }
#if TRACE && WINDOWS
                Trace.WriteLine( typeof(T).ToString() + ":\t\tResizing pool. Old size: " + _items.Length + ". New size: " + ( _items.Length + ResizeAmount ) );
#endif
                T[] newItems = new T[ _items.Length + ResizeAmount ];
                for( int i = _items.Length - 1; i >= 0; i-- )
                {
                    newItems[ i + ResizeAmount ] = _items[ i ];
                    newItems[ i + ResizeAmount ].PoolIndex = ( i + ResizeAmount );
                }
                _items = newItems;

                AvailableCount += ResizeAmount;
            }

            AvailableCount--;

            T obj = _items[ AvailableCount ];

            if( obj == null )
            {
                if( Locked )
                {
                    throw new ObjectPoolException( "No more pooled instances available." );
                }

                obj = ( null != _allocate ) ? _allocate() : new T();

                if( obj == null )
                {
                    throw new InvalidOperationException( "The pool's allocate method returned a null object reference" );
                }

                SetItemAtIndex( AvailableCount, obj );
            }

            if( Initialize != null )
            {
                Initialize( obj );
            }

            return obj;
        }

        internal void Release( PooledObject instance )
        {
            Debug.Assert( null != instance );
            Debug.Assert( _items[ instance.PoolIndex ] == instance );
            ReclaimItemAtIndex( instance.PoolIndex );
        }
    }
}