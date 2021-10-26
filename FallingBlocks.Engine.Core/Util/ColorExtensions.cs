using System;
using System.Drawing;

namespace FallingBlocks.Engine.Core.Util
{
    public static class ColorExtensions
    {
        public static Color AdjustAlpha(this Color src, float factor)
        {
            float a = Math.Min(src.A * factor, 255);
            return Color.FromArgb((int) a, src.R, src.G, src.B);
        }
    }
}