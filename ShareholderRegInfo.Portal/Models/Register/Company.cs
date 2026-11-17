namespace ShareholderRegInfo.Portal.Models.Register;

/// <summary>Company information (group Selskap-grp-2588).</summary>
public sealed record Company
{
    /// <summary>EnhetNavn.</summary>
    public string? Name { get; init; }

    /// <summary>EnhetOrganisasjonsnummer, 9 digits</summary>
    public string? OrganizationNumber { get; init; }

    public Address Address { get; init; } = new();

    /// <summary>AksjeType.</summary>
    public ShareClass? ShareClass { get; init; }

    /// <summary>Inntektsar</summary>
    public int? IncomeYear { get; init; }
}
