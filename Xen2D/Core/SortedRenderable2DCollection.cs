using System.Collections.Generic;
using XenAspects;

namespace Xen2D
{
    public interface IDrawable2DComparer : IComparer<IDrawable2D> { }

    /// <summary>
    /// This comparer sorts IDrawable2D by DrawOrder.  It can be used to sort a collection comparers 
    /// so that they end in decrementing order.  
    /// </summary>
    /// <example>
    /// Unsorted DrawOrder Collection: {7, 2, 3, 1}
    /// Sorted DrawOrder Collection: {7, 3, 2, 1}
    /// </example>
    public class DecrementingDrawable2DComparer : IDrawable2DComparer
    {
        public static readonly DecrementingDrawable2DComparer Instance = new DecrementingDrawable2DComparer();

        private DecrementingDrawable2DComparer() { }

        public int Compare( IDrawable2D x, IDrawable2D y )
        {
            return y.DrawOrder - x.DrawOrder;
        }
    }

    /// <summary>
    /// This comparer sorts IDrawable2D by DrawOrder.  It can be used to sort a collection comparers 
    /// so that they end in incrementing order.  
    /// </summary>
    /// <example>
    /// Unsorted DrawOrder Collection: {7, 2, 3, 1}
    /// Sorted DrawOrder Collection: {1, 2, 3, 7}
    /// </example>
    public class IncrementingDrawable2DComparer : IDrawable2DComparer
    {
        public static readonly IncrementingDrawable2DComparer Instance = new IncrementingDrawable2DComparer();

        private IncrementingDrawable2DComparer() { }

        public int Compare( IDrawable2D x, IDrawable2D y )
        {
            return x.DrawOrder - y.DrawOrder;
        }
    }

    public interface IRenderable2DComparer : IComparer<IRenderable2D> { }

    /// <summary>
    /// This comparer sorts IDrawable2D by DrawOrder.  It can be used to sort a collection comparers 
    /// so that they end in decrementing order.  
    /// </summary>
    /// <example>
    /// Unsorted DrawOrder Collection: {7, 2, 3, 1}
    /// Sorted DrawOrder Collection: {7, 3, 2, 1}
    /// </example>
    public class DecrementingRenderable2DComparer : IRenderable2DComparer
    {
        public static readonly DecrementingRenderable2DComparer Instance = new DecrementingRenderable2DComparer();

        private DecrementingRenderable2DComparer() { }

        public int Compare( IRenderable2D x, IRenderable2D y )
        {
            return y.DrawOrder - x.DrawOrder;
        }
    }

    /// <summary>
    /// This comparer sorts IDrawable2D by DrawOrder.  It can be used to sort a collection comparers 
    /// so that they end in incrementing order.  
    /// </summary>
    /// <example>
    /// Unsorted DrawOrder Collection: {7, 2, 3, 1}
    /// Sorted DrawOrder Collection: {1, 2, 3, 7}
    /// </example>
    public class IncrementingRenderable2DComparer : IRenderable2DComparer
    {
        public static readonly IncrementingRenderable2DComparer Instance = new IncrementingRenderable2DComparer();

        private IncrementingRenderable2DComparer() { }

        public int Compare( IRenderable2D x, IRenderable2D y )
        {
            return x.DrawOrder - y.DrawOrder;
        }
    }

    /// <summary>
    /// This specialized SortedPooledObjectNList keeps the contents of the collection in a sorted
    /// order by DrawOrder is a SortComparer is specified.
    /// </summary>
    public class SortedRenderable2DCollection : SortedPooledObjectNList<IRenderable2D>
    {
        public SortedRenderable2DCollection()
        {
            SortComparer = IncrementingRenderable2DComparer.Instance;
        }
    }

    /// <summary>
    /// This specialized SortedPooledObjectNList keeps the contents of the collection in a sorted
    /// order by DrawOrder is a SortComparer is specified.
    /// </summary>
    public class SortedRenderable2DCollection<T> : SortedPooledObjectNList<T>
        where T : class, IDrawable2D, IPooledObject
    {
    }
}
