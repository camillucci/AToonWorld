using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class IListExtensions
    {
        public static IEnumerable<T> ReverseAsEnumerable<T>(this IList<T> items)
        {
            for (int i = items.Count - 1; i >= 0; i--)
                yield return items[i];
        }
    }
}
