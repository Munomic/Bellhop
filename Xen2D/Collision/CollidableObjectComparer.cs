using System.Collections.Generic;
using XenAspects;

namespace Xen2D
{
    public class CollisionPropertiesLowestXComparer : ComparerBase<ICollidableObject>
    {
        protected override float GetComparisonValue( ICollidableObject co )
        {
            return co.CollisionExtent.LowestX;
        }
    }
}


