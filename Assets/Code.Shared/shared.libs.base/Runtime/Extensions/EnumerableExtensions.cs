using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

[SuppressMessage("ReSharper", "InconsistentNaming"), SuppressMessage("ReSharper", "CheckNamespace")]
public static class EnumerableExtensions
{
    [ContractAnnotation("obj:null => true")]
    public static bool IsNullOrEmpty<K, T>(this Dictionary<K, T> obj) => obj == null || obj.Count == 0;

    [ContractAnnotation("obj:null => true")]
    public static bool IsNullOrEmpty<T>(this ICollection<T> obj) => obj == null || obj.Count == 0;

    [ContractAnnotation("obj:null => true")]
    public static bool IsNullOrEmpty<T>(this T[] obj) => obj == null || obj.Length == 0;

    [ContractAnnotation("obj:null => true")]
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> obj) => obj == null || !obj.Any();


}