using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xen2D;

namespace DuckyEngine
{
    public class DuckyImage : DuckyElement
    {
        public Texture2D Texture { get; set; }
        public Rectangle? Source { get; set; }

        internal override void Load( XElement element )
        {
            Unload();
            this.Enabled = true;
            this.Source = null;
            this.Visible = true;
            this.Color = Color.White;
            // Argument Checks
            if( element.Attribute( "X" ) == null )
            {
                throw new ArgumentException( "X Missing for Element" );
            }
            if( element.Attribute( "Y" ) == null )
            {
                throw new ArgumentException( "Y Missing for Element" );
            }
            if( element.Attribute( "Scale" ) == null )
            {
                throw new ArgumentException( "Scale Missing for Element" );
            }

            this.Extent = new RectangularExtent();
            float x = float.Parse( element.Attribute( "X" ).Value );
            float y = float.Parse( element.Attribute( "Y" ).Value );
            Texture = this.ContentManager.Load<Texture2D>( element.Value );
            this.Extent.Reset( new Vector2( x, y ), (float)Texture.Width, (float)Texture.Height );
            if( element.Attribute( "Angle" ) != null )
            {
                this.Extent.Angle = MathHelper.ToRadians( float.Parse( element.Attribute( "Angle" ).Value ) );
            }
            if( element.Attribute( "OriginX" ) != null )
            {
                this.OriginX = float.Parse( element.Attribute( "OriginX" ).Value );
            }
            if( element.Attribute( "OriginY" ) != null )
            {
                this.OriginY = float.Parse( element.Attribute( "OriginY" ).Value );
            }
            // Set position again to account for any Origin values
            this.Extent.Anchor = new Vector2( x, y );
            float scale = float.Parse( element.Attribute( "Scale" ).Value );
            this.Scale = new Vector2( scale, scale );
            if( element.Attribute( "Enabled" ) != null )
            {
                this.Enabled = bool.Parse( element.Attribute( "Enabled" ).Value );
            }
            if( element.Attribute( "Visible" ) != null )
            {
                this.Visible = bool.Parse( element.Attribute( "Visible" ).Value );
            }
            if( element.Attribute( "R" ) != null &&
                element.Attribute( "G" ) != null &&
                element.Attribute( "B" ) != null )
            {
                int r = int.Parse( element.Attribute( "R" ).Value );
                int g = int.Parse( element.Attribute( "G" ).Value );
                int b = int.Parse( element.Attribute( "B" ).Value );
                this.Color = new Color( r, g, b );
            }
        }

        internal override void Unload()
        {
        }

        internal override void Render( ResizedSpriteBatch sb )
        {
            if( Visible )
            {
                sb.Draw( Texture, Extent.Anchor, this.Source, this.Color, Extent.Angle, Extent.Origin, Extent.Scale, SpriteEffects.None, 0.0f );
            }
            base.Render( sb );
        }

        internal override void Render( ResizedSpriteBatch sb, Matrix transform )
        {
            if( Visible )
            {
                sb.Draw( Texture, Extent.Anchor, this.Source, this.Color, Extent.Angle, Extent.Origin, Extent.Scale, SpriteEffects.None, 0.0f, transform, RenderParams.Default );
            }
            base.Render( sb );
        }
    }
}
