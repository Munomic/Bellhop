using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xen2D;

namespace Jubble
{
    public class FireCenterParticleSystem : ParticleSystem
    {
        public override void Initialize()
        {
            // Set the default constraint values
            minInitialSpeed = 30;
            maxInitialSpeed = 50;
            minAcceleration = 0;
            maxAcceleration = 0;
            minLifetime = 0.1f;
            maxLifetime = 0.4f;
            minScale = 0.25f;
            maxScale = 0.35f;
            minSpawnCount = 0;
            maxSpawnCount = 1;
            minRotationSpeed = 0;
            maxRotationSpeed = 0;
            particleManager.BlendMode = BlendState.Additive;

            base.Initialize();
        }

        public void Initialize(
            float minSpd, float maxSpd,
            float minAcl, float maxAcl,
            float minLife, float maxLife,
            float minScl, float maxScl,
            float minRotSpd, float maxRotSpd,
            int minCount, int maxCount )
        {
            minInitialSpeed = minSpd; maxInitialSpeed = maxSpd;
            minAcceleration = minAcl; maxAcceleration = maxAcl;
            minLifetime = minLife; maxLifetime = maxLife;
            minScale = minScl; maxScale = maxScl;
            minRotationSpeed = minRotSpd; maxRotationSpeed = maxRotSpd;
            minSpawnCount = minCount; maxSpawnCount = maxCount;

            particleManager.BlendMode = BlendState.Additive;

            base.Initialize();
        }

        protected override void DirectionGenerator( int particleNumber, int particleCount, out Vector2 initialDirection, out Vector2 accelerationDirection )
        {
            Vector2 randomDirection = XenMath.GetRandomDirectionBetween( MathHelper.ToRadians( -80 ), MathHelper.ToRadians( -100 ) );
            initialDirection = randomDirection;
            accelerationDirection = Vector2.Zero;
        }
    }

    public class FireParticleSystem : ParticleSystem
    {
        public override void Initialize()
        {
            // Set the default constraint values
            minInitialSpeed = 60;
            maxInitialSpeed = 100;
            minAcceleration = 0;
            maxAcceleration = 0;
            minLifetime = 0.1f;
            maxLifetime = 0.4f;
            minScale = 0.35f;
            maxScale = 0.6f;
            minSpawnCount = 1;
            maxSpawnCount = 1;
            minRotationSpeed = 0;
            maxRotationSpeed = 0;
            particleManager.BlendMode = BlendState.Additive;

            base.Initialize();
        }

        public void Initialize(
            float minSpd, float maxSpd,
            float minAcl, float maxAcl,
            float minLife, float maxLife,
            float minScl, float maxScl,
            float minRotSpd, float maxRotSpd,
            int minCount, int maxCount )
        {
            minInitialSpeed = minSpd; maxInitialSpeed = maxSpd;
            minAcceleration = minAcl; maxAcceleration = maxAcl;
            minLifetime = minLife; maxLifetime = maxLife;
            minScale = minScl; maxScale = maxScl;
            minRotationSpeed = minRotSpd; maxRotationSpeed = maxRotSpd;
            minSpawnCount = minCount; maxSpawnCount = maxCount;

            particleManager.BlendMode = BlendState.Additive;

            base.Initialize();
        }

        protected override void DirectionGenerator( int particleNumber, int particleCount, out Vector2 initialDirection, out Vector2 accelerationDirection )
        {
            Vector2 randomDirection = XenMath.GetRandomDirectionBetween( MathHelper.ToRadians( -80 ), MathHelper.ToRadians( -100 ) );
            initialDirection = randomDirection;
            accelerationDirection = Vector2.Zero;
        }
    }

    public class SplashParticleSystem : ParticleSystem
    {
        public override void Initialize()
        {
            // Set the default constraint values
            minInitialSpeed = 100;
            maxInitialSpeed = 150;
            minAcceleration = 300;
            maxAcceleration = 400;
            minLifetime = 0.3f;
            maxLifetime = 0.5f;
            minScale = 0.15f;
            maxScale = 0.25f;
            minSpawnCount = 5;
            maxSpawnCount = 10;
            minRotationSpeed = -MathHelper.PiOver4 / 2.0f;
            maxRotationSpeed = MathHelper.PiOver4 / 2.0f;

            particleManager.BlendMode = BlendState.AlphaBlend;

            base.Initialize();
        }

