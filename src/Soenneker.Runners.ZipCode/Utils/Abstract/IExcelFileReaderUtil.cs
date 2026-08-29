using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Runners.ZipCode.Utils.Abstract;

/// <summary>
/// Defines the excel file reader util contract.
/// </summary>
public interface IExcelFileReaderUtil
{
    /// <summary>
    /// Creates zip codes from xls.
    /// </summary>
    /// <param name="path">Path of the file or directory to use.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by create Zip Codes From Xls.</returns>
    ValueTask<string> CreateZipCodesFromXls(string path, CancellationToken cancellationToken = default);
}
