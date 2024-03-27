using System.Text;
using System.Text.RegularExpressions;

namespace SD.Shared.Core.Helper
{
    public static partial class StringHelper
    {
        public static string Format(this string format, object? arg0)
        {
            return string.Format(format, arg0);
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            return RemoveSpecialCharacters(str.AsSpan()).ToString();
        }

        public static ReadOnlySpan<char> RemoveSpecialCharacters(this ReadOnlySpan<char> str)
        {
            Span<char> buffer = new char[str.Length];
            int idx = 0;

            foreach (char c in str)
            {
                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                {
                    buffer[idx] = c;
                    idx++;
                }
            }

            return buffer[..idx];
        }

        [GeneratedRegex(@"\p{Mn}", RegexOptions.Compiled)]
        private static partial Regex DiacriticsRegex();

        public static string RemoveDiacritics(this string Text)
        {
            return DiacriticsRegex().Replace(Text.Normalize(NormalizationForm.FormD), string.Empty);
        }
    }
}