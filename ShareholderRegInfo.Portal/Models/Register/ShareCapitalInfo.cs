namespace ShareholderRegInfo.Portal.Models.Register;

/// <summary>
/// Share capital and number of shares (group Selskapsopplysninger-grp-2589).
/// Every figure is reported for both the previous and the current year.
/// </summary>
public sealed record ShareCapitalInfo
{
    /// <summary>Share capital for the whole company (Aksjekapital)</summary>
    public YearComparison<decimal> TotalShareCapital { get; init; } = YearComparison<decimal>.Empty;

    /// <summary>Share capital in this share class (AksjekapitalISINAksjetype)</summary>
    public YearComparison<decimal> ShareClassCapital { get; init; } = YearComparison<decimal>.Empty;

    /// <summary>Nominal value per share (AksjeMvPalydende)</summary>
    public YearComparison<decimal> NominalValuePerShare { get; init; } = YearComparison<decimal>.Empty;

    /// <summary>Number of shares in this share class (AksjerMvAntall).</summary>
    public YearComparison<long> NumberOfShares { get; init; } = YearComparison<long>.Empty;

    /// <summary>Paid-in share capital in this share class (AksjekapitalInnbetalt)</summary>
    public YearComparison<decimal> PaidInShareCapital { get; init; } = YearComparison<decimal>.Empty;

    /// <summary>Paid-in share premium in this share class (AksjeOverkursISINAksjetype)</summary>
    public YearComparison<decimal> PaidInSharePremium { get; init; } = YearComparison<decimal>.Empty;
}
