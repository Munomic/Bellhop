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
    public class DuckyPage
    {
        internal ContentManager ContentManager { get; set; }
        public string Name { get; set; }

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

        internal virtual void Load( XElement page )
        {
            Unload();
            IEnumerable<XElement> xelements = page.Elements( "Element" );
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
                        case "Group":
                            element = new DuckyGroup();
                            element.ContentManager = this.ContentManager;
                            element.Load( xelement );
                            break;
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
                }
            }

            // Reverse the Z-Order of the elements
            _elementKeys.Reverse();
        }

        internal virtual void Unload()
        {
            foreach( DuckyElement element in _elements.Values )
            {
                element.Unload();
            }
            _elementKeys.Clear();
            _elements.Clear();
        }

        internal virtual void Update( float timeDiff )
        {
            // TODO: Z-Order Tap Processing
            for( int i = 0; i < _elementKeys.Count; i++ )
            {
                DuckyElement element = _elements[ _elementKeys[ i ] ];
                if( element.Enabled )
                {
                    if( element.IsTapped )
                    {
#if MONOTOUCH || ANDROID
                        TouchCollection touches = TouchPanel.GetState();
                        if( touches.Count > 0 )
                        {
                            for( int j = 0; j < touches.Count; j++ )
                            {
                                TouchLocation touch = touches[ j ];
                                Vector2 currentPosition = new Vector2( touch.Position.X, touch.Position.Y );

                                if( touch.State == TouchLocationState.Pressed ||
                                    touch.State == TouchLocationState.Moved )
                                {
                                    element.TouchPosition = currentPosition;
                                    bool doesTouchExist = false;
                                    for( int k = 0; k < element.TouchIds.Count; k++ )
                                    {
                                        if( element.TouchIds[ k ] == touch.Id )
                                        {
                                            doesTouchExist = true;
                                            break;
                                        }
                                    }
                                    if( !doesTouchExist )
                                    {
                                        element.TouchIds.Add( touch.Id );
                                        if( element is DuckyParticle || element.Extent.Contains( currentPosition ) )
                                        {
                                            if( element.OnTouchDown != null )
                                            {
                                                element.OnTouchDown( touch.Id, currentPosition );
                                            }
                                        }
                                    }
                                    if( element.OnTouchMove != null )
                                    {
                                        element.OnTouchMove( touch.Id, currentPosition, element.Extent.Contains( currentPosition ) );
                                    }
                                }
                                else if( touch.State == TouchLocationState.Released ||
                                         touch.State == TouchLocationState.Invalid )
                                {
                                    element.IsTapped = ( touches.Count > 1 );
                                    element.TouchPosition = currentPosition;
                                    for( int k = 0; k < element.TouchIds.Count; k++ )
                                    {
                                        if( element.TouchIds[ k ] == touch.Id )
                                        {
                                            element.TouchIds.RemoveAt( k );
                                            break;
                                        }
                                    }
                                    if( element.OnTouchUp != null )
                                    {
                                        element.OnTouchUp( touch.Id, currentPosition, element.Extent.Contains( currentPosition ) );
                                    }
                                }
                            }
                        }
                        else
                        {
                            element.IsTapped = false;
                            for( int j = 0; j < element.TouchIds.Count; j++ )
                            {
                                // Assume it was released without us knowing
                                if( element.OnTouchUp != null )
                                {
                                    element.OnTouchUp( element.TouchIds[ j ], element.TouchPosition, element.Extent.Contains( element.TouchPosition ) );
                                }
                            }
                            element.TouchIds.Clear();
                        }
#elif WINDOWS
                        MouseState state = Mouse.GetState();
                        Vector2 currentPosition = Vector2.Transform( new Vector2( state.X, state.Y ), Matrix.Invert( SpriteBatchEx.ScreenSizeTransform ) );

                        if( state.LeftButton == ButtonState.Pressed )
                        {
                            element.TouchPosition = currentPosition;
                            if( element.OnTouchMove != null )
                            {
                                element.OnTouchMove( 0, currentPosition, element.Extent.Contains( currentPosition ) );
                            }
                        }
                        else if( state.LeftButton == ButtonState.Released )
                        {
                            element.IsTapped = false;
                            element.TouchPosition = currentPosition;
                            if( element.OnTouchUp != null )
                            {
                                element.OnTouchUp( 0, currentPosition, element.Extent.Contains( currentPosition ) );
                            }
                        }
#endif
                    }
                    else
                    {
#if MONOTOUCH || ANDROID
                        TouchCollection touches = TouchPanel.GetState();
                        if( touches.Count > 0 )
                        {
                            for( int j = 0; j < touches.Count; j++ )
                            {
                                TouchLocation touch = touches[ j ];
                                Vector2 currentPosition = new Vector2( touch.Position.X, touch.Position.Y );

                                if( touch.State == TouchLocationState.Pressed ||
                                    touch.State == TouchLocationState.Moved )
                                {
                                    if( element is DuckyParticle || element.Extent.Contains( currentPosition ) )
                                    {
                                        element.IsTapped = true;
                                        element.TouchPosition = currentPosition;
                                        element.TouchIds.Add( touch.Id );
                                        if( element.OnTouchDown != null )
                                        {
                                            element.OnTouchDown( touch.Id, currentPosition );
                                        }
                                    }
                                }
                            }
                        }
#elif WINDOWS
                        MouseState state = Mouse.GetState();
                        Vector2 currentPosition = Vector2.Transform( new Vector2( state.X, state.Y ), Matrix.Invert( SpriteBatchEx.ScreenSizeTransform ) );

                        if( state.LeftButton == ButtonState.Pressed )
                        {
                            if( element is DuckyParticle || element.Extent.Contains( currentPosition ) )
                            {
                                element.IsTapped = true;
                                element.TouchPosition = currentPosition;
                                if( element.OnTouchDown != null )
                                {
                                    element.OnTouchDown( 0, currentPosition );
                                }
                            }
                        }
#endif
                    }
                }
                element.Update( timeDiff );
            }
        }

        internal virtual void Render( ResizedSpriteBatch sb )
        {
            for( int i = 0; i < _elementKeys.Count; i++ )
            {
                _elements[ _elementKeys[ i ] ].Render( sb );
            }
        }
    }
}
