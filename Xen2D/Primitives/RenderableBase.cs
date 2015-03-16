using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    /// <summary>
    /// This class serves as the base for any object that renders/updates.
    /// </summary>
    /// <typeparam name="T">The parameterized type.</typeparam>
    public abstract class RenderableBase<T> : ComposableObject<T>
        where T : ComposableObject, new()
    {
        public const int DefaultUpdateOrder = 0;

        /// <summary>
        /// Gets or sets whether to update this object.
        /// </summary>
        public bool Enabled { get; set; }
        public int UpdateOrder { get; set; }

        /// <summary>
        /// Gets or sets whether to draw this object.
        /// </summary>
        public bool Visible { get; set; }

        public void Update( GameTime gameTime ) 
        {
            if( Enabled )
                UpdateInternal( gameTime );
        }

        protected virtual void UpdateInternal( GameTime gameTime ) { }

        protected override void ResetDirectState()
        {
            base.ResetDirectState();
            Visible = true;
            Enabled = true;
            UpdateOrder = DefaultUpdateOrder;
        }
    }
}
