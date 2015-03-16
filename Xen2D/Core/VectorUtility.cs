using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    public static class VectorUtility
    {
        public static Vector2 Zero
        {
            get
            {
                return new Vector2( 0, 0 );
            }
        }

        public static Vector2 TransformAndRound( Vector2 vectorToTransform, Matrix transformMatrix )
        {
            return XenMath.RoundVector( Vector2.Transform( vectorToTransform, transformMatrix ) );
        }
    }
}
