using System;
using System.Collections.Generic;
using System.Linq;

namespace Skybrud.Umbraco.Feedback.Extensions {

    public static class EnumerableExtensionMethods {

        public static IOrderedEnumerable<T> OrderBy<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> func, bool reverse) {
            return reverse ? collection.OrderByDescending(func) : collection.OrderBy(func);
        }

    }

}