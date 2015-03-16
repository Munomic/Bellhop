using System;
using System.Collections.Generic;

namespace XenAspects
{
    public interface IEvent
    {
        int NumSubscribers { get; }
        void Add( Action handler );
        bool Remove( Action handler );
        void Clear();
        void Notify();
    }
    
    public interface IEvent<T>
    {
        int NumSubscribers { get; }
        void Add( Action<T> handler );
        bool Remove( Action<T> handler );
        void Clear();
        void Notify( T args );
    }

    /// <summary>
    /// An alternative to .net events that does not generate garbage when adding/removing/invoking
    /// callback handlers.  
    /// </summary>
    public class Event : IEvent
    {
        /// <summary>
        /// Default subscriber capacity
        /// </summary>
        public static int DefaultNumCallbacks = 32;
        private NList<Action> _callbackDelegates = null;

        /// <summary>
        /// Indicates the number of Action subscribers to this event
        /// </summary>
        public int NumSubscribers { get { return _callbackDelegates.Items.Count; } }

        /// <summary>
        /// Creates an Event
        /// </summary>
        public Event() : this( DefaultNumCallbacks ) { }

        /// <summary>
        /// Creates an Event
        /// </summary>
        /// <param name="numCallbacks">Initial callback capacity</param>
        public Event( int numCallbacks )
        {
            _callbackDelegates = new NList<Action>( numCallbacks );
        }

        /// <summary>
        /// Subscribes an Action handler for the event
        /// </summary>
        /// <param name="handler">Action event handler</param>
        public void Add( Action handler )
        {
            if( !_callbackDelegates.Contains( handler ) )
                _callbackDelegates.Add( handler );
        }

        /// <summary>
        /// Removes an Action handler from the event
        /// </summary>
        /// <param name="handler">Action event handler</param>
        /// <returns>True on Success, False on Failure</returns>
        public bool Remove( Action handler )
        {
            return _callbackDelegates.Remove( handler );
        }

        /// <summary>
        /// Removes all event handlers from the event
        /// </summary>
        public void Clear()
        {
            _callbackDelegates.Clear();
        }

        /// <summary>
        /// Calls all Actions subscribed to the event
        /// </summary>
        public virtual void Notify()
        {
            _callbackDelegates.BeginEnumeration();

            foreach( var callback in _callbackDelegates.Items )
            {
                callback.Invoke();
            }

            _callbackDelegates.EndEnumeration();
        }
    }

    /// <summary>
    /// An alternative to .net events that does not generate garbage when adding/removing/invoking
    /// callback handlers.  
    /// </summary>
    /// <typeparam name="T">The type of the event argument.</typeparam>
    public class Event<T> : IEvent<T>
    {
        /// <summary>
        /// Default subscriber capacity
        /// </summary>
        public static int DefaultNumCallbacks = 32;
        private NList<Action<T>> _callbackDelegates = null;
        
        /// <summary>
        /// Indicates the number of Action subscribers to this event
        /// </summary>
        public int NumSubscribers { get { return _callbackDelegates.Items.Count; } }

        /// <summary>
        /// Creates an Event
        /// </summary>
        public Event() : this( DefaultNumCallbacks ) { }

        /// <summary>
        /// Creates an Event
        /// </summary>
        /// <param name="numCallbacks">Initial callback capacity</param>
        public Event( int numCallbacks )
        {
            _callbackDelegates = new NList<Action<T>>( numCallbacks );
        }

        /// <summary>
        /// Subscribes an Action handler for the event
        /// </summary>
        /// <param name="handler">Action event handler</param>
        public void Add( Action<T> handler )
        {
            if( !_callbackDelegates.Contains( handler ) )
                _callbackDelegates.Add( handler );
        }
        
        /// <summary>
        /// Removes an Action handler from the event
        /// </summary>
        /// <param name="handler">Action event handler</param>
        /// <returns>True on Success, False on Failure</returns>
        public bool Remove( Action<T> handler )
        {
            return _callbackDelegates.Remove( handler );
        }
        
        /// <summary>
        /// Removes all event handlers from the event
        /// </summary>
        public void Clear()
        {
            _callbackDelegates.Clear();
        }

        /// <summary>
        /// Calls all Actions subscribed to the event
        /// </summary>
        public virtual void Notify( T args )
        {
            _callbackDelegates.BeginEnumeration();
            
            foreach( var callback in _callbackDelegates.Items )
            {
                callback.Invoke( args );
            }

            _callbackDelegates.EndEnumeration();
        }
    }
}