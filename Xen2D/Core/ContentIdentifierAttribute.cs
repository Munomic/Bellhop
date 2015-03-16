using System;

namespace Xen2D
{
    public class ContentIdentifierAttribute : Attribute
    {
        public string Value { get; set; }

        public ContentIdentifierAttribute( string value )
        {
            Value = value;
        }
    }
}