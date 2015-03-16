using System;
using System.Diagnostics;

namespace XenAspects
{
    public delegate T FactoryDelegate<T>();

    public interface IComposedField
    {
        //Resets the composed object to its initial state, as though it had just been acquired from the pool.
        //Use this method to avoid an expire/acquire cycle.  
        void ResetField();

        void ReleaseField();

        void AcquireField();
    }

    public interface IComposedField<FieldType> : IComposedField
        where FieldType : IComposableObject
    {
        FieldType Value { get; }
    }

    /// <summary>
    /// Describes a field that is part of a composable object.  If registered with the composable object via DeclareComposition, 
    /// this field will automatically be acquired/reset/expired when the parent is acquired/reset/expired.  
    /// </summary>
    /// <typeparam name="FieldType">The data contained by the field.</typeparam>
    public class ComposedField<FieldType> : IComposedField<FieldType>
        where FieldType : IComposableObject
    {
        FieldType _field = default( FieldType );
        FactoryDelegate<FieldType> _poolAcquireDelegate;
        Action<IPooledObject> _fieldOnReleasedDelegate;

        public FieldType Value { get { return _field; } }

        public ComposedField( FactoryDelegate<FieldType> acquireDelegate )
        {
            Debug.Assert( null != acquireDelegate, "acquireDelegate cannot be null" );
            _poolAcquireDelegate = acquireDelegate;
            _fieldOnReleasedDelegate = new Action<IPooledObject>( FieldOnReleased );
        }

        public void ResetField()
        {
            _field.Reset();
        }

        public void ReleaseField()
        {
            _field.Release();
            _field = default( FieldType );
        }

        public void AcquireField()
        {
            if( null == _field )
            {
                _field = _poolAcquireDelegate.Invoke();
                _field.OnReleased.Add( _fieldOnReleasedDelegate );
            }
            else
            {
                _field.Reset();
            }
        }

        /// <summary>
        /// Event handler for when the subject field expires.  
        /// </summary>
        /// <param name="obj"></param>
        protected void FieldOnReleased( IPooledObject obj )
        {
            _field.OnReleased.Remove( _fieldOnReleasedDelegate );
        }
    }
}