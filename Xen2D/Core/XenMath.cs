using System;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    public static class XenMath
    {
        private static Random random = new Random();
        public static Random Random { get { return random; } }

        /// <summary>
        /// Sets a manual random number seed
        /// </summary>
        /// <param name="seed">Seed for the Random Number Generator</param>
        public static void SetSeed( int seed )
        {
            random = new Random( seed );
        }

        /// <summary>
        /// Returns a value between 0.0 and 1.0
        /// </summary>
        /// <returns>The randomly generated value</returns>
        public static float GetRandomFloat()
        {
            return (float)random.NextDouble();
        }

        /// <summary>
        /// Returns a value inclusive between [min, max]
        /// </summary>
        /// <param name="min">Minimum random value</param>
        /// <param name="max">Maximum random value</param>
        /// <returns>The randomly generated value</returns>
        public static float GetRandomFloatBetween( float min, float max )
        {
            return min + (float)random.NextDouble() * ( max - min );
        }

        /// <summary>
        /// Returns a value inclusive between [min, max]
        /// </summary>
        /// <param name="min">Minimum random value</param>
        /// <param name="max">Maximum random value</param>
        /// <returns>The randomly generated value</returns>
        public static int GetRandomIntBetween( int min, int max )
        {
            return min + random.Next( max - min + 1 );
        }

        /// <summary>
        /// Returns a normalized 2D vector between two angles
        /// </summary>
        /// <param name="minAngle">Minimum angle value</param>
        /// <param name="maxAngle">Maximum angle value</param>
        /// <returns>The randomly generated value</returns>
        public static Vector2 GetRandomDirectionBetween( float minAngle, float maxAngle )
        {
            return CreateNormalVectorFromAngleFloat( GetRandomFloatBetween( minAngle, maxAngle ) );
        }

        public static float OneDegreeInRadians
        {
            get { return MathHelper.Pi / 180; }
        }

        /// <summary>
        /// Creates a normalized vector at the specified angle.  
        /// </summary>
        /// <param name="angle">The angle of the vector in radians.</param>
        /// <returns>The normalized vector at the specified angle.</returns>
        public static Vector2 CreateNormalVectorFromAngleDouble( double angle )
        {
            Vector2 vector = new Vector2( (float)Math.Cos( angle ), (float)Math.Sin( angle ) );
            vector.Normalize();
            return vector;
        }

        /// <summary>
        /// Creates a normalized vector at the specified angle.  
        /// </summary>
        /// <param name="angle">The angle of the vector in radians.</param>
        /// <returns>The normalized vector at the specified angle.</returns>
        public static Vector2 CreateNormalVectorFromAngleFloat( float angle )
        {
            Vector2 vector = new Vector2( (float)Math.Cos( angle ), (float)Math.Sin( angle ) );
            vector.Normalize();
            return vector;
        }

        /// <summary>
        /// Extends the specified vector by a specified scale factor.
        /// </summary>
        /// <param name="direction">The vector to extend.</param>
        /// <param name="value">The amount to scale the direction vector.</param>
        /// <returns>The scaled vector</returns>
        public static Vector2 Extend( Vector2 direction, float value )
        {
            if( Vector2.Zero == direction )
            {
                throw new NotSupportedException( "Cannot extend the zero vector" );
            }
            float angle = GetAngleFloat( direction );
            direction.X += value * (float)Math.Cos( angle );
            direction.Y += value * (float)Math.Sin( angle );
            return direction;
        }

        public static Vector2 Extend( Vector2 start, Vector2 stop, float value )
        {
            return start + Extend( stop - start, value );
        }

        public static double GetAngleDouble( Vector2 vector )
        {
            if( vector == Vector2.Zero )
            {
                return 0;
            }

            double angle = Math.Atan2( vector.Y, vector.X );

            return NormalizeAngleRadians( angle );
        }

        public static float GetAngleFloat( Vector2 vector )
        {
            return (float)GetAngleDouble( vector );
        }

        /// <summary>
        /// Calculates the angle difference between the two specified angles.
        /// </summary>
        /// <param name="minuend">The value to subtract from.  Ex. "a" in (a-b)</param>
        /// <param name="subtrahend">The value to subtract.  Ex. "b" in (a-b)</param>
        /// <returns>The angle representing the difference between the two specified vectors.</returns>
        public static double GetDifferenceAngleDouble( Vector2 minuend, Vector2 subtrahend )
        {
            return GetAngleDouble( minuend ) - GetAngleDouble( subtrahend );
        }

        /// <summary>
        /// Calculates the angle difference between the two specified angles.
        /// </summary>
        /// <param name="minuend">The value to subtract from.  Ex. "a" in (a-b)</param>
        /// <param name="subtrahend">The value to subtract.  Ex. "b" in (a-b)</param>
        /// <returns>The angle representing the difference between the two specified vectors.</returns>
        public static float GetDifferenceAngleFloat( Vector2 minuend, Vector2 subtrahend )
        {
            return (float)GetDifferenceAngleDouble( minuend, subtrahend );
        }

        /// <summary>
        /// Calculates the angle difference between the two specified angles.
        /// </summary>
        /// <param name="minuend">The value to subtract from.  Ex. "a" in (a-b)</param>
        /// <param name="subtrahend">The value to subtract.  Ex. "b" in (a-b)</param>
        /// <returns>The vector representing the angle between the difference between the two specified vectors.</returns>
        public static Vector2 GetDifferenceAngleVector( Vector2 minuend, Vector2 subtrahend )
        {
            //float angle 
            double differenceAngle = GetDifferenceAngleDouble( minuend, subtrahend );
            return CreateNormalVectorFromAngleDouble( differenceAngle );
        }

        public static bool IsWithinToleranceToTarget( float f, float target, float tolerance )
        {
            f = f - target;
            if( f < 0 )
            {
                f = f * -1;
            }
            if( f < tolerance )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Normalizes the specific angle, converting it to within the range of 0 and 2*pi.  
        /// </summary>
        /// <param name="angle">The angle to normalized.</param>
        /// <returns>The normalized angle.</returns>
        public static double NormalizeAngleRadians( double angle )
        {
            if( angle > MathHelper.TwoPi )
            {
                int factor = (int)( angle / MathHelper.TwoPi );
                return angle - ( factor * MathHelper.TwoPi );
            }
            else if( angle < 0 )
            {
                int factor = (int)( angle / MathHelper.TwoPi );
                return angle - ( ( factor ) * MathHelper.TwoPi ) + MathHelper.TwoPi;
            }
            else
            {
                return angle;
            }
        }

        /// <summary>
        /// Normalizes the specific angle, converting it to within the range of 0 and 2-pi.  
        /// </summary>
        /// <param name="angle">The angle to normalized.</param>
        /// <returns>The normalized angle.</returns>
        public static float NormalizeAngleRadians( float angle )
        {
            if( angle > MathHelper.TwoPi )
            {
                int factor = (int)( angle / MathHelper.TwoPi );
                return angle - (float)( factor * MathHelper.TwoPi );
            }
            else if( angle < 0 )
            {
                int factor = (int)( angle / MathHelper.TwoPi );
                return angle - (float)( ( factor ) * MathHelper.TwoPi ) + MathHelper.TwoPi;
            }
            else
            {
                return angle;
            }
        }

        public static Vector2 Rotate( Vector2 vectorToRotate, Vector2 vectorToRotateBy )
        {
            return Rotate( vectorToRotate, GetAngleFloat( vectorToRotateBy ) );
        }

        public static Vector2 Rotate( Vector2 vectorToRotate, float radiansToRotateBy )
        {
            if( 0 == radiansToRotateBy )
            {
                return vectorToRotate;
            }

            var rotationMatrix = Matrix.CreateRotationZ( radiansToRotateBy );
            var rotatedVector = Vector2.Transform( vectorToRotate, rotationMatrix );
            return RoundVector( rotatedVector );
        }

        public static float RoundFloatToFloat( float numberToRound )
        {
            return (float)RoundDoubleToDouble( (double)numberToRound );
        }

        public static int RoundFloatToInt( float f )
        {
            return (int)( f < 0.0f ? f - 0.5f : f + 0.5f );
        }

        public static float RoundDoubleToFloat( double numberToRound )
        {
            return (float)RoundDoubleToDouble( numberToRound );
        }

        public static double RoundDoubleToDouble( double numberToRound )
        {
            // TODO: RoundDoubleToFloat() isn't so good... float can do much better than 4-decimal digits
            return Math.Round( numberToRound, 4 );
        }

        public static Vector2 RoundVector( Vector2 vectorToRound )
        {
            return new Vector2( RoundFloatToFloat( vectorToRound.X ), RoundFloatToFloat( vectorToRound.Y ) );
        }

        public static float ShortestRotationAngleTo( float desiredFacing, float currentFacing )
        {
            if( desiredFacing == currentFacing )
            {
                return 0;
            }

            desiredFacing = NormalizeAngleRadians( desiredFacing );
            currentFacing = NormalizeAngleRadians( currentFacing );

            float differenceAngle = desiredFacing - currentFacing;
            float differenceSign = ( differenceAngle >= 0 ) ? 1 : -1;

            if( Math.Abs( differenceAngle ) <= MathHelper.Pi )
                return RoundFloatToFloat( differenceAngle );
            else
                return RoundFloatToFloat( differenceAngle - ( differenceSign * MathHelper.TwoPi ) );
        }
    }
}