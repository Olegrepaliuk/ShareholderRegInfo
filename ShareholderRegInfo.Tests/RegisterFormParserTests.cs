using System.Xml.Linq;
using ShareholderRegInfo.Portal.Models.Register;
using ShareholderRegInfo.Portal.Services;
using Schema = ShareholderRegInfo.Portal.XML.RegisterXmlSchema;

namespace ShareholderRegInfo.Tests;

public class RegisterFormParserTests
{
    private readonly RegisterFormParser _parser = new();

    private (ShareholderRegisterForm Form, IReadOnlyList<string> Warnings) ParseSuccessfully(XDocument document)
    {
        var result = _parser.Parse(document);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        return (result.Form, result.Warnings);
    }

    [Fact]
    public void Parse_SampleFile_ReadsCompanyInformation()
    {
        var (form, warnings) = ParseSuccessfully(SampleForm.Load());

        Assert.Empty(warnings);
        Assert.Equal("Testklient 141678", form.Company.Name);
        Assert.Equal("313849899", form.Company.OrganizationNumber);
        Assert.Equal("Karl Johans Gate 4", form.Company.Address.Street);
        Assert.Equal("0567", form.Company.Address.PostalCode);
        Assert.Equal("BERGEN", form.Company.Address.City);
        Assert.Equal("03", form.Company.ShareClass?.Code);
        Assert.Equal("A-aksjer", form.Company.ShareClass?.DisplayName);
        Assert.Equal(2024, form.Company.IncomeYear);
    }

    [Fact]
    public void Parse_SampleFile_ReadsContactPerson()
    {
        var (form, _) = ParseSuccessfully(SampleForm.Load());

        Assert.Equal("John Johnson", form.ContactPerson.Name);
        Assert.Equal("jjohnson@epost.no", form.ContactPerson.Email);
        Assert.Equal("214567", form.ContactPerson.Phone);
    }

    [Fact]
    public void Parse_SampleFile_ReadsShareCapitalForBothYears()
    {
        var (form, _) = ParseSuccessfully(SampleForm.Load());
        var capital = form.ShareCapital;

        Assert.Equal(new YearComparison<decimal>(800000m, 800000m), capital.TotalShareCapital);
        Assert.Equal(new YearComparison<decimal>(400000m, 400000m), capital.ShareClassCapital);
        Assert.Equal(new YearComparison<decimal>(300m, 300m), capital.NominalValuePerShare);
        Assert.Equal(new YearComparison<long>(1800, 1800), capital.NumberOfShares);
        Assert.Equal(new YearComparison<decimal>(800000m, 800000m), capital.PaidInShareCapital);
        Assert.Equal(new YearComparison<decimal>(0m, 0m), capital.PaidInSharePremium);
        Assert.False(capital.TotalShareCapital.HasChanged);
    }

    [Fact]
    public void Parse_UnknownRootElement_Fails()
    {
        var result = _parser.Parse(XDocument.Parse("<Faktura><Belop>100</Belop></Faktura>"));

        Assert.False(result.IsSuccess);
        Assert.Null(result.Form);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public void Parse_MissingRequiredField_LeavesNullAndWarns()
    {
        var (form, warnings) = ParseSuccessfully(SampleForm.Load().Without(Schema.Company.Name));

        Assert.Null(form.Company.Name);
        var warning = Assert.Single(warnings);
        Assert.Contains(Schema.Company.Name.Label, warning);
    }

    [Fact]
    public void Parse_EmptyRequiredField_IsTreatedAsMissing()
    {
        var (form, warnings) = ParseSuccessfully(SampleForm.Load().WithValue(Schema.Company.City, "   "));

        Assert.Null(form.Company.Address.City);
        Assert.Single(warnings);
    }

    [Theory]
    [InlineData("25")]
    [InlineData("year25")]
    [InlineData("")]
    public void Parse_InvalidIncomeYear_LeavesNullAndWarns(string value)
    {
        var (form, warnings) = ParseSuccessfully(SampleForm.Load().WithValue(Schema.Company.IncomeYear, value));

        Assert.Null(form.Company.IncomeYear);
        Assert.Contains(warnings, w => w.Contains(Schema.Company.IncomeYear.Label));
    }

    [Theory]
    [InlineData("01", "Ordinære aksjer")]
    [InlineData("02", "Preferanseaksjer")]
    [InlineData("03", "A-aksjer")]
    [InlineData("06", "D-aksjer")]
    [InlineData("07", "Ekstraordinære aksjer")]
    [InlineData("08", "E-aksjer")]
    [InlineData("13", "J-aksjer")]
    public void ShareClass_DisplayName_KnownCode_ReturnsOfficialName(string code, string expected)
    {
        Assert.Equal(expected, new ShareClass(code).DisplayName);
    }
}
