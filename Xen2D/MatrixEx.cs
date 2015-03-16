using System;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    public static class MatrixEx
    {
        public static bool Decompose(
            this Matrix matrix,
            out Vector3 scale,
            out Quaternion rotation,
            out Vector3 translation
            )
        {
            translation = new Vector3( matrix.M14, matrix.M24, matrix.M34 );

            Vector3 row1 = new Vector3(matrix.M11, matrix.M12, matrix.M13);
            Vector3 row2 = new Vector3(matrix.M21, matrix.M22, matrix.M23);
            Vector3 row3 = new Vector3(matrix.M31, matrix.M32, matrix.M33);
            scale = new Vector3(
                (float)Math.Sqrt( Vector3.Dot( row1, row1 ) ),
                (float)Math.Sqrt( Vector3.Dot( row2, row2 ) ),
                (float)Math.Sqrt( Vector3.Dot( row3, row3 ) )
                );

            // TODO: Silversprite doesn't have CreateFromRotationMatrix() implemented
            rotation = Quaternion.Identity;
            //Matrix rotationMatrix = new Matrix(
            //    matrix.M11 / scale.X, matrix.M12 / scale.X, matrix.M13 / scale.X, 0,
            //    matrix.M21 / scale.Y, matrix.M22 / scale.Y, matrix.M23 / scale.Y, 0,
            //    matrix.M31 / scale.Z, matrix.M32 / scale.Z, matrix.M33 / scale.Z, 0,
            //    0,                    0,                    0,                    1
            //    );
            //rotation = Quaternion.CreateFromRotationMatrix( rotationMatrix );
            return true;
        }
    }
}
