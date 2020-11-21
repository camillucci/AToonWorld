using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class IEnumerableExtensions
    {
        public static T WithMinOrDefault<T, V>(this IEnumerable<T> @this, Func<T, V> toComparableField) where V : IComparable<V>
            => @this.MinimumPoint((t1, t2) => toComparableField(t1).CompareTo(toComparableField(t2)) == -1);
        
        public static T MinimumPoint<T>(this IEnumerable<T> @this, Func<T, T, bool> isLessThan)
        {
            var enumerator = @this.GetEnumerator();
            if (!enumerator.MoveNext())
                return default;

            var minObj = enumerator.Current;

            while (enumerator.MoveNext())
            {
                var obj = enumerator.Current;
                if (isLessThan(obj, minObj))
                    minObj = obj;
            }

            return minObj;
        }        
    }
}
