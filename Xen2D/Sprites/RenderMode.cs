using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XenAspects;

namespace Xen2D
{
    [Flags]
    public enum RenderMode
    {
        None = 0x00,
        Texture = 0x01,
        TraceRenderingExtent = 0x02,
        TraceLogicalExtent = 0x04,
        TraceBoundingBox = 0x08,
        MarkCenter = 0x10,
        MarkTopLeft = 0x20,
        MarkOrigin = 0x40,
        All = 0x7F,
    }

    public struct RenderParams
    {
        private static RenderParams _default = new RenderParams( RenderMode.Texture, null, null, null );
        public static RenderParams Default
        {
            get
            {
                return _default;
            }
        }

        private static RenderParams _debug = new RenderParams( 
            RenderMode.Texture | 
            RenderMode.TraceBoundingBox | 
            RenderMode.TraceLogicalExtent | 
            RenderMode.TraceRenderingExtent, 
            null, null, null );

        public static RenderParams Debug
        {
            get
            {
                return _debug;
            }
        }

        public RenderMode Mode;

        public Color TraceRenderingExtentColor;
        public Color TraceLogicalExtentColor;
        public Color TraceBoundingBoxColor;

        public Getter<ISprite> GetTexture_MarkCenter;
        public Getter<ISprite> GetTexture_MarkTopLeft;
        public Getter<ISprite> GetTexture_MarkOrigin;

        public RenderParams( 
            RenderMode mode,
            Getter<ISprite> getTexture_MarkCenter,
            Getter<ISprite> getTexture_MarkTopLeft,
            Getter<ISprite> getTexture_MarkOrigin )
        {
            Mode = mode;
            GetTexture_MarkCenter = getTexture_MarkCenter;
            GetTexture_MarkTopLeft = getTexture_MarkTopLeft;
            GetTexture_MarkOrigin = getTexture_MarkOrigin;

            TraceRenderingExtentColor = Color.Orange;
            TraceLogicalExtentColor = Color.Teal;
            TraceBoundingBoxColor = Color.Aqua;
        }
    }
}
