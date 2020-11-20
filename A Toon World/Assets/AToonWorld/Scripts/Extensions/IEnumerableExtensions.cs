using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class IEnumerableExtensions
    {
        public static T WithMinOrDefault<T, V>(this IEnumerable<T> enumerable, Func<T, V> valueToMinimizeFunction) where V : IComparable<V>
            => enumerable.MinimumPointOrDefault((t1, t2) => valueToMinimizeFunction(t1).CompareTo(valueToMinimizeFunction(t2)) == -1);

        public static T MinimumPointOrDefault<T> (this IEnumerable<T> enumerable, Func<T,T, bool> lessThanPredicate)
        {
            var enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext())
                return default;

            var minObj = enumerator.Current;            

            while (enumerator.MoveNext())
            {
                var obj = enumerator.Current;
                if (lessThanPredicate(obj, minObj))
                    minObj = obj;
            }

            return minObj;
        }       
    }
}
