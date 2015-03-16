using System;
using System.Collections.Generic;

namespace XenAspects
{
    public interface INListTemplate<T> : INList<T>
        where T : class
    {
        Event<T> OnItemBeforeAdded { get; }
        Event<T> OnItemAfterAdded { get; }
        Event<T> OnItemBeforeRemoved { get; }
        Event<T> OnItemAfterRemoved { get; }
    }

    /// <summary>
    /// This class extends the NList by adding the ability to listen on ItemAdded and ItemRemoved events.  
    /// This is necessary because the Event class uses NLists to track subscribers.  
    /// </summary>
    /// <typeparam name="T">The contained type.</typeparam>
    public class NListTemplate<T> : NList<T>, INListTemplate<T>
        where T : class
    {
        /// <summary>
        /// Event Handler Called immediately before an item is added
        /// </summary>
        public Event<T> OnItemBeforeAdded { get; private set; }

        /// <summary>
        /// Event Handler Called immediately after an item is added
        /// </summary>
        public Event<T> OnItemAfterAdded { get; private set; }

        /// <summary>
        /// Event Handler Called immediately before an item is removed
        /// </summary>
        public Event<T> OnItemBeforeRemoved { get; private set; }

        /// <summary>
        /// Event Handler Called immediately after an item is removed
        /// </summary>
        public Event<T> OnItemAfterRemoved { get; private set; }

        /// <summary>
        /// Creates an NListTemplate
        /// </summary>
        public NListTemplate() : this( DefaultCapacity ) { }

        /// <summary>
        /// Creates an NListTemplate
        /// </summary>
        /// <param name="initialCapacity">NList initial capacity</param>
        public NListTemplate( int initialCapacity )
            : base( initialCapacity )
        {
            OnItemBeforeAdded = new Event<T>();
            OnItemAfterAdded = new Event<T>();
            OnItemBeforeRemoved = new Event<T>();
            OnItemAfterRemoved = new Event<T>();
        }

        /// <summary>
        /// Gets or Sets the item at the given index
        /// </summary>
        /// <param name="index">Item index</param>
        public override T this[ int index ]
        {
            get
            {
                return base[ index ];
            }
            set
            {
                if( base[ index ] != value )
                {
                    T removed = base[ index ];
                    base[ index ] = value;
                    OnItemAfterRemoved.Notify( removed );
                    OnItemAfterAdded.Notify( value );
                }
            }
        }

        #region Private Methods

        protected override bool BeforeAddItem( T item )
        {
            OnItemBeforeAdded.Notify( item );
            return base.BeforeAddItem( item );
        }

        protected override void AfterAddItem( T item )
        {
            OnItemAfterAdded.Notify( item );
        }

        protected override bool BeforeRemoveItem( T item )
        {
            OnItemBeforeRemoved.Notify( item );
            return base.BeforeRemoveItem( item );
        }

        protected override void AfterRemoveItem( T item )
        {
            OnItemAfterRemoved.Notify( item );
        }

        #endregion
    }
}