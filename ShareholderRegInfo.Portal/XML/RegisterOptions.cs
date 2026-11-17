namespace ShareholderRegInfo.Portal.XML;

/// <summary>Settings from the "Register" section in appsettings.json.</summary>
public sealed class RegisterOptions
{
    public const string SectionName = "Register";
    public string FilePath { get; set; } = string.Empty;
}
