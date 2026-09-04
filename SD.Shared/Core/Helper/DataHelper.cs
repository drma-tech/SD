using System.Globalization;

namespace SD.Shared.Core.Helper;

public static class DataHelper
{
    public static string GetResume(this string? text, int count)
    {
        if (string.IsNullOrEmpty(text)) return "";

        return text.Length > count ? string.Concat(text.AsSpan(0, count), "...") : text;
    }

    public static string FormatRuntime(this int? runtime)
    {
        if (!runtime.HasValue || runtime == 0) return "";
        var time = TimeSpan.FromMinutes(runtime.Value);
        return string.Create(CultureInfo.InvariantCulture, $"{time.Hours}h {time.Minutes}m");
    }
}