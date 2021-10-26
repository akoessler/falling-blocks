using System;

namespace FallingBlocks.Engine.Core.Core.Propertyaccess
{
    public abstract class PropertyAccess<TObject, TValue>
    {
        private Func<TObject, TValue> getValue;
        private Action<TObject, TValue> setValue;


        public PropertyAccess(Func<TObject, TValue> getter, Action<TObject, TValue> setter)
        {
            this.getValue = getter;
            this.setValue = setter;
        }

        public TValue GetValue(TObject obj)
        {
            return this.getValue(obj);
        }

        public void SetValue(TObject obj, TValue value)
        {
            this.setValue(obj, value);
        }

        /// <summary>
        /// Multiply the value with the given scale.
        /// </summary>
        public abstract TValue Scale(TValue value, float scale);


        /// <summary>
        /// Creates a sum of 2 value. v1 + v2
        /// </summary>
        /// <returns></returns>
        public abstract TValue Sum(TValue v1, TValue v2);

        /// <summary>
        /// Calculates v1 - v2.
        /// </summary>
        /// <returns></returns>
        public virtual TValue Difference(TValue v1, TValue v2)
        {
            // By default difference can be achieved by building a sum and scaling the second value with -1.
            return this.Sum(v1, this.Scale(v2, -1.0f));
        }
    }
}