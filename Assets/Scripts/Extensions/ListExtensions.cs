using System;
using System.Collections.Generic;

namespace SmashBall.Extensions
{
    public static class ListExtensions
    {
        private static readonly Random rng = new();

        public static T PopRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Cannot pop from an empty list.");

            int index = rng.Next(list.Count);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Cannot get a random element from an empty list.");

            return list[rng.Next(list.Count)];
        }
    }
}