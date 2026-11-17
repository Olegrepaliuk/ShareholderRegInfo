namespace ShareholderRegInfo.Portal.Models.Register;

/// <summary>
/// The parts of the shareholder register main form (OrdinaereHovedskjema)
/// </summary>
public sealed record ShareholderRegisterForm
{
    public Company Company { get; init; } = new();
    public ContactPerson ContactPerson { get; init; } = new();
    public ShareCapitalInfo ShareCapital { get; init; } = new();
}
