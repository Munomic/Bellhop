using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Xen2D;

namespace DuckyEngine
{
    public class DuckyGroup : DuckyElement
    {
        Dictionary<string, DuckyElement> _elements = new Dictionary<string, DuckyElement>();
        List<string> _elementKeys = new List<string>();
        public DuckyElement this[ string element ]
        {
            get
            {
                if( _elements.ContainsKey( element ) )
                {
                    return _elements[ element ];
                }
                return null;
            }
        }
        public List<string> Keys { get { return _elementKeys; } }
        public int Count { get { return _elementKeys.Count; } }

        internal override void Load( XElement group )
        {
            Unload();
            this.Enabled = true;
            this.Visible = true;
            this.Color = Color.White;

            float lowestX = float.MaxValue;
            float lowestY = float.MaxValue;
            float highestX = float.MinValue;
            float highestY = float.MinValue;

            IEnumerable<XElement> xelements = group.Elements( "Element" );
            int untitledElementNumber = 0;
            foreach( XElement xelement in xelements )
            {
                if( xelement.Attribute( "Type" ) == null )
                {
                    throw new ArgumentException( "Type Missing for Element in Page " + this.Name );
                }
                else
                {
                    string elementName;
                    if( xelement.Attribute( "Name" ) != null )
                    {
                        elementName = xelement.Attribute( "Name" ).Value;
                    }
                    else
                    {
                        elementName = "Untitled_" + untitledElementNumber;
                        untitledElementNumber++;
                    }
                    DuckyElement element;
                    string type = xelement.Attribute( "Type" ).Value;
                    switch( type )
                    {
                        case "Image":
                            element = new DuckyImage();
                            element.ContentManager = this.ContentManager;
                            element.Load( xelement );
                            break;
                        case "Text":
                            element = new DuckyText();
                            element.ContentManager = this.ContentManager;
                            element.Load( xelement );
                            break;
                        case "Particle":
                            element = new DuckyParticle();
                            element.ContentManager = this.ContentManager;
                            element.Load( xelement );
                            break;
                        default:
                            throw new NotSupportedException( "Unsupported Type " + type );
                    }
                    element.Name = elementName;
                    _elements.Add( elementName, element );
                    _elementKeys.Add( elementName );

                    if( element.Extent.LowestX < lowestX )
                    {
                        lowestX = element.Extent.LowestX;
                    }
                    if( element.Extent.LowestY < lowestY )
                    {
                        lowestY = element.Extent.LowestY;
                    }
                    if( element.Extent.HighestX > highestX )
                    {
                        highestX = element.Extent.HighestX;
                    }
                    if( element.Extent.HighestY > highestY )
                    {
                        highestY = element.Extent.HighestY;
                    }
                }
            }

            this.Extent = new RectangularExtent();
            float x = 0.0f;
            float y = 0.0f;
            if( group.Attribute( "X" ) != null )
            {
                x = float.Parse( group.Attribute( "X" ).Value );
            }
            if( group.Attribute( "Y" ) != null )
            {
                y = float.Parse( group.Attribute( "Y" ).Value );
            }
            this.Extent.Reset( new Vector2( lowestX, lowestY ), highestX - lowestX, highestY - lowestY );
            if( group.Attribute( "Angle" ) != null )
            {
                this.Extent.Angle = MathHelper.ToRadians( float.Parse( group.Attribute( "Angle" ).Value ) );
            }
            if( group.Attribute( "OriginX" ) != null )
            {
                this.OriginX = float.Parse( group.Attribute( "OriginX" ).Value );
            }
            if( group.Attribute( "OriginY" ) != null )
            {
                this.OriginY = float.Parse( group.Attribute( "OriginY" ).Value );
            }
            // Set position again to account for any Origin values
            this.Extent.Anchor = new Vector2( x, y );
            float scale = 1.0f;
            if( group.Attribute( "Scale" ) != null )
            {
                scale = float.Parse( group.Attribute( "Scale" ).Value );
            }
            this.Scale = new Vector2( scale, scale );
            if( group.Attribute( "Enabled" ) != null )
            {
                this.Enabled = bool.Parse( group.Attribute( "Enabled" ).Value );
            }
            if( group.Attribute( "Visible" ) != null )
            {
                this.Visible = bool.Parse( group.Attribute( "Visible" ).Value );
            }

            // Reverse the Z-Order of the elements
            _elementKeys.Reverse();
        }

        internal override void Unload()
        {
            foreach( DuckyElement element in _elements.Values )
            {
                element.Unload();
            }
            _elementKeys.Clear();
            _elements.Clear();
        }

        internal override void Render( ResizedSpriteBatch sb )
        {
            if( Visible )
            {
                Matrix transformation = MatrixUtility.CreateTransform( Scale, Angle, Position );
                for( int i = 0; i < _elementKeys.Count; i++ )
                {
                    _elements[ _elementKeys[ i ] ].Render( sb, transformation );
                }
            }
            base.Render( sb );
        }

        internal override void Render( ResizedSpriteBatch sb, Matrix transform )
        {
            if( Visible )
            {
                Matrix transformation = transform * MatrixUtility.CreateTransform( Scale, Angle, Position );
                for( int i = 0; i < _elementKeys.Count; i++ )
                {
                    _elements[ _elementKeys[ i ] ].Render( sb, transformation );
                }
            }
            base.Render( sb );
        }
    }
}
