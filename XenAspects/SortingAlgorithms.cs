using System;
using System.Collections.Generic;

namespace XenAspects
{
    public static class SortingAlgorithms
    {
        /// <summary>
        /// Sorts the specified list using Insertion Sort.  This sort is efficient for smaller sets of data that are mostly sorted.  
        /// </summary>
        /// <typeparam name="ItemType">The type of the item to sort</typeparam>
        /// <param name="listToSort">The list of items to sort.</param>
        /// <param name="comparer">The comparer that specifies how to compare items of type ItemType.</param>
        public static void InsertionSort<ItemType>( List<ItemType> listToSort, IComparer<ItemType> comparer )
        {
            if( null == listToSort || null == comparer )
                throw new ArgumentNullException( "list and comparer must both be non-null" );

            int j;
            ItemType currentItem;
            int numElements = listToSort.Count;
            for( int i = 0; i < numElements; i++ )
            {
                currentItem = listToSort[ i ];
                j = i;

                //comparer.Compare( listToSort[ j - 1], currentItem )
                //returns 1 if listToSort[ j - 1] is greater
                while( ( j > 0 ) && ( comparer.Compare( listToSort[ j - 1 ], currentItem ) > 0 ) )
                {
                    listToSort[ j ] = listToSort[ j - 1 ];
                    j--;
                }

                listToSort[ j ] = currentItem;
            }
        }
    }
}