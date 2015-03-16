using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xen2D;

namespace DuckyEngine
{
    public class DuckyParticle : DuckyElement
    {
        public List<Vector2> Positions = new List<Vector2>();
        public List<Vector2> Velocities = new List<Vector2>();
        public List<Vector2> Accelerations = new List<Vector2>();
        public List<float> Angles = new List<float>();
        public List<Color> Colors = new List<Color>();
        public int Count { get { return Positions.Count; } }
        public Texture2D Texture { get; set; }

        public void Add( Vector2 position, Vector2 velocity, Color color )
        {
            Positions.Add( position );
            Velocities.Add( velocity );
            Accelerations.Add( Vector2.Zero );
            Angles.Add( 0.0f );
            Colors.Add( color );
        }

        public void Add( Vector2 position, Vector2 velocity, float angle, Color color )
        {
            Positions.Add( position );
            Velocities.Add( velocity );
            Accelerations.Add( Vector2.Zero );
            Angles.Add( angle );
            Colors.Add( color );
        }

        public void Add( Vector2 position, Vector2 velocity, Vector2 acceleration, float angle, Color color )
        {
            Positions.Add( position );
            Velocities.Add( velocity );
            Accelerations.Add( acceleration );
            Angles.Add( angle );
            Colors.Add( color );
        }

        public void RemoveAt( int index )
        {
            Positions.RemoveAt( index );
            Velocities.RemoveAt( index );
            Accelerations.RemoveAt( index );
            Angles.RemoveAt( index );
            Colors.RemoveAt( index );
        }

        public void Clear()
        {
            Positions.Clear();
            Velocities.Clear();
            Accelerations.Clear();
            Angles.Clear();
            Colors.Clear();
        }

        internal override void Load( XElement element )
        {
            Unload();
            this.Enabled = true;
            this.Visible = true;
            this.Color = Color.White;

            this.Extent = new RectangularExtent();
            Texture = this.ContentManager.Load<Texture2D>( element.Value );
            this.Extent.Reset( Vector2.Zero, (float)Texture.Width, (float)Texture.Height );
            if( element.Attribute( "Angle" ) != null )
            {
                this.Extent.Angle = MathHelper.ToRadians( float.Parse( element.Attribute( "Angle" ).Value ) );
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
            Positions.Clear();
            Velocities.Clear();
            Accelerations.Clear();
            Angles.Clear();
            Colors.Clear();
        }

        internal override void Update( float timeDiff )
        {
            for( int i = 0; i < Count; i++ )
            {
                Velocities[ i ] = Velocities[ i ] + Accelerations[ i ] * timeDiff;
                Positions[ i ] = Positions[ i ] + Velocities[ i ] * timeDiff;
            }

            base.Update( timeDiff );
        }

        internal override void Render( ResizedSpriteBatch sb )
        {
            if( Visible )
            {
                for( int i = 0; i < Count; i++ )
                {
                    sb.Draw( Texture, Positions[ i ], null, Colors[ i ], Extent.Angle + Angles[ i ], Extent.Origin, Extent.Scale, SpriteEffects.None, 0.0f );
                }
            }
            base.Render( sb );
        }

        internal override void Render( ResizedSpriteBatch sb, Matrix transform )
        {
            if( Visible )
            {
                for( int i = 0; i < Count; i++ )
                {
                    sb.Draw( Texture, Positions[ i ], null, Colors[ i ], Extent.Angle + Angles[ i ], Extent.Origin, Extent.Scale, SpriteEffects.None, 0.0f, transform, RenderParams.Default );
                }
            }
            base.Render( sb );
        }
    }
}
