using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Xna.Framework.Content;

namespace Xen2D
{
    public interface IXenCache<T>
    {
        T this[ int identifier ] { get; }
    }

    public interface IXenCache<T, U>
    {
        T this[ U identifier ] { get; }
    }

    /// <summary>
    /// This class contains a set of resources.  It assumes that resource data is static and will never change.
    /// The Cache should be constructed and initialized when the game/level is loaded.
    /// Texture enumeration definitions must inherit from integer and provide ContentIdentifierAttributes
    /// </summary>
    public class XenCache<T> : IXenCache<T>
    {
        private T[] _contentElements = null;
        private ContentManager _contentManager;

        public T this[ int identifier ]
        {
            get
            {
                return _contentElements[ identifier ];
            }
        }

        public XenCache( Type contentElementEnumeration ) : this( Globals.Content, contentElementEnumeration ) { }

        public XenCache( ContentManager content, Type contentElementEnumeration )
        {
            Debug.Assert( null != content, "Content Manager cannot be null" );

            _contentManager = content;
            Initialize( contentElementEnumeration );
        }

        private void Initialize( Type contentElementEnumeration )
        {
            FieldInfo[] fields = contentElementEnumeration.GetFields();

            //We use Length - 1 because fields[0] seems to contain a special artifact of reflection containing type information.
            _contentElements = new T[ fields.Length - 1 ];

            foreach( FieldInfo field in contentElementEnumeration.GetFields() )
            {
                foreach( Attribute attribute in field.GetCustomAttributes( true ) )
                {
                    if( attribute is ContentIdentifierAttribute )
                    {
                        string identifier = ( (ContentIdentifierAttribute)attribute ).Value;
                        _contentElements[ (int)field.GetValue( null ) ] = _contentManager.Load<T>( identifier );
                    }
                }
            }
        }
    }

    public abstract class XenCache<T, U> : XenCache<T>, IXenCache<T, U>
    {
        public abstract T this[ U identifier ] { get; }

        public XenCache( Type contentElementEnumeration ) : base( contentElementEnumeration ) { }
        public XenCache( ContentManager content, Type contentElementEnumeration ) : base( content, contentElementEnumeration ) { }
    }
}
