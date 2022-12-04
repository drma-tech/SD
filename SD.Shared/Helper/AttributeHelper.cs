using System.Globalization;

namespace SD.Shared.Helper
{
    public static class AttributeHelper
    {
        public static DateTime? GetDate(this string? value)
        {
            if (string.IsNullOrEmpty(value))
                return DateTime.MaxValue;
            else if (!DateTime.TryParse(value, out _))
                return DateTime.MinValue;
            else
                return DateTime.Parse(value, CultureInfo.CurrentCulture);
        }

        public static string FormatRuntime(this int? runtime)
        {
            if (!runtime.HasValue || runtime == 0) return "";
            var time = TimeSpan.FromMinutes(runtime.Value);
            return $"{time.Hours}h {time.Minutes}m";
        }
    }
}