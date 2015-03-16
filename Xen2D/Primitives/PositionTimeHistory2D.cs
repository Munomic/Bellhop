using System;
using Microsoft.Xna.Framework;
using XenAspects;

namespace Xen2D
{
    public interface IPositionTimeHistory2D
    {
        /// <summary>
        /// Retrieves the last position time entry that was recorded (according to its time property).  This property is read-only.
        /// </summary>
        PositionTimeEntry2D Last { get; set; }
        PositionTimeEntry2D SecondToLast { get; }

        /// <summary>
        /// Retriesve the last velocity in units per second.  
        /// </summary>
        Vector2 LastVelocity { get; }

        void Add( PositionTimeEntry2D entry );
        void Add( Vector2 position, float angle, TimeSpan time );

        int Count { get; }
    }

    /// <summary>
    /// This class is responsible for maintaing a list of PositionTimeEntries in sorted order to retrieve the latest one quickly.  
    /// </summary>
    public sealed class PositionTimeHistory2D : ComposableObject<PositionTimeHistory2D>, IPositionTimeHistory2D
    {
        public const int DefaultCapacity = 4;

        private PositionTimeEntry2D[] _entries = new PositionTimeEntry2D[ DefaultCapacity ];
        private TimeSpan _lastEntryTime = PositionTimeEntry2D.InvalidTime;
        private int _lastEntryIndex;

        public int Count { get; private set; }

        public PositionTimeEntry2D Last
        {
            get { return _entries[ _lastEntryIndex ]; }
            set { _entries[ _lastEntryIndex ] = value; }
        }

        public PositionTimeEntry2D SecondToLast
        {
            get
            {
                int secondToLastEntryIndex = _lastEntryIndex - 1;
                
                //If the second to last entry is an invalid index, it means the entries wrapped.
                if( secondToLastEntryIndex < 0 )
                    secondToLastEntryIndex += DefaultCapacity;

                return _entries[ secondToLastEntryIndex ];
            }
        }

        public Vector2 LastVelocity
        {
            get
            {
                if( Count < 2 )
                {
                    return Vector2.Zero;
                }
                return PositionTimeEntry2D.GetVelocity( SecondToLast, Last );
            }
        }

        public void Add( PositionTimeEntry2D entry )
        {
            Add( entry.Position, entry.Angle, entry.Time );
        }

        public void Add( Vector2 position, float angle, TimeSpan time )
        {
            if( time == _lastEntryTime )
                return; // Note: if the entry time is too quick since the last call, it can report as the same time.
            if( ( time <= _lastEntryTime ) && ( time != TimeSpan.Zero ) )
                throw new PositionHistoryOrderException( "Cannot add an entry whose time equals or precedes the last recorded time" );

            _lastEntryTime = time;

            if( Count < DefaultCapacity )
                Count++;

            _lastEntryIndex++;
            if( _lastEntryIndex == DefaultCapacity )
                _lastEntryIndex = 0;

            _entries[ _lastEntryIndex ].Position    = position;
            _entries[ _lastEntryIndex ].Angle       = angle;
            _entries[ _lastEntryIndex ].Time        = time;
        }

        protected override void ResetDirectState()
        {
            base.ResetDirectState();
            for( int i = 0; i < _entries.Length; i++ )
                _entries[ i ] = PositionTimeEntry2D.Invalid;
        }
    }
}
