using System;

namespace FallingBlocks.Engine.Core.Core.Propertyaccess
{
    public class PropertyAccessFloat<TObject> : PropertyAccess<TObject, float>
    {
        /// <summary>
        /// ctor.
        /// </summary>
        public PropertyAccessFloat(Func<TObject, float> getter, Action<TObject, float> setter) : base(getter, setter)
        {
        }

        /// <inheritdoc/>
        public override float Scale(float value, float scale)
        {
            return value * scale;
        }

        /// <inheritdoc/>
        public override float Sum(float v1, float v2)
        {
            return v1 + v2;
        }
    }
}