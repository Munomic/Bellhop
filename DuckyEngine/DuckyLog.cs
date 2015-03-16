using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckyEngine
{
    public static class DuckyLogger
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
