namespace FallingBlocks.Engine.Core.Util
{
    /// <summary>
    /// Helper for float operations.
    /// </summary>
    public class FloatHelper
    {
        /// <summary>
        /// Helper for checking some of our float value for being equal.
        /// </summary>
        public static bool FloatEquals(float f1, float f2)
        {
            float diff = f1 - f2;
            if (diff < 0)
            {
                diff = diff * -1;
            }

            return diff < 0.0001;
        }
    }
}