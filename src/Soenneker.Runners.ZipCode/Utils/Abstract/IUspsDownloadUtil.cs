using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Runners.ZipCode.Utils.Abstract;

/// <summary>
/// Locates and downloads the current USPS ZIP Locale Detail workbook.
/// </summary>
public interface IUspsDownloadUtil
{
    /// <summary>
    /// Downloads the current USPS ZIP Locale Detail workbook.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The local path to the downloaded workbook.</returns>
    ValueTask<string> Download(CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts the published update date from the USPS page HTML.
    /// </summary>
    /// <param name="html">Rendered page HTML to inspect.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The parsed update date, or <see langword="null"/> when the page does not contain a recognizable date.</returns>
    ValueTask<DateTime?> GetDateFromHtml(string html, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the update date from the USPS ZIP Locale Detail page.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The parsed update date, or <see langword="null"/> when it cannot be determined.</returns>
    ValueTask<DateTime?> GetLastUpdatedDateTime(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the year-month directory used by USPS for the current workbook.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A directory segment formatted as <c>yyyy-MM</c>.</returns>
    ValueTask<string> GetDirectory(CancellationToken cancellationToken = default);
}
