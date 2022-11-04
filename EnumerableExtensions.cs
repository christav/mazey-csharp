using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mazey
{
    static class EnumerableExtensions
    {
        public static void ForEach<TItem>(this IEnumerable<TItem> seq, Action<TItem> action)
        {
            foreach (var item in seq)
            {
                action(item);
            }
        }

        public static void ForEach<TItem>(this IEnumerable<TItem> seq, Action<TItem, int> action)
        {
            int i = 0;
            foreach (var item in seq)
            {
                action(item, i);
                ++i;
            }
        }
    }
}
