using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeCommerce.Application.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ThrowIfEmpty<T>(
            this IEnumerable<T>? source,
            Func<Exception> exceptionFactory)
        {
            if (source is null || !source.Any())
                throw exceptionFactory();

            return source;
        }
    }
}
