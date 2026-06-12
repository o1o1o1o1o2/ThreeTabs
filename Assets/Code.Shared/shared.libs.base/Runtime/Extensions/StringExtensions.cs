using System.Collections.Generic;
using System.Linq;

namespace Shared.Libs.Extensions
{
    public static class StringExtensions
    {
        public static string JoinToString<T>(this IEnumerable<T> array, string separator = ",") =>
            array != null ? string.Join(separator, array.Select(x => x?.ToString())) : string.Empty;
    }
}