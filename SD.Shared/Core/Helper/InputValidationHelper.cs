using System.Globalization;
using System.Text.RegularExpressions;

namespace SD.Shared.Core.Helper;

/// <summary>
/// Provides reusable validation and normalization methods for untrusted string input.
/// </summary>
public static class InputValidationHelper
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Checks whether a value matches a developer-defined regular expression.
    /// A timeout is applied to reduce the risk of excessive processing time.
    /// </summary>
    public static bool MatchesPattern(string? value, string pattern, RegexOptions options = RegexOptions.CultureInvariant)
    {
        return !string.IsNullOrEmpty(value) && Regex.IsMatch(value, pattern, options, RegexTimeout);
    }

    /// <summary>
    /// Returns the canonical allowed value when the input matches one of the supplied values.
    /// Returns null when the input is null, empty, or not allowed.
    /// </summary>
    public static string? NormalizeAllowedValue(string? value, IEnumerable<string> allowedValues, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        foreach (var allowedValue in allowedValues)
        {
            if (string.Equals(value, allowedValue, comparison))
                return allowedValue;
        }

        return null;
    }

    /// <summary>
    /// Checks whether a string represents a positive integer using invariant culture.
    /// Only unsigned decimal digits are accepted.
    /// </summary>
    public static bool IsPositiveInteger(string? value)
    {
        return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var number) && number > 0;
    }

    /// <summary>
    /// Checks whether a string length falls within the supplied inclusive boundaries.
    /// </summary>
    public static bool IsWithinLength(string? value, int minimumLength, int maximumLength)
    {
        return value != null
            && minimumLength >= 0
            && maximumLength >= minimumLength
            && value.Length >= minimumLength
            && value.Length <= maximumLength;
    }
}