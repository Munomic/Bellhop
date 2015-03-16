using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XenAspects;

namespace Xen2D
{
    public class CompositeRenderable2D : Renderable2DBase<CompositeRenderable2D>, ICompositeRenderable2D
    {
        private SortedRenderable2DCollection _children = new SortedRenderable2DCollection();
        private ICompositeExtent _renderingExtent = new CompositeExtent();

        public void AddExtent( IExtent extent )
        {
            _renderingExtent.Add( extent );
        }

        protected override void DrawInternal( SpriteBatch spriteBatch, GameTime gameTime, Matrix transformFromWorldToCamera )
        {
            _children.BeginEnumeration();
            foreach( var child in _children.Items )
                child.Draw( spriteBatch, gameTime, transformFromWorldToCamera );
            _children.EndEnumeration();
        }

        protected override void UpdateInternal( GameTime gameTime )
        {
            base.UpdateInternal( gameTime );
            _children.BeginEnumeration();
            foreach( var child in _children.Items )
                child.Update( gameTime );
            _children.EndEnumeration();
        }

        public override IRectangularExtent RenderingExtent{ get{ return _renderingExtent; } }

        #region ISortedPooledObjectNList<IRenderable2D> Members (redirected to _children)

        public IComparer<IRenderable2D> SortComparer
        {
            get{ return _children.SortComparer; }
            set{ _children.SortComparer = value; }
        }

        public void ReleaseAndClearChildren()
        {
            _children.ReleaseAndClearChildren();
        }

        public Event<IRenderable2D> OnItemBeforeAdded
        {
            get { return _children.OnItemBeforeAdded; }
        }

        public Event<IRenderable2D> OnItemAfterAdded
        {
            get { return _children.OnItemAfterAdded; }
        }

        public Event<IRenderable2D> OnItemBeforeRemoved
        {
            get { return _children.OnItemBeforeRemoved; }
        }

        public Event<IRenderable2D> OnItemAfterRemoved
        {
            get { return _children.OnItemAfterRemoved; }
        }

        public Event<IRenderable2D> OnItemReleased
        {
            get { return _children.OnItemReleased; }
        }

        public bool Add( IRenderable2D item )
        {
            bool result = _children.Add( item );

            if( result )
                _renderingExtent.Add( item.RenderingExtent );

            return result;
        }

        public void BeginEnumeration()
        {
            _children.BeginEnumeration();
        }

        public void Clear()
        {
            _children.Clear();
            _renderingExtent.Clear();
        }

        public bool Contains( IRenderable2D item )
        {
            return _children.Contains( item );
        }

        public int Count
        {
            get { return _children.Count; }
        }

        public void EndEnumeration()
        {
            _children.EndEnumeration();
        }

        public int IndexOf( IRenderable2D item )
        {
            return _children.IndexOf( item );
        }

        public bool Insert( int index, IRenderable2D item )
        {
            bool result = _children.Insert( index, item );
            if( result )
                _renderingExtent.Add( item.RenderingExtent );
            return result;
        }

        public List<IRenderable2D> Items
        {
            get { return _children.Items; }
        }

        public bool Remove( IRenderable2D item )
        {
            bool result = _children.Remove( item );
            if( result )
                _renderingExtent.Remove( item.RenderingExtent );
            return result;
        }

        public bool RemoveAt( int index )
        {
            var item = _children[ index ];

            bool result = _children.RemoveAt( index );
            if( result )
                _renderingExtent.Remove( item.RenderingExtent );
            return result;
        }

        public IRenderable2D this[ int index ]
        {
            get { return _children[ index ]; }
        }
        #endregion

        public CompositeRenderable2D()
        {
            Reset();
        }

        protected override void OnOpacityChanged()
        {
            base.OnOpacityChanged();

            foreach( var child in _children.Items )
                child.OpacityModifier = OpacityFinal;
        }
    }
}
