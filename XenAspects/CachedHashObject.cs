using System;

namespace XenAspects
{
    /// <summary>
    /// This object overrides the default GetHashCode with a caching variant.  Subsequent calls to GetHashCode will
    /// return the cached value, thus bypassing the cost of recalculating the hash code.
    /// </summary>
    /// <remarks>
    /// Benchmarks indicate that the caching implementation performs 10x faster than the default implementation if 
    /// the number of calls to GetHashCode is large.  Use this class when GetHashCode will be called frequently.  
    /// This class is only needed when compiling for .net 3.5 or lower.  .net 4.0 seems to have implemented this same behavior
    /// </remarks>
    public class CachedHashObject
    {
        private bool _hashCodeInitialized = false;
        private int _cachedHashCode;

        public override int GetHashCode()
        {
            if( !_hashCodeInitialized )
            {
                _hashCodeInitialized = true;
                _cachedHashCode = base.GetHashCode();
            }
            return _cachedHashCode;
        }
    }
}