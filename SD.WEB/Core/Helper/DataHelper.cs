using System.Globalization;

namespace SD.WEB.Core.Helper;

public static class DataHelper
{
    public static string GetResume(this string? text, int count)
    {
        if (string.IsNullOrEmpty(text)) return "";

        return text.Length > count ? string.Concat(text.AsSpan(0, count), "...") : text;
    }

    public static DateTime? GetDate(this string? value)
    {
        if (!string.IsNullOrEmpty(value) && DateTime.TryParse(value, out _))
            return DateTime.Parse(value, CultureInfo.CurrentCulture);
        return null;
    }

    public static string FormatRuntime(this int? runtime)
    {
        if (!runtime.HasValue || runtime == 0) return "";
        var time = TimeSpan.FromMinutes(runtime.Value);
        return $"{time.Hours}h {time.Minutes}m";
    }
}