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
        return value.NotEmpty() && Regex.IsMatch(value, pattern, options, RegexTimeout);
    }

    /// <summary>
    /// Returns the canonical allowed value or throws a notification exception when the value is invalid.
    /// </summary>
    public static string GetAllowedValueOrThrow(string? value, string parameterName, IEnumerable<string> allowedValues, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        var values = allowedValues.ToArray();

        foreach (var allowedValue in values)
        {
            if (string.Equals(value, allowedValue, comparison))
                return allowedValue;
        }

        throw new NotificationException($"Invalid query parameter '{parameterName}'. Allowed values are {string.Join(", ", values.Select(x => $"'{x}'"))}.");
    }

    /// <summary>
    /// Checks whether a string represents a positive integer using invariant culture.
    /// Only unsigned decimal digits are accepted.
    /// </summary>
    public static bool IsPositiveInteger(string? value)
    {
        return value.NotEmpty() && int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var number) && number > 0;
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
