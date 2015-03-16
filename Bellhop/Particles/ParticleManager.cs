using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XenAspects;
using Xen2D;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using DuckyEngine;

namespace Jubble
{
    public class Particle : PooledObject<Particle>
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        private float lifetime;
        public float Lifetime
        {
            get { return lifetime; }
            set { lifetime = value; }
        }

        private float timeSinceStart;
        public float TimeSinceStart
        {
            get { return timeSinceStart; }
            set { timeSinceStart = value; }
        }

        private float scale;
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        private float rotation;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        private float rotationSpeed;
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

        public bool Active
        {
            get { return TimeSinceStart < Lifetime; }
        }
        
        public void Update( float dt )
        {
            Velocity += Acceleration * dt;
            Position += Velocity * dt;

            Rotation += rotationSpeed * dt;

            TimeSinceStart += dt;
        }

        public void Update( float dt, Vector2 offset)
        {
            Update( dt );
            Position += offset;
        }
    }

    public class ParticleManager
    {
        NList<Particle> activeParticleList;
        public NList<Particle> ParticleList { get { return activeParticleList; } }

        Color color = Color.White;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        BlendState blendState = BlendState.Additive; // Additive by default
        public BlendState BlendMode
        {
            get { return blendState; }
            set { blendState = value; }
        }

        public ParticleManager()
        {
            activeParticleList = new NList<Particle>();
        }

        #region Public Methods

        public void Reset()
        {
            activeParticleList.BeginEnumeration();
            foreach( Particle p in activeParticleList.Items )
            {
                activeParticleList.Remove( p );
                p.Release();
            }
            activeParticleList.EndEnumeration();
        }

        public void AddParticle( Vector2 position, Vector2 velocity, Vector2 acceleration,
            float lifetime, float scale, float rotationSpeed )
        {
            Particle particle = Particle.Acquire(); // Get one from the pool
            particle.Position = position;
            particle.Velocity = velocity;
            particle.Acceleration = acceleration;
            particle.Lifetime = lifetime;
            particle.Scale = scale;
            particle.RotationSpeed = rotationSpeed;

            particle.TimeSinceStart = 0.0f;
            particle.Rotation = XenMath.GetRandomFloatBetween( 0, MathHelper.TwoPi );
            activeParticleList.Add( particle );
        }

        public void Update( float dt )
        {
            activeParticleList.BeginEnumeration();
            foreach( Particle p in activeParticleList.Items )
            {
                p.Update( dt );
                if( !p.Active )
                {
                    activeParticleList.Remove( p );
                    p.Release();
                }
            }
            activeParticleList.EndEnumeration();
        }

        public void Update( float dt, Vector2 offset )
        {
            activeParticleList.BeginEnumeration();
            foreach( Particle p in activeParticleList.Items )
            {
                p.Update( dt, offset );
                if( !p.Active )
                {
                    activeParticleList.Remove( p );
                    p.Release();
                }
            }
            activeParticleList.EndEnumeration();
        }
        
#if ANDROID
        public void Draw( ResizedSpriteBatch spriteBatch, Texture2D texture, Vector2 origin )
#else
        public void Draw( SpriteBatch spriteBatch, Texture2D texture, Vector2 origin )
#endif
        {
            spriteBatch.Begin( SpriteSortMode.Deferred, (blendState == BlendState.Opaque ? BlendState.AlphaBlend : blendState ) );

            foreach( Particle p in activeParticleList.Items )
            {
                float normalizedLifetime = p.TimeSinceStart / p.Lifetime;
                float alpha = ( blendState == BlendState.Opaque ? 1.0f : 4 * normalizedLifetime * ( 1 - normalizedLifetime ) );
                float scale = p.Scale * ( .75f + .25f * normalizedLifetime );

                spriteBatch.Draw( texture, p.Position, null, color * alpha,
                    p.Rotation, origin, scale, SpriteEffects.None, 0.0f );
            }

            spriteBatch.End();
        }
        
#if ANDROID
        public void Draw( ResizedSpriteBatch spriteBatch, Texture2D texture, Vector2 origin, Matrix transformFromWorldToCamera )
#else
        public void Draw( SpriteBatch spriteBatch, Texture2D texture, Vector2 origin, Matrix transformFromWorldToCamera )
#endif
        {
            spriteBatch.Begin( SpriteSortMode.Deferred, ( blendState == BlendState.Opaque ? BlendState.AlphaBlend : blendState ) );

            foreach( Particle p in activeParticleList.Items )
            {
                float normalizedLifetime = p.TimeSinceStart / p.Lifetime;
                float alpha = ( blendState == BlendState.Opaque ? 1.0f : 4 * normalizedLifetime * ( 1 - normalizedLifetime ) );
                float scale = p.Scale * ( .75f + .25f * normalizedLifetime );

                spriteBatch.Draw( texture, p.Position, null, color * alpha,
                    p.Rotation, origin, scale, SpriteEffects.None, 0.0f, transformFromWorldToCamera, RenderParams.Default );
            }

            spriteBatch.End();
        }

        #endregion
    }
}
