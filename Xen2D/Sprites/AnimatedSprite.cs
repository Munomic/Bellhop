using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XenAspects;

namespace Xen2D
{
    /// <summary>
    /// This class is responsible for drawing an animated 2d sprite.  Animation here refers to rapidly swapping out the 
    /// source rectangle from an animation strip texture.  
    /// </summary>
    public class AnimatedSprite : Sprite2D<AnimatedSprite>, IAnimatedSprite
    {
        public void ResetFromCenter( Texture2D animationStrip, int numRows, int numColumns, Vector2 center )
        {
            Vector2 origin = new Vector2( animationStrip.Width / ( 2 * numColumns ), animationStrip.Height / ( 2 * numRows ) );
            Reset( animationStrip, numRows, numColumns, center - origin, origin );
        }

        public void ResetFromTopLeft( Texture2D animationStrip, int numRows, int numColumns, Vector2 topLeft )
        {
            Reset( animationStrip, numRows, numColumns, topLeft, Vector2.Zero );
        }

        public void Reset( Texture2D animationStrip, int numRows, int numColumns, Vector2 anchor, Vector2 origin )
        {
            AnimationStripAsset.Reset( animationStrip, numRows, numColumns );
            Reset();
            RenderingExtent.Reset( animationStrip.Width / numColumns, animationStrip.Height / numRows );
            RenderingExtent.Anchor = anchor;
            RenderingExtent.Origin = origin;
        }

        protected IEvent<AnimationLoopEventArgs> _animationLoopCompleted = new Event<AnimationLoopEventArgs>();

        protected float     _timeOnCurrentFrame     = 0.0f;
        protected int       _currentLoop            = 0;
        protected float     _millisecondsPerFrame   = 1000.0f / 30.0f;

        public float FramesPerSecond
        {
            get { return 1000.0f / _millisecondsPerFrame; }
            set
            {
                if( value <= 0 )
                {
                    throw new ArgumentException( "Framerate cannot be <= 0 " );
                }
                else
                {
                    _millisecondsPerFrame = 1000.0f / value;
                }
            }
        }

        public override ITextureInfo TextureInfo{ get{ return AnimationStripAsset; } }
        public int CurrentLoop { get { return _currentLoop; } }
        public IEvent<AnimationLoopEventArgs> OnAnimationLoopCompleted { get { return _animationLoopCompleted; } }

        public AnimatedSprite() 
        {
            AnimationStripAsset = new AnimationStripDescriptor();
        }

        protected AnimationStripDescriptor AnimationStripAsset { get; set; }

        protected override void ResetDirectState()
        {
            base.ResetDirectState();
            AnimationStripAsset.CurrentFrame = 0;
            _timeOnCurrentFrame = 0.0f;
            _currentLoop = 0;
        }

        protected override void UpdateInternal( GameTime gameTime )
        {
            // How many milliseconds since the last time the game's Update() method was called
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Accumulate time this frame has been active
            _timeOnCurrentFrame += elapsedTime;

            // If the time active for this frame is more than the time allowed...
            if( _timeOnCurrentFrame >= _millisecondsPerFrame )
            {
                // Reset the frame timer
                _timeOnCurrentFrame = 0.0f;

                AnimationStripAsset.NextFrame();
                if( AnimationStripAsset.CurrentFrame == 0 )
                {
                    //Animation has finished a loop
                    _currentLoop++;
                    _animationLoopCompleted.Notify( new AnimationLoopEventArgs( this, _currentLoop ) );
                }
            }
        }
    }
}
