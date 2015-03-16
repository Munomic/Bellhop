using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    public interface IViewport2D : IRectangularExtent
    {
        void MoveBy( Vector2 moveBy );
        void MoveTo( Vector2 moveTo );
        void MoveHorizontal( float amount );
        void MoveVertical( float amount );

        void RotateBy( float rotateBy );
        void RotateTo( float rotateTo );

        void ZoomIn( float zoomFactor );
        void ZoomOut( float zoomFactor );

        void ResetWithScreenSize( int width, int height );
    }

    public class Viewport2D : RectangularExtentBase<Viewport2D>, IViewport2D
    {
        public void MoveBy( Vector2 moveBy )
        {
            Anchor += moveBy;
        }

        public void MoveTo( Vector2 moveTo )
        {
            Anchor = moveTo;
        }

        public void MoveHorizontal( float amount )
        {
            Anchor = Anchor + XenMath.Rotate( new Vector2( amount, 0 ), Angle );
        }

        public void MoveVertical( float amount )
        {
            Anchor = Anchor + XenMath.Rotate( new Vector2( 0, amount ), Angle );
        }

        public void RotateBy( float rotateBy )
        {
            Angle += rotateBy;
        }

        public void RotateTo( float rotateTo )
        {
            Angle = rotateTo;
        }

        public void ZoomIn( float zoomFactor )
        {
            Scale /= zoomFactor;
        }

        public void ZoomOut( float zoomFactor )
        {
            Scale *= zoomFactor;
        }

        public void ResetWithScreenSize( int width, int height )
        {
            Reset( width, height );
            Origin = new Vector2( width / 2, height / 2 );
        }
    }
}
