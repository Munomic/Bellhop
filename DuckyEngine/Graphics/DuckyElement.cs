using System;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xen2D;

namespace DuckyEngine
{
    public interface IDuckyElement
    {
        void Load( XElement element );
        void Unload();
        void Update( float timeDiff );
        void Render( ResizedSpriteBatch sb );
    }

    public abstract class DuckyElement
    {
#if MONOTOUCH || ANDROID
        internal List<int> TouchIds = new List<int>();
#endif
        internal ContentManager ContentManager { get; set; }
        internal Vector2 TouchPosition { get; set; }
        internal bool IsTapped { get; set; }
        internal RectangularExtent Extent { get; set; }
        public string Name { get; set; }
        public float X
        {
            get { return Extent.Anchor.X; }
            set { Extent.Anchor = new Vector2( value, Extent.Anchor.Y ); }
        }
        public float Y
        {
            get { return Extent.Anchor.Y; }
            set { Extent.Anchor = new Vector2( Extent.Anchor.X, value ); }
        }
        public Vector2 Position
        {
            get { return Extent.Anchor; }
            set { Extent.Anchor = value; }
        }
        public float OriginX
        {
            get { return Extent.Origin.X; }
            set { Extent.Origin = new Vector2( value, Extent.Origin.Y ); }
        }
        public float OriginY
        {
            get { return Extent.Origin.Y; }
            set { Extent.Origin = new Vector2( Extent.Origin.X, value ); }
        }
        public float Angle
        {
            get { return Extent.Angle; }
            set { Extent.Angle = value; }
        }
        public float Width
        {
            get { return Extent.ReferenceWidth * Extent.Scale.X; }
        }
        public float Height
        {
            get { return Extent.ReferenceHeight * Extent.Scale.Y; }
        }
        public Vector2 Scale
        {
            get { return Extent.Scale; }
            set { Extent.Scale = value; }
        }
        public Color Color { get; set; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public Action<int, Vector2> OnTouchDown { get; set; }
        public Action<int, Vector2, bool> OnTouchMove { get; set; }
        public Action<int, Vector2, bool> OnTouchUp { get; set; }
        public Action<DuckyElement, float> OnUpdate { get; set; }
        public Action OnDraw { get; set; }

        internal abstract void Load( XElement element );
        internal abstract void Unload();
        internal virtual void Update( float timeDiff )
        {
            if( OnUpdate != null )
            {
                OnUpdate( this, timeDiff );
            }
        }

        internal virtual void Render( ResizedSpriteBatch sb )
        {
            if( Visible )
            {
                if( OnDraw != null )
                {
                    OnDraw();
                }
            }
        }

        internal virtual void Render( ResizedSpriteBatch sb, Matrix transform )
        {
            if( Visible )
            {
                if( OnDraw != null )
                {
                    OnDraw();
                }
            }
        }
    }
}
