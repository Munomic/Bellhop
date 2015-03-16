using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xen2D
{
    /// <summary>
    /// Reduced version of Microsoft.Xna.Framework.IUpdateable.
    /// </summary>
    public interface IUpdateableEx
    {
        bool Enabled { get; set; }
        int UpdateOrder { get; set; }
        void Update( GameTime gameTime );
    }
}
