namespace ShareholderRegInfo.Portal.Models.Register;

/// <summary>
/// A value reported for both the previous and the current income year.
/// </summary>
public sealed record YearComparison<T>(T? PreviousYear, T? CurrentYear) where T : struct
{
    public static YearComparison<T> Empty { get; } = new(null, null);

    public bool HasAnyValue => PreviousYear.HasValue || CurrentYear.HasValue;

    public bool HasChanged =>
        PreviousYear.HasValue && CurrentYear.HasValue &&
        !EqualityComparer<T>.Default.Equals(PreviousYear.Value, CurrentYear.Value);
}
