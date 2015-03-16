using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XenAspects;

namespace Xen2D
{
    /// <summary>
    /// This class is responsible for holding the state of various attributes that affect how a sprite will be displayed.  
    /// These include: modulation color, layer depth, and sprite effects.
    /// </summary>
    public sealed class SpriteDisplayAttributeDefaults : I2DDisplayModifiers
    {
        private static Color DefaultModulationColor = Color.White;
        private static Color DefaultModulationColorWithOpacity = Color.White;
        private static float DefaultLayerDepth = 0.0f;
        private static float DefaultOpacity = 1.0f;
        private static float DefaultOpacityModifier = 1.0f;
        private static SpriteEffects DefaultSpriteEffects = SpriteEffects.None;

        public static void SetDefaults( I2DDisplayModifiers displayAttributes )
        {
            if( null != displayAttributes )
            {
                displayAttributes.ModulationColor = DefaultModulationColor;
                //displayAttributes.ModulationColorWithOpacity = DefaultModulationColorWithOpacity;
                displayAttributes.LayerDepth = DefaultLayerDepth;
                displayAttributes.Opacity = DefaultOpacity;
                displayAttributes.OpacityModifier = DefaultOpacityModifier;
                displayAttributes.SpriteEffects = DefaultSpriteEffects;
            }
        }

        private static SpriteDisplayAttributeDefaults _default = new SpriteDisplayAttributeDefaults();
        public static SpriteDisplayAttributeDefaults Default { get { return _default; } }

        public float LayerDepth { get; set; }
        public Color ModulationColor { get; set; }
        public Color ModulationColorWithOpacity { get; private set; }
        public float Opacity { get; set; }
        public float OpacityModifier { get; set; }
        public float OpacityFinal { get; private set; }
        public SpriteEffects SpriteEffects { get; set; }

        private SpriteDisplayAttributeDefaults()
        {
            SetDefaults( this );
            OpacityFinal = Opacity * OpacityModifier;
#if SILVERLIGHT
            ModulationColorWithOpacity = new Color(ModulationColor, OpacityFinal);
#else
            ModulationColorWithOpacity = ModulationColor * OpacityFinal;
#endif
        }

        public static bool HasDefaultDisplayAttributes( I2DDisplayModifiers displayAttributes )
        {
            return ( ( DefaultModulationColor == displayAttributes.ModulationColor ) &&
                     ( DefaultModulationColorWithOpacity == displayAttributes.ModulationColorWithOpacity ) &&
                     ( DefaultLayerDepth == displayAttributes.LayerDepth ) &&
                     ( DefaultOpacity == displayAttributes.Opacity ) &&
                     ( DefaultOpacityModifier == displayAttributes.OpacityModifier ) &&
                     ( DefaultSpriteEffects == displayAttributes.SpriteEffects ) );
        }
    }
}
