using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace XenAspects
{
    public interface INList<T>
        where T : class
    {
        bool Add( T item );
        void BeginEnumeration();
        void Clear();
        bool Contains( T item );
        int Count { get; }
        void EndEnumeration();
        int IndexOf( T item );
        bool Insert( int index, T item );

        /// <summary>
        /// Only use this property for iteration.  Never modify this collection or else the world will end.  
        /// </summary>
        List<T> Items { get; }
        bool Remove( T item );
        bool RemoveAt( int index );
        T this[ int index ] { get; }
    }

    /// <summary>
    /// This class provides a set of wrapper methods for generic lists that do not generate garbage.  When
    /// enumerating over this list, use the Items property.  
    /// </summary>
    /// <typeparam name="T">The type of object contained in the list.</typeparam>
    /// <remarks>
    /// When performing enumeration actions on this list, make sure to call BeginEnumeration() and EndEnumeration().  
    /// These calls ensure that released objects will be removed from the list as soon as possible.  During enumeration,
    /// the list cannot be modified, so the call to EndEnumeration() will cause all pending-release items to be flushed.
    /// </remarks>
    public class NList<T> : INList<T>
        where T : class
    {
        /// <summary>
        /// Default NList capacity
        /// </summary>
        public static int DefaultCapacity = 32;

        /// <summary>
        /// NList resize amount when necessary to resize the operation set
        /// </summary>
        public static int ResizeAmount = 16;

        private List<T> _items = null;

        //Use an array of structs so that add/remove do not allocate from managed heap.
        private NListBufferedOperation<T>[] _buffer = new NListBufferedOperation<T>[ DefaultCapacity ];
        private int _numItemsInBuffer;
         
        //BeginEnumeration increments this field, EndEnumeration decrements this field
        private int _enumerationCount;

        /// <summary>
        /// Indicates whether or not a BeginEnumeration has been met with an EndEnumeration
        /// </summary>
        private bool IsEnumerating { get { return _enumerationCount > 0; } }

        /// <summary>
        /// Gets the number of items in the List
        /// </summary>
        public int Count { get { return _items.Count; } }

        /// <summary>
        /// Gets the item list that NList encapsulates
        /// </summary>
        public List<T> Items { get { return _items; } }

        /// <summary>
        /// Gets or Sets the item at the given index
        /// </summary>
        /// <param name="index">Item index</param>
        public virtual T this[ int index ]
        {
            get{ return _items[ index ]; }
            set
            {
                if( IsEnumerating )
                    throw new InvalidOperationException( "This collection is currently being enumerated.  Cannot modify an indexed element during enumeration" );
                T oldItem = _items[ index ];
                if( ( oldItem != value ) )
                {
                    if( BeforeAddItem( value ) && BeforeRemoveItem( oldItem ) )
                    {
                        _items[ index ] = value;
                        AfterAddItem( value );
                        AfterRemoveItem( oldItem );
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new NList
        /// </summary>
        public NList() : this( DefaultCapacity ) { }

        /// <summary>
        /// Creates a new NList
        /// </summary>
        /// <param name="initialCapacity">Initial Cache Size of the list</param>
        public NList( int initialCapacity )
        {
            _items = new List<T>( initialCapacity );
        }

        /// <summary>
        /// Starts an enumeration of the list
        /// </summary>
        public void BeginEnumeration()
        {
            _enumerationCount++;
        }

        /// <summary>
        /// Completes an enumeration of the list
        /// </summary>
        public void EndEnumeration()
        {
            _enumerationCount--;
            if( !IsEnumerating )
                ProcessBuffer();
        }

        /// <summary>
        /// Returns the list index of the given item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Index</returns>
        public int IndexOf( T item )
        {
            return _items.IndexOf( item );
        }

        /// <summary>
        /// Inserts an item into the list
        /// </summary>
        /// <param name="index">Index at which to perform the insert</param>
        /// <param name="item">Item to insert</param>
        /// <returns>True on success, False on failure</returns>
        /// <remarks>This method cannot be called during enumeration.</remarks>
        /// <exception cref="InvalidOperationException">Thrown when this method is called during an enumeration</exception>
        public virtual bool Insert( int index, T item )
        {
            if( IsEnumerating )
                throw new InvalidOperationException( "This collection is currently being enumerated.  Insert is unpredictable when performed during enumeration" );

            if( CanAdd( item ) && BeforeAddItem( item ) )
            {
                _items.Insert( index, item );
                AfterAddItem( item );
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an item in the list
        /// </summary>
        /// <param name="index">Index at which to perform the remove</param>
        /// <returns>True on success, False on failure</returns>
        /// <remarks>This method cannot be called during enumeration.</remarks>
        /// <exception cref="InvalidOperationException">Thrown when this method is called during an enumeration</exception>
        public bool RemoveAt( int index )
        {
            if( IsEnumerating )
                throw new InvalidOperationException( "This collection is currently being enumerated.  RemoveAt is unpredictable when performed during enumeration" );

            if( CanRemoveAt( index ) )
            {
                RemoveAtActual( index );
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds an item to the list
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>True on success, False on failure</returns>
        /// <remarks>If this method is called during enumeration, the operation will be processed upon EndEnumeration()</remarks>
        public virtual bool Add( T item )
        {
            if( CanAdd( item ) && BeforeAddItem( item ) )
            {
                if( !IsEnumerating )
                {
                    AddActual( item );
                }
                else
                {
                    BufferOperation( NListBufferedOperationType.Add, item );
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an item from the list
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>True on success, False on failure</returns>
        /// <remarks>If this method is called during enumeration, the operation will be processed upon EndEnumeration()</remarks>
        public virtual bool Remove( T item )
        {
            int foundIndex = GetIndexOf( item );
            
            //If item was found
            if( foundIndex >= 0 )
            {
                if( !IsEnumerating )
                {
                    RemoveAtActual( foundIndex );
                }
                else
                {
                    BufferOperation( NListBufferedOperationType.Remove, item );
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears the entire list
        /// </summary>
        /// <remarks>If this method is called during enumeration, the operation will be processed upon EndEnumeration()</remarks>
        public virtual void Clear()
        {
            if( !IsEnumerating )
            {
                //iterate and call RemoveAt so that the template methods for handling 
                //before and after remove are correctly invoked
                for( int i = _items.Count - 1; i >= 0; i-- )
                {
                    RemoveAt( i );
                }
            }
            else
            {
                BufferOperation( NListBufferedOperationType.Clear, null );
            }
        }

        /// <summary>
        /// Determines whether or not an item is in the NList<<typeparamref name="T"/>>
        /// </summary>
        /// <param name="item">The object to locate in the list</param>
        /// <returns>True if the item is found, False otherwise</returns>
        public bool Contains( T item )
        {
            return _items.Contains( item );
        }

        #region Private Methods

        /// <summary>
        /// This template method is called before an item is added to the collection.  Derived classes can abort the
        /// operation by returning false.
        /// </summary>
        /// <param name="item">The item that is about to be added.</param>
        /// <returns>True if the operation should continue.  False to abort the operation.</returns>
        protected virtual bool BeforeAddItem( T item ) { return true; }

        /// <summary>
        /// This template method is called after an item is added to the collection.  Derived classes can override this 
        /// method to perform sorting operations on the collection.
        /// </summary>
        /// <param name="item">The item that was just added.</param>
        protected virtual void AfterAddItem( T item ) { }

        /// <summary>
        /// This template method is called before an item is removed from the collection.  Derived classes can abort the
        /// operation by returning false.
        /// </summary>
        /// <param name="item">The item that is about to be removed.</param>
        /// <returns>True if the operation should continue.  False to abort the operation.</returns>
        protected virtual bool BeforeRemoveItem( T item ) { return true; }

        /// <summary>
        /// This template method is called after an item is removed from the collection.  Derived classes can override this 
        /// method to perform sorting operations on the collection.
        /// </summary>
        /// <param name="item">The item that was just removed.</param>
        protected virtual void AfterRemoveItem( T item ) { }

        /// <summary>
        /// Buffers an Add/Remove/Clear operation so that it can be executed on the collection when enumeration is complete.
        /// </summary>
        /// <param name="opType">The operation type.</param>
        /// <param name="item">The item to act on.  This parameter is ignored if the operation type is Clear.</param>
        private void BufferOperation( NListBufferedOperationType opType, T item )
        {
            _buffer[ _numItemsInBuffer ] = new NListBufferedOperation<T>( opType, item );
            _numItemsInBuffer++;

            //grow array if the buffer index is larger than the array size, no choice but to allocate from heap.
            if( _numItemsInBuffer >= _buffer.Length )
            {
#if TRACE && WINDOWS
                Trace.WriteLine( typeof(T).ToString() + ":\t\tResizing NList for type: " + typeof( T ) + ". New size: " + ( _buffer.Length + ResizeAmount ) );
#endif
                var tempBuffer = new NListBufferedOperation<T>[ _buffer.Length + ResizeAmount ];
                Array.Copy( _buffer, tempBuffer, _buffer.Length );
                _buffer = tempBuffer;
            }
        }

        private void ProcessBuffer()
        {
            for( int i = 0; i < _numItemsInBuffer; i++ )
            {
                var bufferItem = _buffer[ i ];
                switch( bufferItem.OperationType )
                {
                    case NListBufferedOperationType.Add:
                        Add( bufferItem.Item );
                        break;
                    case NListBufferedOperationType.Clear:
                        Clear();
                        break;
                    case NListBufferedOperationType.Remove:
                        Remove( bufferItem.Item );
                        break;
                    case NListBufferedOperationType.None:
                        break;
                }
                _buffer[ i ] = NListBufferedOperation<T>.None;
            }
            _numItemsInBuffer = 0;
        }

        private void AddActual( T item )
        {
            BeforeAddItem( item );
            _items.Add( item );
            AfterAddItem( item );
        }

        private void RemoveAtActual( int index )
        {
            T item = _items[ index ];

            BeforeRemoveItem( item );

            //BeforeRemoveItem may have already removed the item, check to see if it still present before removing
            //list.contains may generate garbage, so use a for loop here.
            int foundIndex = -1;
            for( int i = 0; i < _items.Count; i++ )
            {
                if( item == _items[ i ] )
                {
                    foundIndex = i;
                }
            }

            if( foundIndex != -1 )
            {
                _items.RemoveAt( foundIndex );
            }

            AfterRemoveItem( item );
        }

        protected virtual bool CanAdd( T item )
        {
            return ( item != null ) && !Contains( item );
        }

        protected virtual bool CanRemove( T item )
        {
            if( null == item ) return false;
            if( !Contains( item ) ) return false;

            return true;
        }

        protected virtual bool CanRemoveAt( int index )
        {
            if( index < 0 ) return false;
            if( index >= _items.Count ) return false;

            return true;
        }

        private int GetIndexOf( T item )
        {
            //Get the index and call RemoveAt because List.Remove generates garbage when it creates an iterator
            int foundIndex = -1;

            for( int i = 0; i < _items.Count; i++ )
            {
                if( item == _items[ i ] )
                {
                    foundIndex = i;
                    break;
                }
            }

            return foundIndex;
        }

        #endregion
    }
}