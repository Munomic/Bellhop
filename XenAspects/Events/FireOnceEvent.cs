using System;
using System.Collections.Generic;

namespace XenAspects
{
    /// <summary>
    /// An alternative to .net events that does not generate garbage when adding/removing/invoking
    /// callback handlers that removes all subscribers after the event has triggered
    /// </summary>
    public class FireOnceEvent<T> : Event<T>
    {
        /// <summary>
        /// Calls all Actions subscribed to the event and then removes all subscribers
        /// </summary>
        public override void Notify( T args )
        {
            base.Notify( args );
            Clear();
        }
    }
}