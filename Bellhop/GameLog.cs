using System;

namespace Jubble
{
    public static class GameLog
    {
        public static void Log( string msg )
        {
            Console.WriteLine( msg );
        }

		public static void Log( string msg, params object[] args )
        {
            Console.WriteLine( msg, args );
        }
    }
}

