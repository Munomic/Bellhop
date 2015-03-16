using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xen2D;

namespace DuckyEngine
{
    public class DuckyText : DuckyElement
    {
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                Vector2 size = DuckyScreen.Fonts[ _font ].MeasureString( _text );
                Vector2 pos = this.Extent.Anchor;
                float angle = this.Extent.Angle;
                Vector2 origin = this.Extent.Origin;
                Vector2 scale = this.Extent.Scale;
                this.Extent.Reset( pos, size.X, size.Y );
                this.Extent.Angle = angle;
                this.Extent.Origin = origin;
                this.Scale = scale;
            }
        }
        string _text = "";
        string _font = "";

        internal override void Load( XElement element )
        {
            Unload();
            this.Enabled = true;
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
            if( element.Attribute( "Font" ) == null )
            {
                throw new ArgumentException( "Font Missing for Element" );
            }
            if( element.Attribute( "Scale" ) == null )
            {
                throw new ArgumentException( "Scale Missing for Element" );
            }

            this.Extent = new RectangularExtent();
            float x = float.Parse( element.Attribute( "X" ).Value );
            float y = float.Parse( element.Attribute( "Y" ).Value );
            _font = element.Attribute( "Font" ).Value;
            if( !DuckyScreen.Fonts.ContainsKey( _font ) )
            {
                DuckyScreen.Fonts.Add( _font, Globals.Content.Load<SpriteFont>( _font ) );
            }
            _text = element.Value;
            Vector2 size = DuckyScreen.Fonts[ _font ].MeasureString( _text );
            this.Extent.Reset( new Vector2( x, y ), (float)size.X, (float)size.Y );
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
                sb.DrawString( DuckyScreen.Fonts[ _font ], _text, Extent.Anchor, this.Color, Extent.Angle, Extent.Origin, Extent.Scale, SpriteEffects.None, 0.0f );
            }
            base.Render( sb );
        }

        internal override void Render( ResizedSpriteBatch sb, Matrix transform )
        {
            if( Visible )
            {
                sb.DrawString( DuckyScreen.Fonts[ _font ], _text, Extent.Anchor, this.Color, Extent.Angle, Extent.Origin, Extent.Scale, SpriteEffects.None, 0.0f, transform, RenderParams.Default );
            }
            base.Render( sb );
        }
    }
}
