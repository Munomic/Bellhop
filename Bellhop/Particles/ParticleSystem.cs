using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Xen2D;
using Microsoft.Xna.Framework.Graphics;
using DuckyEngine;

namespace Jubble
{
    public abstract class ParticleSystem
    {
        // Constraint Variables
        protected float minInitialSpeed = 0;
        protected float maxInitialSpeed = 0;
        protected float minAcceleration = 0;
        protected float maxAcceleration = 0;
        protected float minLifetime = 1.0f;
        protected float maxLifetime = 1.0f;
        protected float minScale = 1.0f;
        protected float maxScale = 1.0f;
        protected float minRotationSpeed = 0;
        protected float maxRotationSpeed = 0;
        protected int minSpawnCount = 1;
        protected int maxSpawnCount = 1;
        protected ParticleManager particleManager = null;

        protected Texture2D texture = null;
        protected Vector2 origin;

        #region Constraint Properties

        public float MinimumInitialSpeed
        {
            get { return minInitialSpeed; }
            set { minInitialSpeed = value; }
        }

        public float MaximumInitialSpeed
        {
            get { return maxInitialSpeed; }
            set { maxInitialSpeed = value; }
        }

        public float MinimumAcceleration
        {
            get { return minAcceleration; }
            set { minAcceleration = value; }
        }

        public float MaximumAcceleration
        {
            get { return maxAcceleration; }
            set { maxAcceleration = value; }
        }

        public float MinimumLifetime
        {
            get { return minLifetime; }
            set { minLifetime = value; }
        }

        public float MaximumLifetime
        {
            get { return maxLifetime; }
            set { maxLifetime = value; }
        }

        public float MinimumScale
        {
            get { return minScale; }
            set { minScale = value; }
        }

        public float MaximumScale
        {
            get { return maxScale; }
            set { maxScale = value; }
        }

        public float MinimumRotationSpeed
        {
            get { return minRotationSpeed; }
            set { minRotationSpeed = value; }
        }

        public float MaximumRotationSpeed
        {
            get { return maxRotationSpeed; }
            set { maxRotationSpeed = value; }
        }

        public int MinimumSpawnCount
        {
            get { return minSpawnCount; }
            set { minSpawnCount = value; }
        }

        public int MaximumSpawnCount
        {
            get { return maxSpawnCount; }
            set { maxSpawnCount = value; }
        }

        #endregion

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public Color Color
        {
            get { return particleManager.Color; }
            set { particleManager.Color = value; }
        }

        public ParticleSystem()
        {
            particleManager = new ParticleManager();
        }

        public virtual void Initialize( Texture2D texture )
        {
            Initialize( texture, new Vector2( texture.Width / 2.0f, texture.Height / 2.0f ) );
        }

        public virtual void Initialize( Texture2D texture, Vector2 origin )
        {
            this.Texture = texture;
            this.Origin = origin;
            Initialize();
        }

        public virtual void Initialize()
        { }

        public virtual void Reset()
        {
            particleManager.Reset();
        }

        public virtual void Update( float timeDiff )
        {
            particleManager.Update( timeDiff );
        }

        protected virtual void DirectionGenerator( int particleNumber, int particleCount, out Vector2 initialDirection, out Vector2 accelerationDirection )
        {
            Vector2 randomDirection = XenMath.GetRandomDirectionBetween( 0.0f, MathHelper.TwoPi );
            initialDirection = randomDirection;
            accelerationDirection = randomDirection;
        }

        public virtual void SpawnParticles( Vector2 position )
        {
            int numParticles = XenMath.GetRandomIntBetween( minSpawnCount, maxSpawnCount );
            Vector2 initDirection, accelDirection;

            for( int i = 0; i < numParticles; i++ )
            {
                DirectionGenerator( i, numParticles, out initDirection, out accelDirection );
                particleManager.AddParticle(
                    position,
                    initDirection * XenMath.GetRandomFloatBetween( minInitialSpeed, maxInitialSpeed ),
                    accelDirection * XenMath.GetRandomFloatBetween( minAcceleration, maxAcceleration ),
                    XenMath.GetRandomFloatBetween( minLifetime, maxLifetime ),
                    XenMath.GetRandomFloatBetween( minScale, maxScale ),
                    XenMath.GetRandomFloatBetween( minRotationSpeed, maxRotationSpeed )
                    );
            }
        }
		
#if ANDROID
		public virtual void Draw( ResizedSpriteBatch spriteBatch )
#else
		public virtual void Draw( SpriteBatch spriteBatch )
#endif
        {
            if( texture != null )
            {
                particleManager.Draw( spriteBatch, texture, origin );
            }
        }
		
#if ANDROID
		public virtual void Draw( ResizedSpriteBatch spriteBatch, Matrix transformFromWorldToCamera )
#else
		public virtual void Draw( SpriteBatch spriteBatch, Matrix transformFromWorldToCamera )
#endif
        {
            if( texture != null )
            {
                particleManager.Draw( spriteBatch, texture, origin, transformFromWorldToCamera );
            }
        }
    }
}
