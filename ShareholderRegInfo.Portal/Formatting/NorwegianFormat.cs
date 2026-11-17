using System.Globalization;

namespace ShareholderRegInfo.Portal.Formatting;

/// <summary>
/// Formats values the way a Norwegian accounting user expects.
/// Every method accepts null and returns Missing for it, so views never need null checks.
/// </summary>
public static class NorwegianFormat
{
    public const string Missing = "Ikke oppgitt";

    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("nb-NO");

    public static string Amount(decimal? value) =>
        value is { } v ? $"{v.ToString("N2", Culture)} kr" : Missing;

    public static string NominalValue(decimal? value) =>
        value is { } v ? $"{v.ToString("#,##0.00####", Culture)} kr" : Missing;

    public static string Count(long? value) =>
        value is { } v ? v.ToString("N0", Culture) : Missing;

    public static string OrganizationNumber(string? value) =>
        value switch
        {
            null or "" => Missing,
            { Length: 9 } when value.All(char.IsAsciiDigit) => $"{value[..3]} {value[3..6]} {value[6..]}",
            _ => value,
        };

    public static string Text(string? value) =>
        string.IsNullOrWhiteSpace(value) ? Missing : value;

    public static string Year(int? value) =>
        value?.ToString(CultureInfo.InvariantCulture) ?? Missing;
}
