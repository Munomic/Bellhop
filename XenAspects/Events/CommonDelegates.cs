using System;

namespace XenAspects
{
    // Basic delegate declarations for Get/Set functionality
    public delegate T Getter<T>();
    public delegate void Setter<T>( T newValue );
}