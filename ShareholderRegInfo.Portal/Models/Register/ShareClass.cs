namespace ShareholderRegInfo.Portal.Models.Register;

/// <summary>
/// Share class (AksjeType), a code from the list KodelisteAksjeklasse ("01"–"13")
/// </summary>
public sealed record ShareClass(string Code)
{
    private static readonly IReadOnlyDictionary<string, string> KnownNames =
        new Dictionary<string, string>
        {
            ["01"] = "Ordinære aksjer",
            ["02"] = "Preferanseaksjer",
            ["03"] = "A-aksjer",
            ["04"] = "B-aksjer",
            ["05"] = "C-aksjer",
            ["06"] = "D-aksjer",
            ["07"] = "Ekstraordinære aksjer",
            ["08"] = "E-aksjer",
            ["09"] = "F-aksjer",
            ["10"] = "G-aksjer",
            ["11"] = "H-aksjer",
            ["12"] = "I-aksjer",
            ["13"] = "J-aksjer",
        };

    public string DisplayName =>
        KnownNames.TryGetValue(Code, out var name) ? name : Code;
}
