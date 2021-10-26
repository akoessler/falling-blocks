using FallingBlocks.Engine.Core.Core.Propertyaccess;

namespace FallingBlocks.Engine.Core.Core.Behavior
{
    /// <summary>
    /// Changes a member of a render object to an absolute end value.
    /// The change is animated.
    /// </summary>
    public class MemberChangeRelative<TValue> : ABehavior
    {
        PropertyAccess<ARenderObject, TValue> propertyAccess;
        private TValue deltaValue;

        public MemberChangeRelative(PropertyAccess<ARenderObject, TValue> propertyAccess, TValue deltaValue)
        {
            this.propertyAccess = propertyAccess;
            this.deltaValue = deltaValue;
        }

        protected override bool UpdateOjectInternal(long timestamp, ARenderObject ro)
        {
            TValue curValue = propertyAccess.GetValue(ro);
            TValue newValue = propertyAccess.Sum(curValue, deltaValue);
            propertyAccess.SetValue(ro, newValue);
            return true;
        }
    }
}