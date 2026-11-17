using System.Globalization;
using System.Numerics;
using System.Xml.Linq;
using ShareholderRegInfo.Portal.Models.Register;
using ShareholderRegInfo.Portal.XML;
using Schema = ShareholderRegInfo.Portal.XML.RegisterXmlSchema;

namespace ShareholderRegInfo.Portal.Services;

/// <summary>
/// Maps the main form XML (OrdinaereHovedskjema) to ShareholderRegisterForm.
/// </summary>
public sealed class RegisterFormParser : IRegisterFormParser
{
    private const NumberStyles DecimalStyle =
        NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

    private const NumberStyles IntegerStyle =
        NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign;

    public RegisterLoadResult Parse(XDocument document)
    {
        var root = document.Root;

        if (root is null || root.Name.LocalName != Schema.RootElement)
        {
            return RegisterLoadResult.Failed("Filen er ikke en aksjonærregisteroppgave (hovedskjema).");
        }

        var warnings = new List<string>();
        var form = new ShareholderRegisterForm
        {
            Company = ReadCompany(root, warnings),
            ContactPerson = ReadContactPerson(root, warnings),
            ShareCapital = ReadShareCapital(root, warnings),
        };

        return RegisterLoadResult.Success(form, warnings);
    }

    private static Company ReadCompany(XElement root, List<string> warnings)
    {
        var group = FindGroup(root, Schema.Company.Group, warnings);
        if (group is null)
        {
            return new Company();
        }

        return new Company
        {
            Name = ReadText(group, Schema.Company.Name, warnings),
            OrganizationNumber = ReadOrganizationNumber(group, warnings),
            Address = new Address
            {
                Street = ReadText(group, Schema.Company.Street, warnings),
                PostalCode = ReadPostalCode(group, warnings),
                City = ReadText(group, Schema.Company.City, warnings),
            },
            ShareClass = ReadShareClass(group, warnings),
            IncomeYear = ReadIncomeYear(group, warnings),
        };
    }

    private static ContactPerson ReadContactPerson(XElement root, List<string> warnings)
    {
        var group = FindGroup(root, Schema.ContactPerson.Group, warnings);

        if (group is null)
        {
            return new ContactPerson();
        }

        return new ContactPerson
        {
            Name = ReadText(group, Schema.ContactPerson.Name, warnings),
            Email = ReadText(group, Schema.ContactPerson.Email, warnings),
            Phone = ReadText(group, Schema.ContactPerson.Phone, warnings),
        };
    }

    private static ShareCapitalInfo ReadShareCapital(XElement root, List<string> warnings) => new()
    {

        TotalShareCapital = ReadYearPair<decimal>(root, Schema.ShareCapital.TotalShareCapital, DecimalStyle, required: true, warnings),
        NominalValuePerShare = ReadYearPair<decimal>(root, Schema.ShareCapital.NominalValuePerShare, DecimalStyle, required: true, warnings),
        NumberOfShares = ReadYearPair<long>(root, Schema.ShareCapital.NumberOfShares, IntegerStyle, required: true, warnings),

        ShareClassCapital = ReadYearPair<decimal>(root, Schema.ShareCapital.ShareClassCapital, DecimalStyle, required: false, warnings),
        PaidInShareCapital = ReadYearPair<decimal>(root, Schema.ShareCapital.PaidInShareCapital, DecimalStyle, required: false, warnings),
        PaidInSharePremium = ReadYearPair<decimal>(root, Schema.ShareCapital.PaidInSharePremium, DecimalStyle, required: false, warnings),
    };


    private static string? ReadOrganizationNumber(XElement group, List<string> warnings)
    {
        var field = Schema.Company.OrganizationNumber;
        var text = ReadText(group, field, warnings);

        if (text is null)
        {
            return null;
        }

        var digits = text.Replace(" ", string.Empty);
        if (digits.Length != 9 || !digits.All(char.IsAsciiDigit))
        {
            warnings.Add($"{field.Label} «{text}» har ugyldig format. Det skal bestå av 9 siffer.");
            return text;
        }

        return digits;
    }

    private static string? ReadPostalCode(XElement group, List<string> warnings)
    {
        var field = Schema.Company.PostalCode;
        var text = ReadText(group, field, warnings);
        if (text is not null && (text.Length != 4 || !text.All(char.IsAsciiDigit)))
        {
            warnings.Add($"{field.Label} «{text}» har ugyldig format. Det skal bestå av 4 siffer.");
        }

        return text;
    }

    private static ShareClass? ReadShareClass(XElement group, List<string> warnings)
    {
        var field = Schema.Company.ShareClass;
        var code = ReadText(group, field, warnings);
        if (code is null)
        {
            return null;
        }

        if (code.Length != 2 || !int.TryParse(code, NumberStyles.None, CultureInfo.InvariantCulture, out var n) || n is < 1 or > 13)
        {
            warnings.Add($"{field.Label} «{code}» er ikke en gyldig kode.");
        }

        return new ShareClass(code);
    }

    private static int? ReadIncomeYear(XElement group, List<string> warnings)
    {
        var field = Schema.Company.IncomeYear;
        var text = ReadText(group, field, warnings);
        if (text is null)
        {
            return null;
        }

        if (text.Length == 4 && int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var year))
        {
            return year;
        }

        warnings.Add($"{field.Label} «{text}» har ugyldig format.");
        return null;
    }

    private static YearComparison<T> ReadYearPair<T>(
        XElement root, XmlYearPair pair, NumberStyles style, bool required, List<string> warnings)
        where T : struct, INumber<T>
    {
        var group = FindGroup(root, pair.Group, warnings, required);
        if (group is null)
        {
            return YearComparison<T>.Empty;
        }

        return new YearComparison<T>(
            PreviousYear: ReadNumber<T>(group, pair.PreviousYear, style, required: false, warnings),
            CurrentYear: ReadNumber<T>(group, pair.CurrentYear, style, required, warnings));
    }

    private static T? ReadNumber<T>(
        XElement parent, XmlField field, NumberStyles style, bool required, List<string> warnings)
        where T : struct, INumber<T>
    {
        var text = ReadText(parent, field, warnings, required);
        if (text is null)
        {
            return null;
        }

        if (!T.TryParse(text, style, CultureInfo.InvariantCulture, out var value))
        {
            warnings.Add($"{field.Label} har ugyldig verdi «{text}».");
            return null;
        }

        if (T.IsNegative(value))
        {
            warnings.Add($"{field.Label} kan ikke være negativ («{text}»).");
            return null;
        }

        return value;
    }

    private static XElement? FindGroup(XElement root, XmlGroup group, List<string> warnings, bool required = true)
    {
        var element = root.Descendants().FirstOrDefault(e => e.Name.LocalName == group.ElementName);
        if (element is null && required)
        {
            warnings.Add($"«{group.Label}» mangler i filen.");
        }

        return element;
    }

    private static string? ReadText(XElement parent, XmlField field, List<string> warnings, bool required = true)
    {
        var value = parent.Elements()
            .FirstOrDefault(e => e.Name.LocalName == field.ElementName)?
            .Value.Trim();

        if (string.IsNullOrEmpty(value))
        {
            if (required)
            {
                warnings.Add($"{field.Label} mangler i filen.");
            }

            return null;
        }

        return value;
    }
}
