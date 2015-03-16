using System;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    public class TextureSizeAttribute : Attribute
    {
        public Vector2 Size { get; private set; }

        public TextureSizeAttribute( uint width, uint height )
        {
            Size = new Vector2( width, height );
        }
    }
}