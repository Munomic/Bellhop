using XenAspects;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    public struct AnimationLoopEventArgs
    {
        public IAnimatedSprite Sprite;
        public int NumLoopsCompleted;

        public AnimationLoopEventArgs( IAnimatedSprite sprite, int numLoopsCompleted )
        {
            Sprite = sprite;
            NumLoopsCompleted = numLoopsCompleted;
        }
    }

    public interface IAnimatedSprite : IComposableObject
    {
    /// <summary>
        /// This event is fired when an animation loop completes.
        /// </summary>
        IEvent<AnimationLoopEventArgs> OnAnimationLoopCompleted { get; }

        /// <summary>
        /// Gets or sets the frames per second for this animation.
        /// </summary>
        float FramesPerSecond { get; set; }

        /// <summary>
        /// Gets the current the animated sprite is on.
        /// </summary>
        int CurrentLoop { get; }
    }
}
