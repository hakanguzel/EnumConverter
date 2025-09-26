using System;

using RootEnumConverter = global::EnumConverter.EnumConverter;

namespace EnumConverter.Nuget
{
    /// <summary>
    /// Backwards compatible facade to keep the historic namespace <c>EnumConverter.Nuget</c> working.
    /// </summary>
    public static class EnumConverter
    {
        public static int ToInt(this Enum value) => RootEnumConverter.ToInt(value);

        public static TUnderlying ToValue<TUnderlying>(this Enum value) where TUnderlying : struct
            => RootEnumConverter.ToValue<TUnderlying>(value);

        public static TUnderlying ToValue<TEnum, TUnderlying>(this TEnum value)
            where TEnum : struct, Enum
            where TUnderlying : struct
            => RootEnumConverter.ToValue<TEnum, TUnderlying>(value);

        public static T ToEnum<T>(this int value, bool validateDefinition = false) where T : struct, Enum
            => RootEnumConverter.ToEnum<T>(value, validateDefinition);

        public static TEnum ToEnum<TEnum, TNumber>(this TNumber value, bool validateDefinition = false)
            where TEnum : struct, Enum
            where TNumber : struct, IConvertible
            => RootEnumConverter.ToEnum<TEnum, TNumber>(value, validateDefinition);

        public static T? ToEnum<T>(this int? value, bool validateDefinition = false) where T : struct, Enum
            => RootEnumConverter.ToEnum<T>(value, validateDefinition);

        public static bool TryToEnum<T>(this int value, out T result) where T : struct, Enum
            => RootEnumConverter.TryToEnum(value, out result);

        public static bool TryToEnum<T>(this int? value, out T result) where T : struct, Enum
            => RootEnumConverter.TryToEnum(value, out result);

        public static bool TryToEnum<TEnum, TNumber>(this TNumber value, out TEnum result)
            where TEnum : struct, Enum
            where TNumber : struct, IConvertible
            => RootEnumConverter.TryToEnum(value, out result);

        public static T ToEnum<T>(this string value, bool ignoreCase = true) where T : struct, Enum
            => RootEnumConverter.ToEnum<T>(value, ignoreCase);

        public static T ToEnum<T>(this string value, StringComparison comparisonType) where T : struct, Enum
            => RootEnumConverter.ToEnum<T>(value, comparisonType);

        public static bool TryToEnum<T>(this string? value, out T result, bool ignoreCase = true) where T : struct, Enum
            => RootEnumConverter.TryToEnum(value, out result, ignoreCase);

        public static bool TryToEnum<T>(this string? value, out T result, StringComparison comparisonType) where T : struct, Enum
            => RootEnumConverter.TryToEnum(value, out result, comparisonType);

        public static T? ToNullableEnum<T>(this string? value, bool ignoreCase = true) where T : struct, Enum
            => RootEnumConverter.ToNullableEnum<T>(value, ignoreCase);

        public static T? ToNullableEnum<T>(this string? value, StringComparison comparisonType) where T : struct, Enum
            => RootEnumConverter.ToNullableEnum<T>(value, comparisonType);

        public static T ToEnum<T>(this ReadOnlySpan<char> value, bool ignoreCase = true) where T : struct, Enum
            => RootEnumConverter.ToEnum<T>(value, ignoreCase);

        public static bool TryToEnum<T>(this ReadOnlySpan<char> value, out T result, bool ignoreCase = true) where T : struct, Enum
            => RootEnumConverter.TryToEnum(value, out result, ignoreCase);

        public static T? ToNullableEnum<T>(this ReadOnlySpan<char> value, bool ignoreCase = true) where T : struct, Enum
            => RootEnumConverter.ToNullableEnum<T>(value, ignoreCase);
    }
}