        public void Initialize(
            float minSpd, float maxSpd,
            float minAcl, float maxAcl,
            float minLife, float maxLife,
            float minScl, float maxScl,
            float minRotSpd, float maxRotSpd,
            int minCount, int maxCount )
        {
            minInitialSpeed = minSpd; maxInitialSpeed = maxSpd;
            minAcceleration = minAcl; maxAcceleration = maxAcl;
            minLifetime = minLife; maxLifetime = maxLife;
            minScale = minScl; maxScale = maxScl;
            minRotationSpeed = minRotSpd; maxRotationSpeed = maxRotSpd;
            minSpawnCount = minCount; maxSpawnCount = maxCount;

            particleManager.BlendMode = BlendState.AlphaBlend;

            base.Initialize();
        }

        protected override void DirectionGenerator( int particleNumber, int particleCount, out Vector2 initialDirection, out Vector2 accelerationDirection )
        {
            Vector2 randomDirection = XenMath.GetRandomDirectionBetween( MathHelper.ToRadians( -70 ), MathHelper.ToRadians( -110 ) );
            initialDirection = randomDirection;
            // simulate a bit of wind to the right
            accelerationDirection = Vector2.UnitY;
        }
    }

    public class BucketPourParticleSystem : ParticleSystem
    {
        public float PourAngle = 0.0f;

        public override void Initialize()
        {
            // Set the default constraint values
            minInitialSpeed = 1000;
            maxInitialSpeed = 1050;
            minAcceleration = 4000;
            maxAcceleration = 4010;
            minLifetime = 0.2f;
            maxLifetime = 0.2f;
            minScale = 0.35f;
            maxScale = 0.45f;
            minSpawnCount = 6;
            maxSpawnCount = 8;
            minRotationSpeed = -MathHelper.PiOver4 / 2.0f;
            maxRotationSpeed = MathHelper.PiOver4 / 2.0f;

            particleManager.BlendMode = BlendState.AlphaBlend;

            base.Initialize();
        }

        public void Initialize(
            float minSpd, float maxSpd,
            float minAcl, float maxAcl,
            float minLife, float maxLife,
            float minScl, float maxScl,
            float minRotSpd, float maxRotSpd,
            int minCount, int maxCount )
        {
            minInitialSpeed = minSpd; maxInitialSpeed = maxSpd;
            minAcceleration = minAcl; maxAcceleration = maxAcl;
            minLifetime = minLife; maxLifetime = maxLife;
            minScale = minScl; maxScale = maxScl;
            minRotationSpeed = minRotSpd; maxRotationSpeed = maxRotSpd;
            minSpawnCount = minCount; maxSpawnCount = maxCount;

            particleManager.BlendMode = BlendState.AlphaBlend;

            base.Initialize();
        }

        protected override void DirectionGenerator( int particleNumber, int particleCount, out Vector2 initialDirection, out Vector2 accelerationDirection )
        {
            Vector2 randomDirection = XenMath.GetRandomDirectionBetween( MathHelper.ToRadians( -88 ) + PourAngle, MathHelper.ToRadians( -92 ) + PourAngle );
            initialDirection = randomDirection;
            // simulate a bit of wind to the right
            accelerationDirection = Vector2.UnitY;
        }
    }

