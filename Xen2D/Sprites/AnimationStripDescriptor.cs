using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XenAspects;

namespace Xen2D
{
    public interface IAnimationStripDescriptor : ITextureInfo
    {
        Rectangle FrameSize { get; }
        int CurrentFrame { get; set; }
        void NextFrame();
        void Reset( Texture2D texture, int numFrameRows, int numFrameColumns );
    }

    /// <summary>
    /// An animation strip consists of a single image that contains one or more frames.  
    /// Frames are identically sized sub-images that are swapped in sequence rapidly to 
    /// give the appearance of animation.
    /// </summary>
    public class AnimationStripDescriptor : ComposableObject<AnimationStripDescriptor>, IAnimationStripDescriptor
    {
        /// <summary>
        /// Creates a collection of frame rectangles to be used as source rectangles from an animation strip.
        /// Assumes the animation strip frames go in order from left to right, then top to bottom.  
        /// </summary>
        /// <param name="animationStrip">The rectangle representing the animation strip.</param>
        /// <param name="numFrameRows">The number of frame rows.</param>
        /// <param name="numFrameColumns">The number of frame columns.</param>
        /// <returns>A collection of source rectangles corresponding to the location of the frame texture.</returns>
        public static Rectangle[] GetFrameRects( Rectangle animationStrip, int numFrameRows, int numFrameColumns )
        {
            //We want the truncation behavior for integer division here
            int width = animationStrip.Width / numFrameColumns;
            int height = animationStrip.Height / numFrameRows;

            return GetFrameRects( animationStrip, new Rectangle( 0, 0, width, height ) );
        }

        /// <summary>
        /// Creates a collection of frame rectangles to be used as source rectangles from an animation strip.
        /// Assumes the animation strip frames go in order from left to right, then top to bottom.  
        /// </summary>
        /// <param name="animationStrip">The rectangle representing the animation strip.</param>
        /// <param name="frameSize">The size of a single frame.</param>
        /// <returns>A collection of source rectangles corresponding to the location of the frame texture.</returns>
        public static Rectangle[] GetFrameRects( Rectangle animationStrip, Rectangle frameSize )
        {
            Rectangle[] frameRects = null;
            if( ( frameSize.Width > animationStrip.Width ) || ( frameSize.Height > animationStrip.Height ) )
            {
                frameRects = new Rectangle[ 1 ] { animationStrip };
            }
            else
            {
                frameSize = new Rectangle( 0, 0, frameSize.Width, frameSize.Height );

                int numColumns = (int)( animationStrip.Width / frameSize.Width );
                int numRows = (int)( animationStrip.Height / frameSize.Height );

                frameRects = new Rectangle[ numColumns * numRows ];

                for( int j = 0; j < numRows; j++ )
                {
                    int indexOfFirstInColumn = j * numColumns;
                    for( int i = 0; i < numColumns; i++ )
                    {
                        frameRects[ indexOfFirstInColumn + i ] =
                            new Rectangle( i * frameSize.Width, j * frameSize.Height, frameSize.Width, frameSize.Height );
                    }
                }

            }
            return frameRects;
        }

        public static AnimationStripDescriptor Acquire( Texture2D texture, int numFrameRows, int numFrameColumns )
        {
            var instance = Pool.Acquire();
            instance.Reset( texture, numFrameRows, numFrameColumns );
            return instance;
        }

        private int                     _currentFrame;
        private int                     _numFrames;
        private int                     _numFrameColumns;
        private Rectangle               _referenceFrameRect;
        private CachedTextureDescriptor TextureDescriptor { get; set; }

        public int NumFrames
        {
            get { return _numFrames; }
        }

        public int CurrentFrame
        {
            get { return _currentFrame; }
            set { _currentFrame = value; }
        }

        public Rectangle FrameSize
        {
            get { return _referenceFrameRect; }
        }

        #region ITextureInfo Members

        public Texture2D Asset
        {
            get { return TextureDescriptor.Asset; }
        }

        public Rectangle SourceRectangle
        {
            get 
            {
                Rectangle sourceRectangle = _referenceFrameRect;
                int column = _currentFrame % _numFrameColumns;
                int row = (int)_currentFrame / _numFrameColumns;
                sourceRectangle.X += column * sourceRectangle.Width;
                sourceRectangle.Y += row * sourceRectangle.Height;
                return sourceRectangle; 
            }
        }

        #endregion

        public AnimationStripDescriptor()
        {
            TextureDescriptor = new CachedTextureDescriptor();
        }

        public void Reset( Texture2D texture, int numFrameRows, int numFrameColumns )
        {
            TextureDescriptor.Reset( texture );
            _numFrames = numFrameRows * numFrameColumns;
            _numFrameColumns = numFrameColumns;
            _referenceFrameRect = new Rectangle( 0, 0, texture.Width / numFrameColumns, texture.Height / numFrameRows );
        }

        public void NextFrame()
        {
            if( CurrentFrame == ( NumFrames - 1 ) )
            {
                //we are on the last frame
                CurrentFrame = 0;
                return;
            }
            CurrentFrame++;
        }
    }
}
