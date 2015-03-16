using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XenAspects;

namespace Xen2D
{
    public class CachedTextureDescriptor : ComposableObject<CachedTextureDescriptor>, ITextureInfo
    {
        public static CachedTextureDescriptor Acquire( Texture2D cachedTexture )
        {
            var instance = Pool.Acquire();
            instance.Reset( cachedTexture );
            return instance;
        }

        protected Rectangle _sourceRect;
        protected Texture2D _asset;

        public Texture2D Asset
        {
            get { return _asset; }
        }

        public virtual Rectangle SourceRectangle
        {
            get { return _sourceRect; }
        }

        public CachedTextureDescriptor() { }

        public void Reset( Texture2D cachedTexture )
        {
            _asset = cachedTexture;
            _sourceRect = new Rectangle( 0, 0, cachedTexture.Width, cachedTexture.Height );
        }
    }
}
