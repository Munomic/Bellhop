using System;
using System.Collections;
using System.Collections.Generic;

namespace XenAspects
{
    public interface ISortedPooledObjectNList<T> : IPooledObjectNList<T> 
        where T : class, IPooledObject
    {
        IComparer<T> SortComparer { get; set; }
    }

    /// <summary>
    /// This specialized PooledObjectNList keeps the contents of the collection in a sorted order as specified
    /// by the SortComparer property.
    /// </summary>
    public class SortedPooledObjectNList<T> : PooledObjectNList<T>, ISortedPooledObjectNList<T>
        where T : class, IPooledObject
    {
        private IComparer<T> _sortComparer;

        /// <summary>
        /// Gets or sets the sort comparer that determines the order of items in this collection.  Default is null.
        /// </summary>
        public IComparer<T> SortComparer
        {
            get { return _sortComparer; }
            set
            {
                if( _sortComparer != value )
                {
                    _sortComparer = value;
                    if( _sortComparer != null )
                        Sort();
                }
            }
        }

        protected override void AfterAddItem( T item )
        {
            base.AfterAddItem( item );

            Sort();
        }

        private void Sort()
        {
            if( null != SortComparer )
                Items.Sort( SortComparer );
        }
    }
}