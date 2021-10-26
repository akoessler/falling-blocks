using FallingBlocks.Engine.Core.Core.Propertyaccess;
using FallingBlocks.Engine.Core.Util;

namespace FallingBlocks.Engine.Core.Core.Behavior
{
    /// <summary>
    /// Changes a member of a render object to an absolute end value.
    /// The change is animated.
    /// </summary>
    public class MemberAnimatorAbsolute<TValue> : ABehavior
    {
        PropertyAccess<ARenderObject, TValue> propertyAccess;
        private TValue targetValue;
        private long durationMs;
        private TValue startValue;

        public MemberAnimatorAbsolute(PropertyAccess<ARenderObject, TValue> propertyAccess, TValue targetValue,
            long durationMs)
        {
            this.propertyAccess = propertyAccess;
            this.targetValue = targetValue;
            this.targetValue = targetValue;
            this.durationMs = durationMs;
        }


        /*
        public static MemberAnimatorAbsolute<float> changeRotation(float newValue, long durationMs)
        {
            return new MemberAnimatorAbsolute<float>(ARenderObject.RotationAccess, newValue, durationMs);
        }
        public static MemberAnimatorAbsolute changeOpacity(float newValue, long durationMs)
        {
            return new MemberAnimatorAbsolute(ro => ro.Opacity, (ro, val) =>ro.Opacity = val, newValue, durationMs);
        }
        public static MemberAnimatorAbsolute changeScale(float newValue, long durationMs)
        {
            return new MemberAnimatorAbsolute(ro => ro.Scale, (ro, val) => ro.Scale = val, newValue, durationMs);
        }
        public static MemberAnimatorAbsolute changePositionX(float newValue, long durationMs)
        {
            return new MemberAnimatorAbsolute(ro => ro.Position.X, (ro, val) => ro.Position = new PointF(val, ro.Position.Y), newValue, durationMs);
        }
        public static MemberAnimatorAbsolute changePositionY(float newValue, long durationMs)
        {
            return new MemberAnimatorAbsolute(ro => ro.Position.Y, (ro, val) => ro.Position = new PointF(ro.Position.X, val), newValue, durationMs);
        }*/


        protected override void OnStart(long timestamp, ARenderObject ro)
        {
            base.OnStart(timestamp, ro);
            this.startValue = this.propertyAccess.GetValue(ro);
        }

        protected override bool UpdateOjectInternal(long timestamp, ARenderObject ro)
        {
            float relativeProgress = getRelativeProgress(timestamp, this.durationMs);
            relativeProgress = (float) CosinusProgressConverter.Convert(relativeProgress);

            TValue diffFromTarget = this.propertyAccess.Difference(this.targetValue, this.startValue);

            var value = this.propertyAccess.Sum(this.startValue,
                this.propertyAccess.Scale(diffFromTarget, relativeProgress));
            this.propertyAccess.SetValue(ro, value);

            // If finished we remove it.
            return relativeProgress >= 1.0f;
        }
    }
}