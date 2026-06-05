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
    /// <param name="path">The path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string> CreateZipCodesFromXls(string path, CancellationToken cancellationToken = default);
}