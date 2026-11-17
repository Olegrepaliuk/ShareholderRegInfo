using ShareholderRegInfo.Portal.Models.Register;

namespace ShareholderRegInfo.Portal.XML;

/// <summary>
/// Outcome of reading the register file.
/// Expected problems (missing file, invalid XML, missing or malformed fields) are reported
/// </summary>
public sealed record RegisterLoadResult
{
    public ShareholderRegisterForm? Form { get; init; }

    public IReadOnlyList<string> Warnings { get; init; } = [];

    public string? ErrorMessage { get; init; }

    public bool IsSuccess => Form is not null;

    public static RegisterLoadResult Success(ShareholderRegisterForm form, IReadOnlyList<string> warnings) =>
        new() { Form = form, Warnings = warnings };

    public static RegisterLoadResult Failed(string errorMessage) =>
        new() { ErrorMessage = errorMessage };
}
