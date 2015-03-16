using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Xen2D
{
    public interface IAssetDescriptor<AssetType>
    {
        AssetType Asset { get; }
    }

    /// <summary>
    /// Base class for describing a game asset, e.g., texture, sound
    /// </summary>
    /// <typeparam name="AssetType">The type of the asset.</typeparam>
    public class AssetDescriptor<AssetType> : IAssetDescriptor<AssetType>, IDisposable
    {
        ContentManager _contentManager = null;
        string _identifier;
        AssetType _asset = default( AssetType );

        public ContentManager ContentManager
        {
            get{ return _contentManager; }
            set{ _contentManager = value; }
        }

        public string Identifier
        {
            get{ return _identifier; }
            set{ _identifier = value; }
        }

        public AssetType Asset{ get{ return _asset; } }

        public AssetDescriptor( ContentManager contentManager, string assetIdentifier )
        {
            ContentManager = contentManager;
            Identifier = assetIdentifier;
            Load();
        }

        private void Load()
        {
            if( null != _contentManager )
            {
                _asset = _contentManager.Load<AssetType>( Identifier );
            }
        }

        public void Dispose()
        {
            // TODO: Should we really be manually calling Dispose for these? Or should _contentManager.Unload() handle it?
            //_asset.Dispose();
            _contentManager = null;
            _identifier = string.Empty;
        }
    }
}
