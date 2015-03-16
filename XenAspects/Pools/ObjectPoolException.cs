using System;

namespace XenAspects
{
    public class ObjectPoolException : Exception
    {
        public ObjectPoolException( string message ) : base( message ) { }
        public ObjectPoolException( string message, Exception inner ) : base( message, inner ) { }
    }
}