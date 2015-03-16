using System;
using System.Collections.Generic;

namespace XenAspects
{
    internal enum NListBufferedOperationType
    {
        None,
        Add,
        Remove,
        Clear,
    }

    internal struct NListBufferedOperation<T>
    {
        public static NListBufferedOperation<T> None = new NListBufferedOperation<T>( NListBufferedOperationType.None, default(T) );

        public NListBufferedOperationType OperationType;
        public T Item;

        public NListBufferedOperation( NListBufferedOperationType opType, T item )
        {
            OperationType = opType;
            Item = item;
        }
    }
}