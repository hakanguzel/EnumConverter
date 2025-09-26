using System;
using System.Collections.Generic;
using System.Globalization;

namespace EnumConverter.Nuget
{
    public static class EnumConverter
    {
        /// <summary>
        /// Returns the numeric representation of the provided enum value.
        /// </summary>
        /// <param name="value">Enum value to convert.</param>
        /// <returns>32-bit integer representation of the enum value.</returns>
        public static int ToInt(this Enum value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return value.ToValue<int>();
        }

        /// <summary>
        /// Returns the underlying value for the provided enum instance.
        /// </summary>
        /// <typeparam name="TUnderlying">Expected underlying numeric type.</typeparam>
        /// <param name="value">Enum value to convert.</param>
        public static TUnderlying ToValue<TUnderlying>(this Enum value) where TUnderlying : struct
        {
            ArgumentNullException.ThrowIfNull(value);

            var enumType = value.GetType();
            var underlyingType = Enum.GetUnderlyingType(enumType);
            if (underlyingType != typeof(TUnderlying))
            {
                throw new InvalidOperationException($"Enum '{enumType.Name}' uses underlying type '{underlyingType.Name}', but '{typeof(TUnderlying).Name}' was requested.");
            }

            return (TUnderlying)Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture)!;
        }

        /// <summary>
        /// Returns the underlying value for the provided enum instance.
        /// </summary>
        /// <typeparam name="TEnum">Enum type.</typeparam>
        /// <typeparam name="TUnderlying">Expected underlying numeric type.</typeparam>
        /// <param name="value">Enum value to convert.</param>
        public static TUnderlying ToValue<TEnum, TUnderlying>(this TEnum value)
            where TEnum : struct, Enum
            where TUnderlying : struct
        {
            return ((Enum)(object)value).ToValue<TUnderlying>();
        }

        /// <summary>
        /// Converts a numeric value to its enum equivalent.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="validateDefinition">When true, throws if the numeric value is not defined for the enum type.</param>
        /// <returns>Enum value represented by the provided number.</returns>
        public static T ToEnum<T>(this int value, bool validateDefinition = false) where T : struct, Enum
        {
            return value.ToEnum<T, int>(validateDefinition);
        }

        /// <summary>
        /// Converts a numeric value to its enum equivalent.
        /// </summary>
        /// <typeparam name="TEnum">Target enum type.</typeparam>
        /// <typeparam name="TNumber">Numeric input type.</typeparam>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="validateDefinition">When true, throws if the numeric value is not defined for the enum type.</param>
        public static TEnum ToEnum<TEnum, TNumber>(this TNumber value, bool validateDefinition = false)
            where TEnum : struct, Enum
            where TNumber : struct, IConvertible
        {
            if (!TryConvertToUnderlyingValue<TEnum, TNumber>(value, out var underlyingValue, out var conversionError))
            {
                throw new ArgumentException(conversionError!, nameof(value));
            }

            if (validateDefinition && !IsValidEnumValue<TEnum>(underlyingValue))
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Value '{value}' is not defined for enum '{typeof(TEnum).Name}'.");
            }

