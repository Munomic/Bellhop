using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    public interface IBasicLine2D : IRenderable2D
    {
        Color Color { get; set; }

        float Angle { get; set; }
        float Length { get; set; }
        int Thickness { get; set; }

        Vector2 Start { get; set; }
        Vector2 Stop { get; set; }
    }

    /// <summary>
    /// This class is responsible for loading, drawing, and positioning of a 2d line segment 2d.  
    /// </summary>
    public sealed class BasicLine2D : Renderable2DBase<BasicLine2D>, IBasicLine2D
    {
        private Vector2 _start;
        private Vector2 _stop;
        private float _lastAngle = 0;

        public BasicLine2D() : this( VectorUtility.Zero, Vector2.UnitX ) { }

        public BasicLine2D( Vector2 start, Vector2 stop ) : this( start, stop, Color.Black ) { }

        public BasicLine2D( Vector2 start, Vector2 stop, Color color ) : this( start, stop, color, 1 ) { }

        public BasicLine2D( Vector2 start, Vector2 stop, Color color, int thickness )
        {
            _start = start;
            _stop = stop;
            Color = color;
            Thickness = thickness;
        }

        public Color Color { get; set; }
        public int Thickness { get; set; }

        public Vector2 Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Vector2 Stop
        {
            get { return _stop; }
            set { _stop = value; }
        }

        /// <summary>
        /// Gets or sets the angle of from Start to Stop.  Angle 0 is ( 1, 0 )
        /// </summary>
        public float Angle
        {
            get
            {
                if( Length == 0 )
                {
                    return _lastAngle;
                }
                else
                {
                    return XenMath.GetAngleFloat( Stop - Start );
                }
            }
            set
            {
                float length = Vector2.Distance( Start, Stop );
                Stop = new Vector2(
                    XenMath.RoundDoubleToFloat( ( length * Math.Cos( value ) ) ),
                    XenMath.RoundDoubleToFloat( ( length * Math.Sin( value ) ) ) );
            }
        }

        public float Length
        {
            get
            {
                return Vector2.Distance( Start, Stop );
            }
            set
            {
                if( value == 0 )
                {
                    _lastAngle = Angle;
                    Stop = Start;
                }
                else
                {
                    float length = Length;
                    float angle = Angle;
                    if( length == 0 )
                    {
                        //length 0 will not yield a valid angle.
                        angle = _lastAngle;
                    }
                    Stop = Start + value * XenMath.Rotate( Vector2.UnitX, angle );
                }
            }
        }

        protected override void DrawInternal( SpriteBatch spriteBatch, GameTime gameTime, Matrix transformFromWorldToCamera )
        {
            //TODO: basic line 2d should have its own SpriteDisplayAttributes
            spriteBatch.DrawLine( Color, this, SpriteDisplayAttributeDefaults.Default, transformFromWorldToCamera );
        }
    }
}
