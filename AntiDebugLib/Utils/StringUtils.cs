using System;
using System.Text;

namespace AntiDebugLib.Utils
{
    internal static class StringUtils
    {
        public const string LowerAlpha = "abcdefghijklmnopqrstuvwxyz";
        public const string UpperAlpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string Numeric = "0123456789";
        public const string Special = "!@#$%^&*()-=_+[]{}\\|;':\",./<>?";

        public const string MixedAlphaNumeric = LowerAlpha + UpperAlpha + Numeric;
        public const string MixedAlphaNumericSpecial = LowerAlpha + UpperAlpha + Numeric + Special;

        public static string RandomString(int length, Random random, string dictionary = MixedAlphaNumeric)
        {
            var builder = new StringBuilder(length);
            random = random ?? new Random();
            for (var i = 0; i < length; i++)
                builder.Append(dictionary[random.Next(dictionary.Length)]);

            return builder.ToString();
        }
    }
}
