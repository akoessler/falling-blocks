using System;
using System.Drawing;

namespace FallingBlocks.Engine.Core.Core.Propertyaccess
{
    public class PropertyAccessPointF<TObject> : PropertyAccess<TObject, PointF>
    {
        /// <summary>
        /// ctor.
        /// </summary>
        public PropertyAccessPointF(Func<TObject, PointF> getter, Action<TObject, PointF> setter)
            : base(getter, setter)
        {
        }

        /// <inheritdoc/>
        public override PointF Scale(PointF value, float scale)
        {
            return new PointF(value.X * scale, value.Y * scale);
        }

        /// <inheritdoc/>
        public override PointF Sum(PointF v1, PointF v2)
        {
            return new PointF(v1.X + v2.X, v1.Y + v2.Y);
        }
    }
}