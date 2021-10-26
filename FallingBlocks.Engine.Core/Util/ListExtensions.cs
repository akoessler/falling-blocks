﻿using System;
using System.Collections.Generic;

namespace FallingBlocks.Engine.Core.Util
{
    public static class ListExtensions
    {
        private static readonly Random Random = new Random(Environment.TickCount);

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}