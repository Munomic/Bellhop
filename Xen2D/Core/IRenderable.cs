using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XenAspects;

namespace Xen2D
{
    /// <summary>
    /// Interface for a 2D component that can be updated and drawn.  
    /// </summary>
    public interface IComponent2D : IUpdateableEx, IDrawable2D, IPooledObject
    {
    }

    /// <summary>
    /// Interface for a 2D component that owns its own rendering extent and transform.
    /// </summary>
    public interface IRenderable2D : IComponent2D
    {
        IRectangularExtent RenderingExtent { get; }
        Space2DTranslation Transform { get; set; }
        IPositionTimeHistory2D History { get; }
    }

    public interface ICompositeRenderable2D : IRenderable2D, ISortedPooledObjectNList<IRenderable2D>
    {
        void AddExtent( IExtent extent );
    }
}
