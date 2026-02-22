using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Common.Library.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Flat<T>(this IEnumerable<IEnumerable<T>> source)
        {
            foreach (var children in source)
            {
                if (children == null) continue;

                foreach (var child in children)
                {
                    yield return child;
                }
            }
        }
    }
}
