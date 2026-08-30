using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Runners.ZipCode.Utils.Abstract;

/// <summary>
/// Converts a USPS ZIP Locale Detail workbook into a text file of unique ZIP Codes.
/// </summary>
public interface IExcelFileReaderUtil
{
    /// <summary>
    /// Extracts unique delivery ZIP Codes from the workbook's <c>ZIP_DETAIL</c> worksheet.
    /// </summary>
    /// <param name="path">Path to the USPS XLS workbook.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The path to the generated text file.</returns>
    ValueTask<string> CreateZipCodesFromXls(string path, CancellationToken cancellationToken = default);
}
