using System;
using Microsoft.Xna.Framework;

namespace Xen2D
{
    public class PositionHistoryOrderException : Exception
    {
        public PositionHistoryOrderException( string message ) : base( message ) { }
        public PositionHistoryOrderException( string message, Exception inner ) : base( message, inner ) { }
    }

    /// <summary>
    /// Records the Position and Angle of a 2D entity at a moment in time since game start.
    /// </summary>
    public struct PositionTimeEntry2D
    {
        public static readonly Vector2 InvalidPosition = new Vector2( -1, -1 );
        public static readonly float InvalidAngle = -1.0f;
        public static readonly TimeSpan InvalidTime = new TimeSpan( -1, 0, 0 );
        public static readonly PositionTimeEntry2D Invalid = new PositionTimeEntry2D( InvalidPosition, InvalidAngle, InvalidTime );

        public static Vector2 GetDisplacement( PositionTimeEntry2D start, PositionTimeEntry2D stop )
        {
            return start.Position - stop.Position;
        }

        public static TimeSpan GetElapsedTime( PositionTimeEntry2D start, PositionTimeEntry2D stop )
        {
            return start.Time - stop.Time;
        }

        public static Vector2 GetVelocity( PositionTimeEntry2D start, PositionTimeEntry2D stop )
        {
            return GetDisplacement( start, stop ) / ( (float)( ( GetElapsedTime( start, stop ).TotalMilliseconds ) / 1000 ) );
        }

        public float Angle { get; set; }
        public Vector2 Position { get; set; }
        public TimeSpan Time { get; set; }

        public PositionTimeEntry2D( Vector2 position, GameTime time ) : this( position, time.TotalGameTime ) { }
        public PositionTimeEntry2D( Vector2 position, TimeSpan time ) : this( position, 0, time ) { }
        public PositionTimeEntry2D( Vector2 position, float angle, TimeSpan time )
            : this()
        {
            Position = position;
            Angle = angle;
            Time = time;
        }
    }
}
