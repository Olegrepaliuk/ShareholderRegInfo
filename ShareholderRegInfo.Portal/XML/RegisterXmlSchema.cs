namespace ShareholderRegInfo.Portal.XML;

/// <summary>A group element in the form XML</summary>
public sealed record XmlGroup(string ElementName, string Label);

/// <summary>A value element in the form XML</summary>
public sealed record XmlField(string ElementName, string Label);

/// <summary>A group holding the same figure for the previous and the current year.</summary>
public sealed record XmlYearPair(XmlGroup Group, XmlField PreviousYear, XmlField CurrentYear)
{
    public XmlYearPair(string groupElement, string label, string previousYearElement, string currentYearElement)
        : this(
            new XmlGroup(groupElement, label),
            new XmlField(previousYearElement, $"{label} (fjorår)"),
            new XmlField(currentYearElement, $"{label} (i år)"))
    {
    }
}

/// <summary>
/// Every element name the parser reads from the main form
/// </summary>
public static class RegisterXmlSchema
{
    public const string RootElement = "Skjema";

    public static class Company
    {
        public static readonly XmlGroup Group = new("Selskap-grp-2588", "Selskapsinformasjon");
        public static readonly XmlField OrganizationNumber = new("EnhetOrganisasjonsnummer-datadef-18", "Organisasjonsnummer");
        public static readonly XmlField Name = new("EnhetNavn-datadef-1", "Selskapets navn");
        public static readonly XmlField Street = new("EnhetAdresse-datadef-15", "Adresse");
        public static readonly XmlField PostalCode = new("EnhetPostnummer-datadef-6673", "Postnummer");
        public static readonly XmlField City = new("EnhetPoststed-datadef-6674", "Poststed");
        public static readonly XmlField ShareClass = new("AksjeType-datadef-17659", "Aksjeklasse");
        public static readonly XmlField IncomeYear = new("Inntektsar-datadef-692", "Inntektsår");
    }

    public static class ContactPerson
    {
        public static readonly XmlGroup Group = new("Kontaktperson-grp-3442", "Kontaktperson");
        public static readonly XmlField Name = new("KontaktpersonSkjemaNavn-datadef-33918", "Kontaktpersonens navn");
        public static readonly XmlField Email = new("KontaktpersonSkjemaEPost-datadef-30533", "Kontaktpersonens e-post");
        public static readonly XmlField Phone = new("KontaktpersonSkjemaTelefonnummer-datadef-33916", "Kontaktpersonens telefonnummer");
    }

    public static class ShareCapital
    {
        public static readonly XmlYearPair TotalShareCapital = new(
            "AksjekapitalForHeleSelskapet-grp-3443", "Aksjekapital for hele selskapet",
            "AksjekapitalFjoraret-datadef-7129", "Aksjekapital-datadef-87");

        public static readonly XmlYearPair ShareClassCapital = new(
            "AksjekapitalIDenneAksjeklassen-grp-3444", "Aksjekapital i denne aksjeklassen",
            "AksjekapitalISINAksjetypeFjoraret-datadef-17663", "AksjekapitalISINAksjetype-datadef-17664");

        public static readonly XmlYearPair NominalValuePerShare = new(
            "PalydendePerAksje-grp-3447", "Pålydende per aksje",
            "AksjeMvPalydendeFjoraret-datadef-23944", "AksjeMvPalydende-datadef-23945");

        public static readonly XmlYearPair NumberOfShares = new(
            "AntallAksjerIDenneAksjeklassen-grp-3445", "Antall aksjer",
            "AksjerMvAntallFjoraret-datadef-29166", "AksjerMvAntall-datadef-29167");

        public static readonly XmlYearPair PaidInShareCapital = new(
            "InnbetaltAksjekapitalIDenneAksjeklassen-grp-3446", "Innbetalt aksjekapital",
            "AksjekapitalInnbetaltFjoraret-datadef-8020", "AksjekapitalInnbetalt-datadef-5867");

        public static readonly XmlYearPair PaidInSharePremium = new(
            "InnbetaltOverkursIDenneAksjeklassen-grp-3448", "Innbetalt overkurs",
            "AksjeOverkursISINAksjetypeFjoraret-datadef-17662", "AksjeOverkursISINAksjetype-datadef-17661");
    }
}
