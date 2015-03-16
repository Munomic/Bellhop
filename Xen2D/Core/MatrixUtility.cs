using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    public static class MatrixUtility
    {
        public static Matrix Zero
        {
            get
            {
                return new Matrix( 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
            }
        }

        /// <summary>
        /// Calculates the matrix to from the extent's space to the absolute space.
        /// </summary>
        /// <param name="extent">The extent to transform from.</param>
        /// <returns>The transform matrix.</returns>
        public static Matrix GetTransformFromExtentToAbsolute( IExtentTransform extent )
        {
            return CreateTransform( extent.Scale, extent.Angle, extent.Anchor, extent.Origin );
        }

        public static Matrix CreateTransform( Vector2 scale, float angle, Vector2 anchor )
        {
            return CreateTransform( scale, angle, anchor, Vector2.Zero );
        }

        public static Matrix CreateTransform( Vector2 scale, float angle, Vector2 anchor, Vector2 origin )
        {
            return
                Matrix.CreateTranslation( -origin.X, -origin.Y, 0 ) *
                Matrix.CreateScale( scale.X, scale.Y, 1 ) *
                Matrix.CreateRotationZ( angle ) *
                Matrix.CreateTranslation( anchor.X, anchor.Y, 0 );
        }
    }
}
