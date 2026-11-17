namespace ShareholderRegInfo.Portal.Models.Register;

/// <summary>Company business address.</summary>
public sealed record Address
{
    /// <summary>EnhetAdresse</summary>
    public string? Street { get; init; }

    /// <summary>EnhetPostnummer</summary>
    public string? PostalCode { get; init; }

    /// <summary>EnhetPoststed</summary>
    public string? City { get; init; }

    public bool IsEmpty =>
        string.IsNullOrWhiteSpace(Street) &&
        string.IsNullOrWhiteSpace(PostalCode) &&
        string.IsNullOrWhiteSpace(City);
}
