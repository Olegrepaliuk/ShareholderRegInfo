# ShareholderRegInfo

A small web app that shows the contents of a **shareholder register return** in the browser. It reads the XML file and presents it on a single page with labels and number formatting, so a finance user never sees raw XML.

The page shows three sections:

- **Selskapsinformasjon**: name, organisation number, address, share type, income year
- **Kontaktperson for skjemaet**: name, email, phone
- **Aksjekapital og antall aksjer**: share capital, nominal value and number of shares, last year and this year side by side

## Technology

| | |
| --- | --- |
| Framework | **ASP.NET Core MVC** on **.NET 10** (C#, Razor views) |
| XML parsing | LINQ to XML (`System.Xml.Linq`), built in, no extra packages |
| Tests | xUnit |


## How it works

```
appsettings.json ("Register:FilePath")
        │
        ▼
RegisterService ──► opens Data/OrdinaereHovedskjema.xml, loads it as XDocument
        │
        ▼
RegisterFormParser ──► reads elements by the names in RegisterXmlSchema,
        │              validates each field, collects warnings
        ▼
RegisterLoadResult (ShareholderRegisterForm + warnings, or an error message)
        │
        ▼
HomeController.Index ──► RegisterViewModel ──► Views/Home/Index.cshtml
                                                (formats values)
```

## Key files

### Data file: `ShareholderRegInfo.Portal/Data/OrdinaereHovedskjema.xml`

The main form to display. It is copied to the build and publish output. To show a different file, change the path in `appsettings.json`:

```json
"Register": {
  "FilePath": "Data/OrdinaereHovedskjema.xml"
}
```

### XML field names: `ShareholderRegInfo.Portal/XML/RegisterXmlSchema.cs`

Every XML group and element name the app reads is defined here, grouped by section (`Company`, `ContactPerson`, `ShareCapital`).

No other file contains raw element names, so if an element is renamed in a new version of the form, this is the only file to change.

### Parser: `ShareholderRegInfo.Portal/Services/RegisterFormParser.cs`

Turns an `XDocument` into a `ShareholderRegisterForm`.

### View: `ShareholderRegInfo.Portal/Views/Home/Index.cshtml`

The only page. It renders the three sections as Bootstrap cards; the share capital table shows last year and this year side by side. All values go through formatting, and missing values show as "Ikke oppgitt". If the file cannot be read, the page shows an error message instead.

Share class codes are translated to names in `Models/Register/ShareClass.cs`, using the official RF-1086 specification (01 = Ordinære aksjer, 02 = Preferanseaksjer, 03 = A-aksjer, ...).


## Tests

The tests in `ShareholderRegInfo.Tests` run the parser against their own copy of the data file (`TestData/OrdinaereHovedskjema.xml`) and against variants with missing fields, invalid numbers and wrong codes.
