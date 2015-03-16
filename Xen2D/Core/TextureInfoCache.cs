using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    /// <summary>
    /// Cache containing texture information for a given texture set.
    /// </summary>
    /// <typeparam name="T">The Texture Enumeration</typeparam>
    public class TextureInfoCache
    {
        private Vector2[] _contentElements = null;

        public Vector2 this[ int identifier ]
        {
            get
            {
                return _contentElements[ identifier ];
            }
        }

        public TextureInfoCache( Type contentElementEnumeration )
        {
            FieldInfo[] fields = contentElementEnumeration.GetFields();

            //We use Length - 1 because fields[0] seems to contain a special artifact of reflection containing type information.
            _contentElements = new Vector2[ fields.Length - 1 ];

            foreach( FieldInfo field in contentElementEnumeration.GetFields() )
            {
                foreach( Attribute attribute in field.GetCustomAttributes( true ) )
                {
                    if( attribute is TextureSizeAttribute )
                    {
                        _contentElements[ (int)field.GetValue( null ) ] = ( (TextureSizeAttribute)attribute ).Size;
                    }
                }
            }
        }
    }

    public abstract class TextureInfoCache<T> : TextureInfoCache
    {
        public TextureInfoCache() : base( typeof( T ) ) { }
    }
}
