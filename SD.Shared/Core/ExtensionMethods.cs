using System.Text;

namespace SD.Shared.Core
{
    public static class ExtensionMethods
    {
        public static bool Empty<TSource>(this IEnumerable<TSource> source)
        {
            return !source.Any();
        }

        public static bool Empty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.Any(predicate);
        }

        public static string SimpleEncrypt(this string? url)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(url ?? ""));
        }

        public static string SimpleDecrypt(this string? obfuscatedUrl)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(obfuscatedUrl ?? ""));
        }
    }
}