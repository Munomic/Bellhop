using System;
using System.Collections.Generic;

namespace XenAspects
{
    public static class HeapTracker
    {
        private static HeapState _before, _after, _delta;

        public static HeapState Execute( Action action )
        {
            ResetBeforeHeapState();

            action.Invoke();

            _after = HeapUtility.GetHeapStateAfter();

            _delta.GCCount = _after.GCCount - _before.GCCount;
            _delta.TotalMemoryAllocated = _after.TotalMemoryAllocated - _before.TotalMemoryAllocated;
            return _delta;
        }

        public static void ResetBeforeHeapState()
        {
            _before = HeapUtility.GetHeapStateBefore();
        }

        public static HeapState GetDeltaFromExecute()
        {
            return _delta;
        }
    }
}