using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

namespace FallingBlocks.Engine.Core.Util
{
    /// <summary>
    /// Helpers for the PointF type, e.g. Vector math
    /// </summary>
    public static class PointfExtensions
    {
        public static float Wrap(this float f, float minValue, float maxValue)
        {
            return (f < minValue) ? maxValue
                : (f > maxValue) ? minValue
                : f;
        }

        /// <summary>
        /// Wraps a given in a given rectangle, e.g. if it exceeds the 
        /// rectangle's right bound, it reenters at the rectangle's left
        /// side.
        /// </summary>
        public static PointF Wrap(this PointF p, RectangleF rect)
        {
            float x = p.X.Wrap(rect.Left, rect.Right);
            float y = p.Y.Wrap(rect.Top, rect.Bottom);
            return new PointF(x, y);
        }

        /// <summary>
        /// Returns the distance of the given point from the origin (0,0)
        /// </summary>
        public static double Length(this PointF p)
        {
            return Math.Sqrt(p.X * p.X + p.Y * p.Y);
        }

        /// <summary>
        /// Returns the distance of the two given points
        /// </summary>
        public static double Distance(this PointF p, PointF q)
        {
            float distX = p.X - q.X;
            float distY = p.Y - q.Y;
            return Math.Sqrt(distX * distX + distY * distY);
        }

        /// <summary>
        /// Multiplies each component of the point with the given factor
        /// </summary>
        public static PointF Multiply(this PointF p, float factor)
        {
            return new PointF(p.X * factor, p.Y * factor);
        }

        /// <summary>
        /// Adds 2 points together
        /// </summary>
        public static PointF Add(this PointF p, PointF q)
        {
            return new PointF(p.X + q.X, p.Y + q.Y);
        }

        /// <summary>
        /// Adds the x and y value to the point
        /// </summary>
        public static PointF Add(this PointF p1, float x, float y)
        {
            return new PointF(p1.X + x, p1.Y + y);
        }

        /// <summary>
        /// Returns the angle of the given vector, in degrees.
        /// Up = 0°, Right = 90°, Down = 180°, Left = 270°
        /// </summary>
        public static double Angle(this PointF p)
        {
            double angleRadians = Math.Atan2(p.Y, p.X);
            return 90 + angleRadians * 180 / Math.PI;
        }

        /// <summary>
        /// Returns the value of the description attribute on an enum value, if it exists
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                        Attribute.GetCustomAttribute(field,
                            typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }

            return null;
        }
    }
}