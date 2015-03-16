using System;
using System.Collections.Generic;

namespace XenAspects
{
    public struct HeapState
    {
        public static HeapState Neutral = new HeapState( 0, 0 );

        public static HeapState GetHeapDelta( HeapState before, HeapState after )
        {
            return new HeapState( after.GCCount - before.GCCount, after.TotalMemoryAllocated - before.TotalMemoryAllocated );
        }

        public int GCCount;
        public long TotalMemoryAllocated;

        public HeapState( int gcCount, long totalMem )
        {
            GCCount = gcCount;
            TotalMemoryAllocated = totalMem;
        }
    }

    public static class HeapUtility
    {
        static long totalMemory;
        static int gcCount;
        static HeapState lastHeapState, tempHeapState;

        public static void CollectGarbage()
        {
            GC.Collect();
            GC.GetTotalMemory( true );
            GC.Collect();
            GC.WaitForPendingFinalizers();
#if WINDOWS
            //I HATE .NETCF
            GC.WaitForFullGCComplete( -1 );
#endif
        }

        public static long GetTotalAllocatedMemory()
        {
            CollectGarbage();
            return GC.GetTotalMemory( false );
        }

        public static int GetTotalCollectionCount()
        {
            int collectionCount = 0;
#if WINDOWS
            for( int i = 0; i <= GC.MaxGeneration; i++ )
            {
                collectionCount += GC.CollectionCount( i );
            }
#endif
            return collectionCount;
        }

        public static HeapState GetHeapStateBefore()
        {
            totalMemory = HeapUtility.GetTotalAllocatedMemory();
            gcCount = HeapUtility.GetTotalCollectionCount();
            lastHeapState.GCCount = gcCount;
            lastHeapState.TotalMemoryAllocated = totalMemory;
            return lastHeapState;
        }

        public static HeapState GetHeapStateAfter()
        {
            gcCount = HeapUtility.GetTotalCollectionCount();
            totalMemory = HeapUtility.GetTotalAllocatedMemory();
            lastHeapState.GCCount = gcCount;
            lastHeapState.TotalMemoryAllocated = totalMemory;
            return lastHeapState;
        }

        public static HeapState GetHeapStateDelta()
        {
            tempHeapState = lastHeapState;
            return HeapState.GetHeapDelta( tempHeapState, GetHeapStateAfter() );
        }
    }
}