using System;

namespace FallingBlocks.Engine.Core.Util
{
    public class CosinusProgressConverter
    {
        public static double Convert(double t)
        {
            double phi = (-Math.PI) + (Math.PI * t * 1.0);
            return (Math.Cos(phi) + 1.0) / 2.0;
        }
    }
}