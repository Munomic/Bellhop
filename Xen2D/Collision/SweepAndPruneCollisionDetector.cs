using System.Collections.Generic;
using XenAspects;

namespace Xen2D
{
    /// <summary>
    /// Collision detector that sweeps along a single axis to reduce the number of comparisons for collision
    /// </summary>
    public sealed class SweepAndPruneCollisionDetector : CollisionDetector
    {
        private IComparer<ICollidableObject> _comparer = new CollisionPropertiesLowestXComparer();
        private List<ICollidableObject> CollisionItems { get { return CollisionObjects.Items; } }

        public override void CheckCollisionsInternal()
        {
            SortCollidableObjects();
            FindOverlappingElements();
        }

        private void SortCollidableObjects()
        {
            SortingAlgorithms.InsertionSort<ICollidableObject>( CollisionItems, _comparer );
        }

        private void FindOverlappingElements()
        {
            ICollidableObject currentItem;
            ICollidableObject toCompare;

            //_collidableEntities is sorted by Extent.LowestX
            for( int i = 0; i < CollisionItems.Count; i++ )
            {
                currentItem = CollisionItems[ i ];
                if( !currentItem.IsCollidable )
                {
                    continue;
                }

                for( int j = i + 1; j < CollisionItems.Count; j++ )
                {
                    toCompare = CollisionItems[ j ];

                    if( !toCompare.IsCollidable )
                    {
                        continue;
                    }

                    if( toCompare.CollisionExtent.LowestX > currentItem.CollisionExtent.HighestX )
                    {
                        //no overlap on x axis, collision for this and all following extents is not possible
                        break;
                    }

                    if( ( currentItem.CollisionExtent.HighestY < toCompare.CollisionExtent.LowestY ) ||
                        ( toCompare.CollisionExtent.HighestY < currentItem.CollisionExtent.LowestY ) )
                    {
                        //bounds check along remaining axis.  If the y-axis projection does not intersect, this 
                        //cannot possibly be a collision.  
                        continue;
                    }
                    CheckCollisionFor( currentItem, toCompare );
                }
            }
        }
    }
}


