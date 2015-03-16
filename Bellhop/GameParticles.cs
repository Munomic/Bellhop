using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Xen2D;
using Microsoft.Xna.Framework;

namespace Jubble
{
    public static class GameParticles
    {
        public static SplashParticleSystem Splash;
        public static FireParticleSystem Flame;
        public static FireCenterParticleSystem FlameYellow;
        public static SmokePlumeParticleSystem Smoke;

        public static void Load()
        {
            Splash = new SplashParticleSystem();
            Splash.Initialize( Globals.Content.Load<Texture2D>( "Textures/particle" ) );
            Splash.Color = new Color( 19, 147, 255 );
            Smoke = new SmokePlumeParticleSystem();
            Smoke.Initialize( Globals.Content.Load<Texture2D>( "Textures/particle" ) );
            Smoke.Color = Color.Gray;
            Flame = new FireParticleSystem();
            Flame.Initialize( Globals.Content.Load<Texture2D>( "Textures/particle" ) );
            Flame.Color = Color.Red;
            FlameYellow = new FireCenterParticleSystem();
            FlameYellow.Initialize( Globals.Content.Load<Texture2D>( "Textures/particle" ) );
            FlameYellow.Color = Color.Yellow;
        }
    }
}
