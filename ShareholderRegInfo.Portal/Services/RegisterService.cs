using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using ShareholderRegInfo.Portal.XML;

namespace ShareholderRegInfo.Portal.Services;

/// <summary>
/// Loads the shareholder register XML file from disk and parses it.
/// Expected failures (missing file, invalid XML) become a failed RegisterLoadResult.
/// </summary>
public sealed class RegisterService(
    IRegisterFormParser parser,
    IOptions<RegisterOptions> options,
    IWebHostEnvironment environment,
    ILogger<RegisterService> logger) : IRegisterService
{
    private static readonly XmlReaderSettings ReaderSettings = new()
    {
        Async = true,
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = null,
    };

    public async Task<RegisterLoadResult> LoadAsync(CancellationToken cancellationToken = default)
    {
        var path = ResolvePath(options.Value.FilePath);
        var fileName = Path.GetFileName(path);

        try
        {
            await using var stream = new FileStream(
                path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            using var reader = XmlReader.Create(stream, ReaderSettings);

            var document = await XDocument.LoadAsync(reader, LoadOptions.None, cancellationToken);
            var result = parser.Parse(document);

            if (!result.IsSuccess)
            {
                logger.LogWarning("Registerfilen {Path} er ikke et gjenkjent skjema: {Error}", path, result.ErrorMessage);
            }
            else if (result.Warnings.Count > 0)
            {
                logger.LogWarning("Registerfilen {Path} ble lest med {Count} advarsel(er)",
                    path, result.Warnings.Count);

                foreach (var warning in result.Warnings)
                {
                    logger.LogWarning("Advarsel: {Warning}", warning);
                }
            }

            return result;
        }
        catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
        {
            logger.LogError(ex, "Fant ikke registerfilen: {Path}", path);
            return RegisterLoadResult.Failed($"Fant ikke filen «{fileName}».");
        }
        catch (XmlException ex)
        {
            logger.LogError(ex, "Registerfilen {Path} er ikke gyldig XML", path);
            return RegisterLoadResult.Failed(
                $"Filen «{fileName}» kunne ikke leses fordi innholdet ikke er gyldig XML (linje {ex.LineNumber}).");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            logger.LogError(ex, "Registerfilen {Path} kunne ikke åpnes", path);
            return RegisterLoadResult.Failed($"Filen «{fileName}» kunne ikke åpnes.");
        }
    }

    private string ResolvePath(string configuredPath) =>
        Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(environment.ContentRootPath, configuredPath);
}
