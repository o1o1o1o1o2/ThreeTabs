using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Shared.Libs.Utils
{
    [SuppressMessage("ReSharper", "UnusedType.Global"), SuppressMessage("ReSharper", "CheckNamespace")]
    public static class Enums
    {
        [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AsInt<TEnum>(this TEnum value) where TEnum : unmanaged, Enum => ToIntegral<TEnum, int>(value);

        [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long AsLong<TEnum>(this TEnum value) where TEnum : unmanaged, Enum => ToIntegral<TEnum, long>(value);

        [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ToEnum<TEnum>(this int value) where TEnum : unmanaged, Enum => ToEnum<TEnum, int>(value);

        [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum ToEnum<TEnum>(this long value) where TEnum : unmanaged, Enum => ToEnum<TEnum, long>(value);

        /// <summary>
        ///     convert enum to integer value
        /// </summary>
        [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe TValue ToIntegral<TEnum, TValue>(TEnum value)
            where TEnum : unmanaged, Enum
            where TValue : unmanaged
        {
            if (sizeof(TValue) > sizeof(TEnum))
            {
                TValue o = default;
                *(TEnum*)&o = value;
                return o;
            }

            return *(TValue*)&value;
        }

        [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe TEnum ToEnum<TEnum, TValue>(TValue value)
            where TEnum : unmanaged, Enum
            where TValue : unmanaged
        {
            if (sizeof(TEnum) > sizeof(TValue))
            {
                TEnum o = default;
                *(TValue*)&o = value;
                return o;
            }

            return *(TEnum*)&value;
        }

        /// <summary>
        ///     return all items of an enum
        /// </summary>
        [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false), MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Values<T>() where T : unmanaged, Enum
        {
            if (!TypeOf<T>.Raw.IsEnum)
                throw new ArgumentException($"T must be an enumerated type but found {TypeOf<T>.Raw.FullName}");

            return (T[])Enum.GetValues(TypeOf<T>.Raw);
        }
    }
}