            return (TEnum)Enum.ToObject(typeof(TEnum), underlyingValue);
        }

        /// <summary>
        /// Converts a nullable numeric value to its enum equivalent when a value is present.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">Nullable numeric value to convert.</param>
        /// <param name="validateDefinition">When true, throws if the numeric value is not defined for the enum type.</param>
        /// <returns>Enum value or null when no numeric value is provided.</returns>
        public static T? ToEnum<T>(this int? value, bool validateDefinition = false) where T : struct, Enum
        {
            return value is null ? null : value.Value.ToEnum<T>(validateDefinition);
        }

        /// <summary>
        /// Attempts to convert a numeric value to its enum equivalent.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="result">Resulting enum value when conversion succeeds.</param>
        /// <returns>True when the numeric value maps to a defined enum value.</returns>
        public static bool TryToEnum<T>(this int value, out T result) where T : struct, Enum
        {
            return value.TryToEnum<T, int>(out result);
        }

        /// <summary>
        /// Attempts to convert a nullable numeric value to its enum equivalent.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">Nullable numeric value.</param>
        /// <param name="result">Resulting enum value when conversion succeeds.</param>
        /// <returns>True when conversion succeeds; otherwise false.</returns>
        public static bool TryToEnum<T>(this int? value, out T result) where T : struct, Enum
        {
            if (value is null)
            {
                result = default;
                return false;
            }

            return value.Value.TryToEnum(out result);
        }

        /// <summary>
        /// Attempts to convert a numeric value to its enum equivalent.
        /// </summary>
        /// <typeparam name="TEnum">Target enum type.</typeparam>
        /// <typeparam name="TNumber">Numeric input type.</typeparam>
        /// <param name="value">Numeric value to convert.</param>
        /// <param name="result">Resulting enum value when conversion succeeds.</param>
        public static bool TryToEnum<TEnum, TNumber>(this TNumber value, out TEnum result)
            where TEnum : struct, Enum
            where TNumber : struct, IConvertible
        {
            if (!TryConvertToUnderlyingValue<TEnum, TNumber>(value, out var underlyingValue, out _))
            {
                result = default;
                return false;
            }

            if (!IsValidEnumValue<TEnum>(underlyingValue))
            {
                result = default;
                return false;
            }

            result = (TEnum)Enum.ToObject(typeof(TEnum), underlyingValue);
            return true;
        }

        /// <summary>
        /// Converts a string representation to its enum equivalent.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">String representation.</param>
        /// <param name="ignoreCase">True to ignore casing when parsing.</param>
        /// <returns>Enum value represented by the string.</returns>
        public static T ToEnum<T>(this string value, bool ignoreCase = true) where T : struct, Enum
        {
            ArgumentNullException.ThrowIfNull(value);

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return value.ToEnum<T>(comparison);
        }

        /// <summary>
        /// Converts a string representation to its enum equivalent using the specified comparison.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">String representation.</param>
        /// <param name="comparisonType">Comparison used when matching enum names.</param>
        /// <returns>Enum value represented by the string.</returns>
        public static T ToEnum<T>(this string value, StringComparison comparisonType) where T : struct, Enum
        {
            ArgumentNullException.ThrowIfNull(value);

            if (value.TryToEnum(out T result, comparisonType))
            {
                return result;
            }

            throw new ArgumentException($"'{value}' is not a valid {typeof(T).Name} value.", nameof(value));
        }

        /// <summary>
        /// Attempts to convert a string representation to its enum equivalent.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">String representation.</param>
        /// <param name="result">Resulting enum value when conversion succeeds.</param>
        /// <param name="ignoreCase">True to ignore casing when parsing.</param>
        /// <returns>True when the string maps to a defined enum value.</returns>
        public static bool TryToEnum<T>(this string? value, out T result, bool ignoreCase = true) where T : struct, Enum
        {
            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return value.TryToEnum(out result, comparison);
        }

        /// <summary>
        /// Attempts to convert a string representation to its enum equivalent using the specified comparison.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">String representation.</param>
        /// <param name="result">Resulting enum value when conversion succeeds.</param>
        /// <param name="comparisonType">Comparison used when matching enum names.</param>
        public static bool TryToEnum<T>(this string? value, out T result, StringComparison comparisonType) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result = default;
                return false;
            }

            var trimmed = value.Trim();

            if (TryParseUnderlyingValue<T>(trimmed, out var numericValue))
            {
                if (!IsValidEnumValue<T>(numericValue))
                {
                    result = default;
                    return false;
                }

                result = (T)Enum.ToObject(typeof(T), numericValue);
                return true;
            }

            if (TryParseByName(trimmed, comparisonType, out result))
            {
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Converts a string representation to its enum equivalent or returns null when parsing fails.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">String representation.</param>
        /// <param name="ignoreCase">True to ignore casing when parsing.</param>
        /// <returns>Enum value or null when conversion fails.</returns>
        public static T? ToNullableEnum<T>(this string? value, bool ignoreCase = true) where T : struct, Enum
        {
            return value.TryToEnum(out T result, ignoreCase) ? result : null;
        }

        /// <summary>
        /// Converts a string representation to its enum equivalent or returns null when parsing fails.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">String representation.</param>
        /// <param name="comparisonType">Comparison used when matching enum names.</param>
        public static T? ToNullableEnum<T>(this string? value, StringComparison comparisonType) where T : struct, Enum
        {
            return value.TryToEnum(out T result, comparisonType) ? result : null;
        }

        /// <summary>
        /// Converts a span representation to its enum equivalent.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">Span representation.</param>
        /// <param name="ignoreCase">True to ignore casing when parsing.</param>
        public static T ToEnum<T>(this ReadOnlySpan<char> value, bool ignoreCase = true) where T : struct, Enum
        {
            if (value.TryToEnum(out T result, ignoreCase))
            {
                return result;
            }

            throw new ArgumentException($"'{value.ToString()}' is not a valid {typeof(T).Name} value.", nameof(value));
        }

        /// <summary>
        /// Attempts to convert a span representation to its enum equivalent.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">Span representation.</param>
        /// <param name="result">Resulting enum value when conversion succeeds.</param>
        /// <param name="ignoreCase">True to ignore casing when parsing.</param>
        public static bool TryToEnum<T>(this ReadOnlySpan<char> value, out T result, bool ignoreCase = true) where T : struct, Enum
        {
            var trimmed = value.Trim();
            if (trimmed.IsEmpty)
            {
                result = default;
                return false;
            }

            if (!Enum.TryParse(trimmed, ignoreCase, out result))
            {
                result = default;
                return false;
            }

            var underlyingValue = GetUnderlyingValue(result);
            if (!IsValidEnumValue<T>(underlyingValue))
            {
                result = default;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Converts a span representation to its enum equivalent or returns null when parsing fails.
        /// </summary>
        /// <typeparam name="T">Enum type to convert to.</typeparam>
        /// <param name="value">Span representation.</param>
        /// <param name="ignoreCase">True to ignore casing when parsing.</param>
        public static T? ToNullableEnum<T>(this ReadOnlySpan<char> value, bool ignoreCase = true) where T : struct, Enum
        {
            return value.TryToEnum(out T result, ignoreCase) ? result : null;
        }

        private static bool TryConvertToUnderlyingValue<TEnum, TNumber>(TNumber value, out object underlyingValue, out string? error)
            where TEnum : struct, Enum
            where TNumber : struct, IConvertible
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
            try
            {
                underlyingValue = Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture)!;
                error = null;
                return true;
            }
            catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
            {
                underlyingValue = default!;
                error = $"Value '{value}' cannot be converted to underlying type '{underlyingType.Name}'.";
                return false;
            }
        }

        private static object GetUnderlyingValue<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
            return Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture)!;
        }

        private static bool IsValidEnumValue<TEnum>(object underlyingValue) where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            if (Enum.IsDefined(enumType, underlyingValue))
            {
                return true;
            }

            if (!enumType.IsDefined(typeof(FlagsAttribute), inherit: false))
            {
                return false;
            }

            var underlyingType = Enum.GetUnderlyingType(enumType);
            var valueBits = ToUInt64(underlyingValue, underlyingType);
            ulong definedBits = 0;
            foreach (var defined in Enum.GetValues<TEnum>())
            {
                var definedValue = Convert.ChangeType(defined, underlyingType, CultureInfo.InvariantCulture)!;
                definedBits |= ToUInt64(definedValue, underlyingType);
            }

            return (valueBits & ~definedBits) == 0;
        }

        private static ulong ToUInt64(object value, Type underlyingType)
        {
            return Type.GetTypeCode(underlyingType) switch
            {
                TypeCode.SByte => unchecked((ulong)(sbyte)value),
                TypeCode.Int16 => unchecked((ulong)(short)value),
                TypeCode.Int32 => unchecked((ulong)(int)value),
                TypeCode.Int64 => unchecked((ulong)(long)value),
                TypeCode.Byte => (byte)value,
                TypeCode.UInt16 => (ushort)value,
                TypeCode.UInt32 => (uint)value,
                TypeCode.UInt64 => (ulong)value,
                _ => throw new InvalidOperationException($"Unsupported enum underlying type '{underlyingType.FullName}'."),
            };
        }

        private static bool TryParseUnderlyingValue<TEnum>(string value, out object underlyingValue) where TEnum : struct, Enum
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
            var numberStyles = NumberStyles.Integer;
            var formatProvider = CultureInfo.InvariantCulture;

            switch (Type.GetTypeCode(underlyingType))
            {
                case TypeCode.SByte:
                    if (sbyte.TryParse(value, numberStyles, formatProvider, out var sbyteResult))
                    {
                        underlyingValue = sbyteResult;
                        return true;
                    }
                    break;
                case TypeCode.Int16:
                    if (short.TryParse(value, numberStyles, formatProvider, out var shortResult))
                    {
                        underlyingValue = shortResult;
                        return true;
                    }
                    break;
                case TypeCode.Int32:
                    if (int.TryParse(value, numberStyles, formatProvider, out var intResult))
                    {
                        underlyingValue = intResult;
                        return true;
                    }
                    break;
                case TypeCode.Int64:
                    if (long.TryParse(value, numberStyles, formatProvider, out var longResult))
                    {
                        underlyingValue = longResult;
                        return true;
                    }
                    break;
                case TypeCode.Byte:
                    if (byte.TryParse(value, numberStyles, formatProvider, out var byteResult))
                    {
                        underlyingValue = byteResult;
                        return true;
                    }
                    break;
                case TypeCode.UInt16:
                    if (ushort.TryParse(value, numberStyles, formatProvider, out var ushortResult))
                    {
                        underlyingValue = ushortResult;
                        return true;
                    }
                    break;
                case TypeCode.UInt32:
                    if (uint.TryParse(value, numberStyles, formatProvider, out var uintResult))
                    {
                        underlyingValue = uintResult;
                        return true;
                    }
                    break;
                case TypeCode.UInt64:
                    if (ulong.TryParse(value, numberStyles, formatProvider, out var ulongResult))
                    {
                        underlyingValue = ulongResult;
                        return true;
                    }
                    break;
            }

            underlyingValue = default!;
            return false;
        }

        private static bool TryParseByName<TEnum>(string value, StringComparison comparisonType, out TEnum result) where TEnum : struct, Enum
        {
            if (value.IndexOf(',') >= 0)
            {
                var parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var canonicalNames = new List<string>(parts.Length);
                foreach (var part in parts)
                {
                    if (!TryGetEnumName<TEnum>(part, comparisonType, out var canonicalName))
                    {
                        result = default;
                        return false;
                    }

                    canonicalNames.Add(canonicalName);
                }

                var canonicalValue = string.Join(", ", canonicalNames);
                if (Enum.TryParse(canonicalValue, out result))
                {
                    return true;
                }

                result = default;
                return false;
            }

            if (TryGetEnumName<TEnum>(value, comparisonType, out var name) && Enum.TryParse(name, out result))
            {
                return true;
            }

            result = default;
            return false;
        }

        private static bool TryGetEnumName<TEnum>(string candidate, StringComparison comparisonType, out string canonicalName) where TEnum : struct, Enum
        {
            foreach (var name in Enum.GetNames(typeof(TEnum)))
            {
                if (string.Equals(name, candidate, comparisonType))
                {
                    canonicalName = name;
                    return true;
                }
            }

            canonicalName = string.Empty;
            return false;
        }
    }
}
