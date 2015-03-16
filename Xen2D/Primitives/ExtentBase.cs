using System;
using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    public abstract class ExtentBase<DerivedType, RegionType> : ComposableObject<DerivedType>, IExtent<RegionType>
        where DerivedType : ComposableObject, new()
        where RegionType : ComposableObject, IShape, new()
    {
        private Space2DTranslation _transform = Space2DTranslation.Create();
        private RegionType _region = new RegionType();
        private float _angle = 0;
        private Vector2 _scale = new Vector2( 1, 1 );
        private Vector2 _anchor = VectorUtility.Zero;
        private Vector2 _origin = VectorUtility.Zero;
        protected Vector2 InnerRadiusVector { get; set; }
        protected Vector2 OuterRadiusVector { get; set; }
        private IEvent _onChangedEvent = new Event();

        public RegionType ReferenceRegion
        {
            get { return _region; }
            protected set { _region = value; }
        }

        public int NumSides
        {
            get { return ReferenceRegion.NumSides; }
        }

        public IEvent OnChanged
        {
            get{ return _onChangedEvent; }
        }

        public Vector2 ActualCenter{ get; protected set; }

        public float Angle
        {
            get { return _angle; }
            set
            {
                if( _angle != value )
                {
                    BeforeSetAngleTemplate( value );
                    _angle = value;
                    AfterSetAngleTemplate( value );
                }
            }
        }

        protected virtual void BeforeSetAngleTemplate( float angle ) { }
        protected virtual void AfterSetAngleTemplate( float angle ) { RecalculateTransform(); }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if( _scale != value )
                {
                    BeforeSetScaleTemplate( value );
                    _scale = value;
                    AfterSetScaleTemplate( value );
                }
            }
        }

        protected virtual void BeforeSetScaleTemplate( Vector2 scale ) { }
        protected virtual void AfterSetScaleTemplate( Vector2 scale ) 
        { 
            RecalculateTransform();
        }

        public Vector2 Anchor
        {
            get { return _anchor; }
            set
            {
                if( _anchor != value )
                {
                    BeforeSetAnchorTemplate( value );
                    _anchor = value;
                    AfterSetAnchorTemplate( value );
                }
            }
        }

        protected virtual void BeforeSetAnchorTemplate( Vector2 anchor ) { }
        protected virtual void AfterSetAnchorTemplate( Vector2 anchor ) { RecalculateTransform(); }

        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                if( _origin != value )
                {
                    BeforeSetOriginTemplate( value );
                    Vector2 delta = value - _origin;
                    _origin = value;
                    _anchor += delta;
                    AfterSetOriginTemplate( value );
                }
            }
        }

        protected virtual void BeforeSetOriginTemplate( Vector2 origin ) { }
        protected virtual void AfterSetOriginTemplate( Vector2 origin ) { RecalculateTransform(); }

        public Space2DTranslation Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public ISpace2D ParentSpace { get; set; }

        public Matrix TranslateTo
        {
            get
            {
                return ( null == ParentSpace ) ?
                    Transform.TranslateTo :
                    Transform.TranslateTo * ParentSpace.TranslateTo;
            }
        }

        public Matrix TranslateFrom
        {
            get
            {
                return ( null == ParentSpace ) ?
                    Transform.TranslateFrom :
                    Transform.TranslateFrom * ParentSpace.TranslateFrom;
            }
        }

        protected virtual void SetTransformTemplate( Space2DTranslation transform ) { }

        public float HighestX { get; protected set; }
        public float HighestY { get; protected set; }
        public float LowestX { get; protected set; }
        public float LowestY { get; protected set; }

        /// <summary>
        /// Returns the radius of the largest circle that completely fits within the extent
        /// </summary>
        public float InnerRadius
        {
            get
            {
                return InnerRadiusVector.Length();
            }
        }

        /// <summary>
        /// Returns the radius of the smallest circle that completely contains the extent
        /// </summary>
        public float OuterRadius
        {
            get
            {
                return OuterRadiusVector.Length();
            }
        }

        protected override void ResetDirectState()
        {
            base.ResetDirectState();
            _angle = 0;
            _anchor = VectorUtility.Zero;
            _origin = VectorUtility.Zero;
            _scale = new Vector2( 1, 1 );
            ReferenceRegion.Reset();
            RecalculateTransform();
        }

        public void ReAnchor( Vector2 anchor )
        {
            Vector2 delta = Transform.TranslateVectorFromAbsoluteToThisSpace( anchor ) - Origin;
            Origin += delta;
        }

        protected virtual void RecalculateTransform()
        {
            Transform = Space2DTranslation.CreateFromTranslateFrom( MatrixUtility.GetTransformFromExtentToAbsolute( this ) );
            RecalculateBounds();
            RaiseOnChanged();
        }

        private void RaiseOnChanged()
        {
            _onChangedEvent.Notify();
        }

        /// <summary>
        /// Recalculates the bounding properties: Highest/Lowest X/Y
        /// </summary>
        public abstract void RecalculateBounds();

        public abstract bool Intersects( IPolygon2D other );
        public abstract bool Intersects( ICircle2D other );

        public abstract Vector2 FindClosestPoint( Vector2 point );        
        public abstract bool Contains( Vector2 point );

        public virtual bool ContainsPoint( Vector2 point )
        {
            return Contains( point );
        }

        public bool Intersects( IExtent other )
        {
            if( null == other )
                return false;

            ICompositeExtent composite = other as ICompositeExtent;
            if( null != composite )
                return Intersects( composite );

            IPolygonExtent polygon = other as IPolygonExtent;
            if( null != polygon )
                return Intersects( polygon );

            ICircularExtent circle = other as ICircularExtent;
            if( null != circle )
                return Intersects( circle );

            throw new NotImplementedException( "otherExtent must be either circle or polygon" );
        }

        public abstract bool Intersects( ICompositeExtent other );
        public abstract bool Intersects( IPolygonExtent other );
        public abstract bool Intersects( ICircularExtent other );

        public bool Intersects( CollisionMode thisCollisionMode, IExtent otherExtent, CollisionMode otherCollisionMode )
        {
            if( ( null == otherExtent ) ||
                ( thisCollisionMode == CollisionMode.None || otherCollisionMode == CollisionMode.None ) ||
                ( thisCollisionMode == CollisionMode.CenterPoint && otherCollisionMode == CollisionMode.CenterPoint ) )
            {
                return false;
            }

            return IntersectsImpl( thisCollisionMode, otherExtent, otherCollisionMode );
        }

        protected abstract bool IntersectsImpl( CollisionMode thisCollisionMode, IExtent otherExtent, CollisionMode otherCollisionMode );
    }
}