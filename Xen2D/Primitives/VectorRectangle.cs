using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using XenAspects;

namespace Xen2D
{
    public class VectorRectangle : ComposableObject<VectorRectangle>, IRectangle
    {
        public static VectorRectangle Create( Vector2 size )
        {
            return Acquire( size.X, size.Y );
        }

        private static List<Vector2> _pointListCache = new List<Vector2>( 16 );

        public static VectorRectangle Acquire( float width, float height )
        {
            var instance = Acquire();
            instance.Reset( width, height );
            return instance;
        }

        public VectorRectangle() { }

        public void Reset( float width, float height )
        {
            Width = width;
            Height = height;
            _pointListCache.Clear();
            _pointListCache.Add( Vector2.Zero );                    //top left
            _pointListCache.Add( new Vector2( width, 0 ) );         //top right
            _pointListCache.Add( new Vector2( width, height ) );    //bottom right
            _pointListCache.Add( new Vector2( 0, height ) );        //bottom left
            _polygon.Reset( _pointListCache );
        }

        private ShapePolygon _polygon = ShapePolygon.Acquire();

        public float Width{ get; private set; }
        public float Height{ get; private set; }

        public Vector2 TopLeft { get { return _polygon.Vertices[ 0 ]; } }
        public Vector2 TopRight { get { return _polygon.Vertices[ 1 ]; } }
        public Vector2 BottomLeft { get { return _polygon.Vertices[ 2 ]; } }
        public Vector2 BottomRight { get { return _polygon.Vertices[ 3 ]; } }

        #region IPolygonShape Members

        public int NumSides
        {
            get { return _polygon.NumSides; }
        }

        public Vector2[] Vertices
        {
            get { return _polygon.Vertices; }
        }

        #endregion

        #region IShape Members

        public Vector2 Center
        {
            get { return _polygon.Center; }
        }

        public bool Intersects( IPolygon2D other )
        {
            return _polygon.Intersects( other );
        }

        public bool Intersects( ICircle2D other )
        {
            return _polygon.Intersects( other );
        }

        #endregion

        #region IRegion Members

        public bool Contains( Vector2 point )
        {
            return ( ( point.X >= 0 ) && ( point.X <= Width ) ) &&
                   ( ( point.Y >= 0 ) && ( point.Y <= Height ) );
        }

        public Vector2 FindClosestPoint( Vector2 point )
        {
            return _polygon.FindClosestPoint( point );
        }

        #endregion
    }
}
