namespace ShareholderRegInfo.Portal.Models.Register;

/// <summary>Contact person for the form (group Kontaktperson-grp-3442).</summary>
public sealed record ContactPerson
{
    /// <summary>KontaktpersonSkjemaNavn.</summary>
    public string? Name { get; init; }

    /// <summary>KontaktpersonSkjemaEPost.</summary>
    public string? Email { get; init; }

    /// <summary>KontaktpersonSkjemaTelefonnummer.</summary>
    public string? Phone { get; init; }

    public bool IsEmpty =>
        string.IsNullOrWhiteSpace(Name) &&
        string.IsNullOrWhiteSpace(Email) &&
        string.IsNullOrWhiteSpace(Phone);
}
