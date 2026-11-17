using ShareholderRegInfo.Portal.XML;

namespace ShareholderRegInfo.Portal.Services;

public interface IRegisterService
{
    /// <summary>Reads and parses the configured shareholder register file.</summary>
    Task<RegisterLoadResult> LoadAsync(CancellationToken cancellationToken = default);
}
