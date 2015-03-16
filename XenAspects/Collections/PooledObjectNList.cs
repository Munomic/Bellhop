using System;
using System.Collections;
using System.Collections.Generic;

namespace XenAspects
{
    public interface IPooledObjectNList<T> : INListTemplate<T>
        where T : class, IPooledObject
    {
        void ReleaseAndClearChildren();
        Event<T> OnItemReleased { get; }
    }

    public interface IPooledObjectNList : IPooledObjectNList<IPooledObject>
    {
    }

    /// <summary>
    /// This class represents a collection of releasable object.  When contained objects are released, they are automatically 
    /// removed from the parent collection.
    /// </summary>
    /// <remarks>
    /// When performing enumeration actions on this list, make sure to call BeginEnumeration() and EndEnumeration().  
    /// These calls ensure that released objects will be removed from the list as soon as possible.  During enumeration,
    /// the list cannot be modified, so the call to EndEnumeration() will cause all pending-release items to be flushed.
    /// </remarks>
    public class PooledObjectNList : PooledObjectNList<IPooledObject>
    {
    }

    /// <summary>
    /// This class represents a collection of releasable object.  When contained objects are released, they are automatically 
    /// removed from the parent collection.
    /// </summary>
    /// <typeparam name="T">The type of releasable object.  Must be a reference type and implement IPooledObject</typeparam>
    /// <remarks>
    /// When performing enumeration actions on this list, make sure to call BeginEnumeration() and EndEnumeration().  
    /// These calls ensure that released objects will be removed from the list as soon as possible.  During enumeration,
    /// the list cannot be modified, so the call to EndEnumeration() will cause all pending-release items to be flushed.
    /// </remarks>
    public class PooledObjectNList<T> : NListTemplate<T>, IPooledObjectNList<T>
        where T : class, IPooledObject
    {
        private Action<IPooledObject> _onChildItemReleasedHandler;

        /// <summary>
        /// Event Handler Called immediately after an item is released
        /// </summary>
        public Event<T> OnItemReleased { get; private set; }

        /// <summary>
        /// Creates a PooledObjectNList
        /// </summary>
        public PooledObjectNList() : this( DefaultCapacity ) { }

        /// <summary>
        /// Creates a PooledObjectNList
        /// </summary>
        /// <param name="initialEntriesCapacity">Initial capacity</param>
        public PooledObjectNList( int initialEntriesCapacity )
            : base( initialEntriesCapacity )
        {
            OnItemReleased = new Event<T>();
            _onChildItemReleasedHandler = new Action<IPooledObject>( OnChildItemReleased );
        }

        /// <summary>
        /// Releases and Removes all items in the list
        /// </summary>
        public void ReleaseAndClearChildren()
        {
            BeginEnumeration();

            foreach( T item in Items )
                item.Release();

            Clear();

            EndEnumeration();
        }

        #region Private Methods

        private void OnChildItemReleased( IPooledObject expiredItem )
        {
            OnChildItemReleased( (T)expiredItem );
        }

        private void OnChildItemReleased( T expiredChild )
        {
            Remove( expiredChild );
            OnItemReleased.Notify( expiredChild );
        }

        protected override void AfterAddItem( T item )
        {
            base.AfterAddItem( item );
            item.OnReleased.Add( _onChildItemReleasedHandler );
        }

        protected override void AfterRemoveItem( T item )
        {
            base.AfterRemoveItem( item );
            item.OnReleased.Remove( _onChildItemReleasedHandler );
        }

        #endregion
    }
}