using System;

namespace WIM.Extensions
{
        //Extension methods must be defined in a static class
        public static class StringExtensions
        {
            // This is the extension method.
            // The first parameter takes the "this" modifier
            // and specifies the type for which the method is defined.
            public static string TrimToUpper(this string str)
            {
                return str.ToUpper().Trim();

            }
        public static string TrimToLower(this string str)
        {
            return str.ToLower().Trim();

        }

        public static bool IgnoreCaseEquals(this string str, string str2)
            {
                return String.Equals(str.TrimToUpper(), str2.TrimToUpper());
            }
        }
}
