using System;

namespace EnumConverter.Nuget
{
    public static class EnumConverter
    {
        /// <summary>
        /// Returns the int value of an enum equivalent.
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static int ToInt(this Enum en)
        {
            return Convert.ToInt32(en);
        }
        /// <summary>
        /// Returns the enum equivalent of an int value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int id) where T : Enum
        {
            T foo = (T)Enum.ToObject(typeof(T), id);
            return foo;
        }
    }
}