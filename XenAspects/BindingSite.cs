using System;
using System.Diagnostics;

namespace XenAspects
{
    public interface IBindingSite<HostType, TargetType>
    {
        bool IsBound { get; }
        HostType Host { get; }
        TargetType Target { get; }
        void Unbind();
        bool Bind( IBindingSite<TargetType, HostType> targetBindingSite );
        void BindBack( IBindingSite<TargetType, HostType> targetBindingSite );
    }

    public class BindingSite<HostType, TargetType> : IBindingSite<HostType, TargetType>
        where HostType : class
        where TargetType : class
    {
        public bool IsBound { get { return ( _otherBindingSite != null ); } }

        public HostType Host { get; protected set; }
        protected IBindingSite<TargetType, HostType> _otherBindingSite = null;

        public TargetType Target
        {
            get
            {
                return ( null != _otherBindingSite ) ? _otherBindingSite.Host : null;
            }
        }

        public BindingSite( HostType host )
        {
            Host = host;
        }

        public void Unbind()
        {
            if( IsBound )
            {
                var otherBindingSite = _otherBindingSite;
                _otherBindingSite = null;
                otherBindingSite.Unbind();
            }
        }

        public bool Bind( IBindingSite<TargetType, HostType> targetBindingSite )
        {
            if( ( null == targetBindingSite ) || targetBindingSite.IsBound || IsBound )
            {
                return false;
            }

            _otherBindingSite = targetBindingSite;
            _otherBindingSite.BindBack( this );
            return true;
        }

        public bool ForceBind( IBindingSite<TargetType, HostType> targetBindingSite )
        {
            if( ( null == targetBindingSite ) || targetBindingSite.IsBound )
            {
                return false;
            }

            Unbind();
            return Bind( targetBindingSite );
        }

        /// <summary>
        /// DO NOT CALL THIS METHOD DIRECTLY.  EXPOSED ONLY BECAUSE OF INTERFACE.
        /// When a binding is made against this site, a bind back will be made against the initiating site.  
        /// </summary>
        /// <param name="targetBindingSite">The other binding site.</param>
        public void BindBack( IBindingSite<TargetType, HostType> targetBindingSite )
        {
            Debug.Assert( null != targetBindingSite );
            _otherBindingSite = targetBindingSite;
        }
    }
}