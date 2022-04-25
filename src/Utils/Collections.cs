using System;
using System.Collections.Generic;

namespace ActionEffectRange.Utils
{
    public static class Collections
    {
        public static List<R> FlatMap<T, R>(
            this IEnumerable<T> s, Func<T, IEnumerable<R>?> map)
        {
            var s1 = new List<R>();
            foreach (var item in s)
            {
                var m = map(item);
                if (m != null) s1.AddRange(m);
            }
            return s1;
        }

        public static void AddAllMappedNotNull<T, R>(
            this ICollection<T> s, IEnumerable<R?> items, Func<R?, T?> map)
        {
            foreach (var item in items)
                if (item != null)
                {
                    var m = map(item);
                    if (m != null) s.Add(m);
                }
        }

    }
}
