using ShareholderRegInfo.Portal.Models.Register;

namespace ShareholderRegInfo.Portal.Models;


public sealed class RegisterViewModel
{
    public ShareholderRegisterForm? Form { get; init; }

    public IReadOnlyList<string> Warnings { get; init; } = [];

    public string? ErrorMessage { get; init; }

    public bool HasForm => Form is not null;
    public bool HasWarnings => Warnings.Count > 0;

    public static RegisterViewModel Failed(string errorMessage) => new() { ErrorMessage = errorMessage };
}
