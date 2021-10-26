using System.Drawing;
using FallingBlocks.Engine.Core.Core.Propertyaccess;

namespace FallingBlocks.Engine.Core.Core.Behavior
{
    /// <summary>
    /// Extention methods for manipulating render objects animated.
    /// </summary>
    public static class MemberChange
    {
        public static MemberAnimatorAbsolute<T> Change<T>(this PropertyAccess<ARenderObject, T> propertyAccess,
            T newValue, long durationMs)
        {
            return new MemberAnimatorAbsolute<T>(propertyAccess, newValue, durationMs);
        }

        public static MemberChangeRelative<T> ChangeRelativeDirect<T>(
            this PropertyAccess<ARenderObject, T> propertyAccess, T deltaValue)
        {
            return new MemberChangeRelative<T>(propertyAccess, deltaValue);
        }

        /// <summary>
        /// Change the opcacity of a render object animated.
        /// </summary>
        public static IObjectWithBehavior ChangeOpacity(this IObjectWithBehavior ro, float newOpacity, long durationMs)
        {
            return ro.AddBehavior(ARenderObject.OpacityAccess.Change(newOpacity, durationMs));
        }

        /// <summary>
        /// Change the scale of a render object animated.
        /// </summary>
        public static IObjectWithBehavior ChangeScale(this IObjectWithBehavior ro, float newScale, long durationMs)
        {
            return ro.AddBehavior(ARenderObject.ScaleAccess.Change(newScale, durationMs));
        }

        /// <summary>
        /// Change the rotation of a render object animated.
        /// </summary>
        public static IObjectWithBehavior ChangeRotation(this IObjectWithBehavior ro, float newRotations,
            long durationMs)
        {
            return ro.AddBehavior(ARenderObject.RotationAccess.Change(newRotations, durationMs));
        }

        /// <summary>
        /// Change the rotation of a render object animated.
        /// </summary>
        public static IObjectWithBehavior Move(this IObjectWithBehavior ro, PointF deltaPos)
        {
            return ro.AddBehavior(ARenderObject.PositionAccess.ChangeRelativeDirect(deltaPos));
        }

        /// <summary>
        /// Change the rotation of a render object animated.
        /// </summary>
        public static IObjectWithBehavior ChangeSpeedRelative(this IObjectWithBehavior ro, PointF deltaPos)
        {
            return ro.AddBehavior(ARenderObject.SpeedAccess.ChangeRelativeDirect(deltaPos));
        }

        /// <summary>
        /// When cascading anomations, it is handy to specify a break between.
        /// Remark: This of course not a real waiting, it simply delays the execution of the next animation.
        /// </summary>
        public static IObjectWithBehavior Wait(this IObjectWithBehavior ro, long durationMs)
        {
            return ro.AddBehavior(new WaitBehavior(durationMs));
        }

        /// <summary>
        /// Change the rotation speed of a render object animated.
        /// </summary>
        public static IObjectWithBehavior ChangeRotationSpeed(this IObjectWithBehavior ro, float newSpeed,
            long durationMs)
        {
            return ro.AddBehavior(ARenderObject.RotationSpeedAccess.Change(newSpeed, durationMs));
        }

        /// <summary>
        /// Change the speed of a render object animated.
        /// </summary>
        public static IObjectWithBehavior ChangeSpeed(this IObjectWithBehavior ro, PointF newSpeed, long durationMs)
        {
            return ro.AddBehavior(ARenderObject.SpeedAccess.Change(newSpeed, durationMs));
        }


        /// <summary>
        /// Change the speed of a render object animated.
        /// </summary>
        public static IObjectWithBehavior ChangePosition(this IObjectWithBehavior ro, PointF newPos, long durationMs)
        {
            return ro.AddBehavior(ARenderObject.PositionAccess.Change(newPos, durationMs));
        }
    }
}