using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    public interface ICompositeExtent : IRectangularExtent, INList<IExtent>
    {
    }

    /// <summary>
    /// This class represents a composite of extents that can contain other extents.  
    /// </summary>
    public class CompositeExtent : RectangularExtentBase<CompositeExtent>, ICompositeExtent
    {
        private static int DefaultChildCollectionSize = 32;
        private PooledObjectNList<IExtent> _children = new PooledObjectNList<IExtent>( DefaultChildCollectionSize );
        private Action _onChildExtentChanged = null;
        private Action<IExtent> _onChildAdded;
        private Action<IExtent> _onChildRemoved;

        public CompositeExtent()
        {
            _onChildExtentChanged = new Action( OnChildExtentChanged );
            _onChildAdded = new Action<IExtent>( OnChildAdded );
            _onChildRemoved = new Action<IExtent>( OnChildRemoved );
            
            _children.OnItemAfterAdded.Add( _onChildAdded );
            _children.OnItemAfterRemoved.Add( _onChildRemoved );

            Reset( 0, 0 );
        }

        private void OnChildAdded( IExtent child )
        {
            Debug.Assert( child.ParentSpace == null, "Extent has an existing parent, remove it from its current parent before adding it to another" );

            child.ParentSpace = this;
            child.OnChanged.Add( _onChildExtentChanged );

            RecalculateBounds();
        }

        private void OnChildRemoved( IExtent child )
        {
            child.ParentSpace = null;
            child.OnChanged.Remove( _onChildExtentChanged );

            EnumerateAndFindBounds();
        }

        public int Count
        {
            get { return _children.Count; }
        }

        public List<IExtent> Items
        {
            get { return _children.Items; }
        }

        public IExtent this[ int index ]
        {
            get{ return _children[ index ]; }
            set{ _children[ index ] = value; }
        }

        public override void Reset( Vector2 anchor, float width, float height, Vector2 origin )
        {
            base.Reset( anchor, width, height, origin );
            _children.Clear();
        }

        public override bool Intersects( ICompositeExtent other )
        {
            foreach( var child in Items )
            {
                if( child.Intersects( other ) )
                    return true;
            }
            return false;
        }

        public override bool Intersects( IPolygonExtent other )
        {
            foreach( var child in Items )
            {
                if( child.Intersects( other ) )
                    return true;
            }
            return false;
        }

        public override bool Intersects( ICircularExtent other )
        {
            foreach( var child in Items )
            {
                if( child.Intersects( other ) )
                    return true;
            }
            return false;
        }

        protected override void AfterSetScaleTemplate( Vector2 scale )
        {
            base.AfterSetScaleTemplate( scale );
            foreach( var child in _children.Items )
            {
                child.RecalculateBounds();
            }
        }

        public void Clear()
        {
            _children.Clear();
        }

        private void RecalculateChildBounds()
        {
            foreach( var extent in _children.Items )
            {
                extent.RecalculateBounds();
            }
        }

        public override void RecalculateBounds()
        {
            RecalculateChildBounds();
            EnumerateAndFindBounds();
        }

        private void EnumerateAndFindBounds()
        {
            if( _children.Items.Count == 0 )
            {
                HighestX = Anchor.X;
                HighestY = Anchor.Y;
                LowestX = Anchor.X;
                LowestY = Anchor.Y;
                return;
            }

            var firstChild = _children.Items[ 0 ];

            HighestX = firstChild.HighestX;
            HighestY = firstChild.HighestY;
            LowestX = firstChild.LowestX;
            LowestY = firstChild.LowestY;

            foreach( var child in _children.Items )
            {
                HighestX = MathHelper.Max( HighestX, child.HighestX );
                HighestY = MathHelper.Max( HighestY, child.HighestY );
                LowestX = MathHelper.Min( LowestX, child.LowestX );
                LowestY = MathHelper.Min( LowestY, child.LowestY );
            }
        }

        private void OnChildExtentChanged()
        {
            EnumerateAndFindBounds();
        }

        public bool Add( IExtent extent )
        {
            return _children.Add( extent );
        }

        public bool Contains( IExtent extent )
        {
            return _children.Contains( extent );
        }

        public override bool Contains( Vector2 point )
        {
            bool containsPoint = false;
            foreach( var child in _children.Items )
            {
                if( child.ContainsPoint( point ) )
                {
                    containsPoint = true;
                    break;
                }
            }
            return containsPoint;
        }

        public void BeginEnumeration()
        {
            _children.BeginEnumeration();
        }

        public void EndEnumeration()
        {
            _children.EndEnumeration();
        }

        public int IndexOf( IExtent extent )
        {
            return _children.IndexOf( extent );
        }

        public bool Insert( int index, IExtent extent )
        {
            return _children.Insert( index, extent );
        }

        public bool RemoveAt( int index )
        {
            return _children.RemoveAt( index );
        }

        public bool Remove( IExtent extent )
        {
            return _children.Remove( extent );
        }
    }
}