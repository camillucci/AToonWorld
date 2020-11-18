using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class IEnumerableExtensions
    {
        public static T WithMinOrDefault<T, V>(this IEnumerable<T> enumerable, Func<T, V> predicate) where V : IComparable<V>
        {
            var enumerator = enumerable.GetEnumerator();            
            if (!enumerator.MoveNext())
                return default;

            var minObj = enumerator.Current;
            var min = predicate(minObj);

            while(enumerator.MoveNext())
            {
                var obj = enumerator.Current;
                var val = predicate(obj);
                if (val.CompareTo(min) == -1)
                    (minObj, min) = (obj, val);
            }

            return minObj;
        }
    }
}