    public class BubbleExplosionParticleSystem : ParticleSystem
    {
        public override void Initialize()
        {
            // Set the default constraint values
            minInitialSpeed = 100 * ( GameSettings.IsPadDevice ? 1.0f : 0.5f );
            maxInitialSpeed = 400 * ( GameSettings.IsPadDevice ? 1.0f : 0.5f );
            minAcceleration = 0;
            maxAcceleration = 0;
            minLifetime = 0.25f;
            maxLifetime = 0.5f;
            minScale = 0.3f * ( GameSettings.IsPadDevice ? 1.0f : 0.5f );
            maxScale = 1.0f * ( GameSettings.IsPadDevice ? 1.0f : 0.5f );
            minSpawnCount = ( GameSettings.IsPadDevice ? 20 : 10 );
            maxSpawnCount = ( GameSettings.IsPadDevice ? 40 : 20 );
            minRotationSpeed = -MathHelper.Pi;
            maxRotationSpeed = MathHelper.Pi;

            particleManager.BlendMode = BlendState.Additive;
            Color = Color.DarkOliveGreen;

            base.Initialize();
        }

        public void Initialize(
            float minSpd, float maxSpd,
            float minAcl, float maxAcl,
            float minLife, float maxLife,
            float minScl, float maxScl,
            float minRotSpd, float maxRotSpd,
            int minCount, int maxCount )
        {
            minInitialSpeed = minSpd; maxInitialSpeed = maxSpd;
            minAcceleration = minAcl; maxAcceleration = maxAcl;
            minLifetime = minLife; maxLifetime = maxLife;
            minScale = minScl; maxScale = maxScl;
            minRotationSpeed = minRotSpd; maxRotationSpeed = maxRotSpd;
            minSpawnCount = minCount; maxSpawnCount = maxCount;

            particleManager.BlendMode = BlendState.Additive;

            base.Initialize();
        }
    }

    public class RockExplosionParticleSystem : ParticleSystem
    {
        public override void Initialize()
        {
            // Set the default constraint values
            minInitialSpeed = 100 * ( GameSettings.IsPadDevice ? 1.0f : 0.5f );
            maxInitialSpeed = 400 * ( GameSettings.IsPadDevice ? 1.0f : 0.5f );
            minAcceleration = 0;
            maxAcceleration = 0;
            minLifetime = 0.5f;
            maxLifetime = 0.75f;
            minScale = 0.3f * ( GameSettings.IsPadDevice ? 1.0f : 0.5f );
            maxScale = 1.0f * ( GameSettings.IsPadDevice ? 1.0f : 0.5f );
            minSpawnCount = ( GameSettings.IsPadDevice ? 10 : 5 );
            maxSpawnCount = ( GameSettings.IsPadDevice ? 20 : 10 );
            minRotationSpeed = -MathHelper.Pi;
            maxRotationSpeed = MathHelper.Pi;

            particleManager.BlendMode = BlendState.AlphaBlend;
            Color = Color.White;

            base.Initialize();
        }

        public void Initialize(
            float minSpd, float maxSpd,
            float minAcl, float maxAcl,
            float minLife, float maxLife,
            float minScl, float maxScl,
            float minRotSpd, float maxRotSpd,
            int minCount, int maxCount )
        {
            minInitialSpeed = minSpd; maxInitialSpeed = maxSpd;
            minAcceleration = minAcl; maxAcceleration = maxAcl;
            minLifetime = minLife; maxLifetime = maxLife;
            minScale = minScl; maxScale = maxScl;
            minRotationSpeed = minRotSpd; maxRotationSpeed = maxRotSpd;
            minSpawnCount = minCount; maxSpawnCount = maxCount;

            particleManager.BlendMode = BlendState.AlphaBlend;

            base.Initialize();
        }
    }

    public class ExplosionParticleSystem : ParticleSystem
    {
        public override void Initialize()
        {
            // Set the default constraint values
            minInitialSpeed = 10;
            maxInitialSpeed = 200;
            minAcceleration = 0;
            maxAcceleration = 0;
            minLifetime = 0.5f;
            maxLifetime = 1.0f;
            minScale = 0.3f;
            maxScale = 1.0f;
            minSpawnCount = ( GameSettings.IsPadDevice ? 10 : 5 );
            maxSpawnCount = ( GameSettings.IsPadDevice ? 12 : 6 );
            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            particleManager.BlendMode = BlendState.Additive;

            base.Initialize();
        }

