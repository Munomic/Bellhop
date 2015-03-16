using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Xen2D;

namespace DuckyEngine
{
    public class DuckySceneController
    {
        internal DuckyScene _scene;
        public DuckyScene Scene { get { return _scene; } }

        public virtual void Initialize( DuckyScene scene )
        {
            _scene = scene;
        }

        public virtual void Deinitialize()
        {
            _scene = null;
        }

        public virtual void Update( float timeDiff )
        {
        }
    }

    public class DuckyScene
    {
        DuckySceneController _controller = null;
        ContentManager _contentManager;
        float _swipeThreshold = 25.0f;
        internal bool IsTapped { get; set; }
        internal Vector2 TapPosition { get; set; }

        Dictionary<string, DuckyPage> _pages = new Dictionary<string, DuckyPage>();
        List<string> _pageKeys = new List<string>();
        public DuckyPage this[ string page ]
        {
            get
            {
                if( _pages.ContainsKey( page ) )
                {
                    return _pages[ page ];
                }
                return null;
            }
        }
        public List<string> Keys { get { return _pageKeys; } }
        public int Count { get { return _pageKeys.Count; } }
        public int CurrentPage { get; set; }
        public DuckySceneController Controller
        {
            get { return _controller; }
            set
            {
                // Deinitialize and detach
                if( _controller != null )
                {
                    _controller.Deinitialize();
                    _controller = null;
                }
                // Attach and initialize
                _controller = value;
                if( _controller != null )
                {
                    _controller.Initialize( this );
                }
            }
        }

        internal DuckyScene()
        {
            _contentManager = new ContentManager( Globals.Content.ServiceProvider, Globals.Content.RootDirectory );
            CurrentPage = 0;
        }

        internal virtual void Load( XElement scene )
        {
            Unload();
            if( scene.Attribute( "SwipeThreshold" ) != null )
            {
                _swipeThreshold = float.Parse( scene.Attribute( "SwipeThreshold" ).Value );
            }
            IEnumerable<XElement> xpages = scene.Elements( "Page" );
            int untitledPageNumber = 0;
            foreach( XElement xpage in xpages )
            {
                string pageName;
                if( xpage.Attribute( "Name" ) != null )
                {
                    pageName = xpage.Attribute( "Name" ).Value;
                }
                else
                {
                    pageName = "Untitled_" + untitledPageNumber;
                    untitledPageNumber++;
                }
                DuckyPage page = new DuckyPage();
                page.Name = pageName;
                page.ContentManager = _contentManager;
                page.Load( xpage );
                _pages.Add( pageName, page );
                _pageKeys.Add( pageName );
            }
        }

        internal virtual void Unload()
        {
            foreach( DuckyPage page in _pages.Values )
            {
                page.Unload();
            }
            _pages.Clear();
            _pageKeys.Clear();
            _contentManager.Unload();
        }

        internal virtual void Update( float timeDiff )
        {
            if( CurrentPage < Count )
            {
                _pages[ _pageKeys[ CurrentPage ] ].Update( timeDiff );
            }

            if( _pageKeys.Count > 1 )
            {
                if( !this.IsTapped && Mouse.GetState().LeftButton == ButtonState.Pressed )
                {
                    // Check that no element was tapped
                    bool elementTapped = false;
                    DuckyPage page = _pages[ _pageKeys[ CurrentPage ] ];
                    for( int i = 0; i < page.Count; i++ )
                    {
                        DuckyElement element = page[ page.Keys[ i ] ];
                        if( element.IsTapped )
                        {
                            elementTapped = true;
                            break;
                        }
                    }
                    if( !elementTapped )
                    {
                        this.IsTapped = true;
                        MouseState state = Mouse.GetState();
                        TapPosition = new Vector2( state.X, state.Y );
                    }
                }
                else if( this.IsTapped && Mouse.GetState().LeftButton == ButtonState.Released )
                {
                    this.IsTapped = false;
                    // Check if the horizontal movement is far enough for a swipe
                    MouseState state = Mouse.GetState();
                    if( state.X - this.TapPosition.X > _swipeThreshold )
                    {
                        // Swiped left -> right
                        if( CurrentPage > 0 )
                        {
                            CurrentPage--;
                        }
                    }
                    else if( this.TapPosition.X - state.X > _swipeThreshold )
                    {
                        // Swiped right -> left
                        if( CurrentPage < Count - 1 )
                        {
                            CurrentPage++;
                        }
                    }
                }
            }

            if( _controller != null )
            {
                _controller.Update( timeDiff );
            }
        }

        internal virtual void Render( ResizedSpriteBatch sb )
        {
            if( CurrentPage < Count )
            {
                _pages[ _pageKeys[ CurrentPage ] ].Render( sb );
            }
        }
    }
}
