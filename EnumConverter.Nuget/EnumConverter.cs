using System;

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

            return Convert.ToInt32(value);
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
            if (validateDefinition && !Enum.IsDefined(typeof(T), value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Value '{value}' is not defined for enum '{typeof(T).Name}'.");
            }

            return (T)Enum.ToObject(typeof(T), value);
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
            if (Enum.IsDefined(typeof(T), value))
            {
                result = (T)Enum.ToObject(typeof(T), value);
                return true;
            }

            result = default;
            return false;
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

            if (Enum.TryParse(value, ignoreCase, out T result))
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
            if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, ignoreCase, out result))
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
            return TryToEnum(value, out T result, ignoreCase) ? result : null;
        }
    }
}