        public void Initialize(
            float minSpd, float maxSpd,
            float minAcl, float maxAcl,
            float minLife, float maxLife,
            float minScl, float maxScl,
            float minRotSpd, float maxRotSpd,
            int minCount, int maxCount )
        {
            minInitialSpeed = minSpd; maxInitialSpeed = maxSpd;
            minAcceleration = minAcl; maxAcceleration = maxAcl;
            minLifetime = minLife; maxLifetime = maxLife;
            minScale = minScl; maxScale = maxScl;
            minRotationSpeed = minRotSpd; maxRotationSpeed = maxRotSpd;
            minSpawnCount = minCount; maxSpawnCount = maxCount;

            particleManager.BlendMode = BlendState.Additive;

            base.Initialize();
        }
    }

    public class ExplosionSmokeParticleSystem : ParticleSystem
    {
        public override void Initialize()
        {
            // Set the default constraint values
            minInitialSpeed = 50;
            maxInitialSpeed = 75;
            minAcceleration = -10;
            maxAcceleration = -20;
            minLifetime = 1.0f;
            maxLifetime = 2.5f;
            minScale = 0.2f;
            maxScale = 0.4f;
            minSpawnCount = 5;
            maxSpawnCount = 10;
            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            particleManager.BlendMode = BlendState.AlphaBlend;

            base.Initialize();
        }

        public void Initialize(
            float minSpd, float maxSpd,
            float minAcl, float maxAcl,
            float minLife, float maxLife,
            float minScl, float maxScl,
            float minRotSpd, float maxRotSpd,
            int minCount, int maxCount )
        {
            minInitialSpeed = minSpd; maxInitialSpeed = maxSpd;
            minAcceleration = minAcl; maxAcceleration = maxAcl;
            minLifetime = minLife; maxLifetime = maxLife;
            minScale = minScl; maxScale = maxScl;
            minRotationSpeed = minRotSpd; maxRotationSpeed = maxRotSpd;
            minSpawnCount = minCount; maxSpawnCount = maxCount;

            particleManager.BlendMode = BlendState.AlphaBlend;

            base.Initialize();
        }
    }

    public class SmokePlumeParticleSystem : ParticleSystem
    {
        public override void Initialize()
        {
            // Set the default constraint values
            minInitialSpeed = 20;
            maxInitialSpeed = 100;
            minAcceleration = 1;
            maxAcceleration = 5;
            minLifetime = 0.15f;
            maxLifetime = 0.75f;
            minScale = 0.3f;
            maxScale = 0.8f;
            minSpawnCount = 1;
            maxSpawnCount = 2;
            minRotationSpeed = -MathHelper.PiOver4 / 2.0f;
            maxRotationSpeed = MathHelper.PiOver4 / 2.0f;

            particleManager.BlendMode = BlendState.AlphaBlend;

            base.Initialize();
        }

        public void Initialize(
            float minSpd, float maxSpd,
            float minAcl, float maxAcl,
            float minLife, float maxLife,
            float minScl, float maxScl,
            float minRotSpd, float maxRotSpd,
            int minCount, int maxCount )
        {
            minInitialSpeed = minSpd; maxInitialSpeed = maxSpd;
            minAcceleration = minAcl; maxAcceleration = maxAcl;
            minLifetime = minLife; maxLifetime = maxLife;
            minScale = minScl; maxScale = maxScl;
            minRotationSpeed = minRotSpd; maxRotationSpeed = maxRotSpd;
            minSpawnCount = minCount; maxSpawnCount = maxCount;

            particleManager.BlendMode = BlendState.AlphaBlend;

            base.Initialize();
        }

        protected override void DirectionGenerator( int particleNumber, int particleCount, out Vector2 initialDirection, out Vector2 accelerationDirection )
        {
            Vector2 randomDirection = XenMath.GetRandomDirectionBetween( MathHelper.ToRadians( -80 ), MathHelper.ToRadians( -100 ) );
            initialDirection = randomDirection;
            // simulate a bit of wind to the right
            accelerationDirection = Vector2.UnitX;
        }
    }
}
