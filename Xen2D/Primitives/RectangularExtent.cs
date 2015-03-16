using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    public abstract class RectangularExtentBase<DerivedType> : PolygonExtentBase<DerivedType>, IRectangularExtent
        where DerivedType : ComposableObject, new()
    {
        private Vector2[] _vertexCache = new Vector2[ 4 ];

        public Vector2 ReferenceTopLeft { get { return ReferenceRegion.Vertices[ 0 ]; } }
        public Vector2 ReferenceTopRight { get { return ReferenceRegion.Vertices[ 1 ]; } }
        public Vector2 ReferenceBottomLeft { get { return ReferenceRegion.Vertices[ 3 ]; } }
        public Vector2 ReferenceBottomRight { get { return ReferenceRegion.Vertices[ 2 ]; } }

        public Vector2 ActualTopLeft { get { return Transform.TranslateVectorFromThisSpaceToAbsolute( ReferenceTopLeft ); } }
        public Vector2 ActualTopRight { get { return Transform.TranslateVectorFromThisSpaceToAbsolute( ReferenceTopRight ); } }
        public Vector2 ActualBottomLeft { get { return Transform.TranslateVectorFromThisSpaceToAbsolute( ReferenceBottomLeft ); } }
        public Vector2 ActualBottomRight { get { return Transform.TranslateVectorFromThisSpaceToAbsolute( ReferenceBottomRight ); } }

        public float ReferenceWidth { get { return Vector2.Distance( ReferenceTopLeft, ReferenceTopRight ); } }
        public float ReferenceHeight { get { return Vector2.Distance( ReferenceTopLeft, ReferenceBottomLeft ); } }

        #region Reset Methods
        public void Reset( Vector2 size )
        {
            Reset( size.X, size.Y );
        }

        public void Reset( float width, float height )
        {
            Reset( VectorUtility.Zero, width, height );
        }

        public void Reset( float width, float height, Vector2 origin )
        {
            Reset( VectorUtility.Zero, width, height, origin );
        }

        public void Reset( Vector2 size, Vector2 origin )
        {
            Reset( VectorUtility.Zero, size.X, size.Y, origin );
        }

        public void Reset( float topLeftX, float topLeftY, float width, float height )
        {
            Reset( new Vector2( topLeftX, topLeftY ), width, height );
        }

        public void Reset( Vector2 anchor, float width, float height )
        {
            Reset( anchor, width, height, VectorUtility.Zero );
        }

        public virtual void Reset( Vector2 anchor, float width, float height, Vector2 origin )
        {
            base.Reset();
            _vertexCache[ 0 ] = new Vector2( 0, 0 );
            _vertexCache[ 1 ] = new Vector2( width, 0 );
            _vertexCache[ 2 ] = new Vector2( width, height );
            _vertexCache[ 3 ] = new Vector2( 0, height );

            ReferenceRegion.Reset( _vertexCache, 4 );
            Anchor = anchor;
            Origin = origin;
            RecalculateBounds();
        }
        #endregion
    }

    public class RectangularExtent : RectangularExtentBase<RectangularExtent>
    {
        public static RectangularExtent Acquire( float topLeftX, float topLeftY, float width, float height )
        {
            var instance = _pool.Acquire();
            instance.Reset( topLeftX, topLeftY, width, height );
            return instance;
        }
    }
}