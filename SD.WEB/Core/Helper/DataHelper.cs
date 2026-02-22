namespace SD.WEB.Core.Helper;

public static class DataHelper
{
    public static string GetResume(this string? text, int count)
    {
        if (string.IsNullOrEmpty(text)) return "";

        return text.Length > count ? string.Concat(text.AsSpan(0, count), "...") : text;
    }
}
