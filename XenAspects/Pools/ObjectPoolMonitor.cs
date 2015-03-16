using System;
using System.Collections.Generic;

namespace XenAspects
{
    /// <summary>
    /// This class maintains a list of all object pools that used by the process.  
    /// It can be used to reclaim expired objects for re-use in their object pools.
    /// </summary>
    public static class ObjectPoolMonitor
    {
        private static List<IObjectPool> _objectPools = new List<IObjectPool>();
        private static bool _lockAllPools = false;

        public static bool LockAllPools
        {
            get { return _lockAllPools; }
            set
            {
                _lockAllPools = value;
                foreach( var pool in _objectPools )
                {
                    pool.Locked = value;
                }
            }
        }

        public static void Register( IObjectPool pool )
        {
            _objectPools.Add( pool );
        }

        //public static void CleanUpAllRegisteredPools()
        //{
        //    foreach( IObjectPool pool in _objectPools )
        //    {
        //        pool.CleanUp();
        //    }
        //}
    }
}