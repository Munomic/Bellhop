using System;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    /// <summary>
    /// Structure containing the transform matrices to and from a designated space.
    /// </summary>
    public struct Space2DTranslation
    {
        public static Space2DTranslation Create()
        {
            Space2DTranslation instance = new Space2DTranslation();
            instance._translateTo = Matrix.Identity;
            instance._translateFrom = Matrix.Identity;
            return instance;
        }

        public static Space2DTranslation CreateFromTranslateTo( Matrix translateTo )
        {
            Space2DTranslation instance = new Space2DTranslation();
            instance._translateTo = translateTo;
            instance._translateFrom = Matrix.Invert( translateTo );
            return instance;
        }

        public static Space2DTranslation CreateFromTranslateFrom( Matrix translateFrom )
        {
            Space2DTranslation instance = new Space2DTranslation();
            instance._translateTo = Matrix.Invert( translateFrom );
            instance._translateFrom = translateFrom;
            return instance;
        }

        private Matrix _translateTo;
        private Matrix _translateFrom;

        public Matrix TranslateTo
        {
            get { return _translateTo; }
            set 
            { 
                _translateTo = value;
                _translateFrom = Matrix.Invert( _translateTo );
            }
        }

        public Matrix TranslateFrom
        {
            get { return _translateFrom; }
            set
            {
                _translateFrom = value;
                _translateTo = Matrix.Invert( _translateFrom );
            }
        }

        public Vector2 TranslateVectorFromAbsoluteToThisSpace( Vector2 vector )
        {
            return Vector2.Transform( vector, _translateTo );
        }

        public Vector2 TranslateVectorFromThisSpaceToAbsolute( Vector2 vector )
        {
            return Vector2.Transform( vector, _translateFrom );
        }
    }
}
