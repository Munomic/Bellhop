
namespace Xen2D
{
    public enum CollisionInteractionType
    {
        Invalid,
        ExtentVsExtent,
        ExtentVsPoint,
        ExtentVsInnerCircle,
        InnerCircleVsInnerCircle,
        InnerCircleVsPoint,
        InnerCircleVsExtent,
        PointVsPoint,
        PointVsExtent,
        PointVsCircle,
    }
}