using System.Xml.Linq;
using ShareholderRegInfo.Portal.XML;

namespace ShareholderRegInfo.Tests;

/// <summary>
/// Loads the sample form and builds variants of it for tests.
/// </summary>
internal static class SampleForm
{
    private static readonly string FilePath =
        Path.Combine(AppContext.BaseDirectory, "TestData", "OrdinaereHovedskjema.xml");

    public static XDocument Load() => XDocument.Load(FilePath);

    public static XDocument WithValue(this XDocument document, XmlField field, string value)
    {
        Find(document, field.ElementName).Value = value;
        return document;
    }

    public static XDocument Without(this XDocument document, XmlField field)
    {
        Find(document, field.ElementName).Remove();
        return document;
    }

    public static XDocument Without(this XDocument document, XmlGroup group)
    {
        Find(document, group.ElementName).Remove();
        return document;
    }

    private static XElement Find(XDocument document, string elementName) =>
        document.Descendants().Single(e => e.Name.LocalName == elementName);
}
