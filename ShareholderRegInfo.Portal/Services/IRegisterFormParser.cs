using System.Xml.Linq;
using ShareholderRegInfo.Portal.XML;

namespace ShareholderRegInfo.Portal.Services;

public interface IRegisterFormParser
{
    /// <summary>
    /// Maps an already loaded main form document to the register model.
    /// Missing or malformed fields are reported as warnings in the result.
    /// </summary>
    RegisterLoadResult Parse(XDocument document);
}
