using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class IReadOnlyListExtensions
    {
        private static readonly Random _random = new Random();
        public static T Random<T>(this IReadOnlyList<T> @this)
        {
            var index = _random.Next(@this.Count);
            return @this[index];
        }
    }
}
