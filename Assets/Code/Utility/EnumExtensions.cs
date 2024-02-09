using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Assets.Code.Utility
{
    public static class EnumExtensions
    {
        public static string ToFlagString<T>(this T value) where T : Enum
        {
            var flags = Enum.GetValues(typeof(T)).Cast<T>();
            return string.Join(", ", flags.Where(x => value.HasFlag(x)));
        }

        public static string ToFlagString<T>(this T value, IEnumerable<T> except) where T : Enum
        {
            var flags = Enum.GetValues(typeof(T)).Cast<T>().Except(except);
            return string.Join(", ", flags.Where(x => value.HasFlag(x)));
        }

        public static IEnumerable<T> GetFlags<T>(this T value) where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Where(x => value.HasFlag(x));
    }
}
