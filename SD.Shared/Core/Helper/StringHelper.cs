using System.Text;
using System.Text.RegularExpressions;

namespace SD.Shared.Core.Helper;

public static partial class StringHelper
{
    public static string Format(this string format, object? arg0)
    {
        return string.Format(format, arg0);
    }

    public static string RemoveSpecialCharacters(this string str, char[]? customExceptions = null, char? replace = null)
    {
        return RemoveSpecialCharacters(str.AsSpan(), customExceptions, replace).ToString();
    }

    public static ReadOnlySpan<char> RemoveSpecialCharacters(this ReadOnlySpan<char> str,
        char[]? customExceptions = null, char? replace = null)
    {
        Span<char> buffer = new char[str.Length];
        var idx = 0;
        char[] exceptions = ['-'];

        if (customExceptions != null) exceptions = exceptions.Union(customExceptions).ToArray();

        foreach (var c in str)
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || Array.Exists(exceptions, match => match == c))
            {
                buffer[idx] = c;
                idx++;
            }
            else if (replace != null)
            {
                buffer[idx] = replace.Value;
                idx++;
            }

        return buffer[..idx];
    }

    [GeneratedRegex(@"\p{Mn}", RegexOptions.Compiled)]
    private static partial Regex DiacriticsRegex();

    public static string RemoveDiacritics(this string Text)
    {
        return DiacriticsRegex().Replace(Text.Normalize(NormalizationForm.FormD), string.Empty);
    }

    public static string? ToSlug(this string? str)
    {
        if (str == null) return null;

        str = str.ToLowerInvariant();
        str = str.RemoveDiacritics();
        str = str.RemoveSpecialCharacters();

        str = Regex.Replace(str, @"\s+", "-", RegexOptions.NonBacktracking); // Replace spaces with hyphens
        str = Regex.Replace(str, @"-+", "-", RegexOptions.NonBacktracking); // Replace multiple hyphens with a single one
        str = str.Trim('-'); // Trim leading and trailing hyphens

        return str;
    }

    public static DateTimeOffset ParseAppleDate(this string appleDate)
    {
        var parts = appleDate.Split(' ');
        if (parts.Length < 3)
            return DateTimeOffset.Parse(appleDate);

        var datePart = $"{parts[0]} {parts[1]}";
        var tzPart = parts[2];

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "America/Los_Angeles", "Pacific Standard Time" },
            { "America/New_York", "Eastern Standard Time" },
            { "Europe/London", "GMT Standard Time" },
            { "Asia/Bangkok", "SE Asia Standard Time" },
            { "Etc/GMT", "GMT Standard Time" }
        };

        if (!map.TryGetValue(tzPart, out var winTz))
            winTz = "UTC"; //fallback

        var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(winTz);
        var localTime = DateTime.Parse(datePart);
        var offset = tzInfo.GetUtcOffset(localTime);

        return new DateTimeOffset(localTime, offset);
    }
}