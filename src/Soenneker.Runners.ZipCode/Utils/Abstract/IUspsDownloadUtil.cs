using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Runners.ZipCode.Utils.Abstract;

/// <summary>
/// Defines the usps download util contract.
/// </summary>
public interface IUspsDownloadUtil
{
    /// <summary>
    /// Downloads usps Download.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by download.</returns>
    ValueTask<string> Download(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets date from html.
    /// </summary>
    /// <param name="html">Rendered page HTML to inspect.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested date Time.</returns>
    ValueTask<DateTime?> GetDateFromHtml(string html, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets last updated date time.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested date Time.</returns>
    ValueTask<DateTime?> GetLastUpdatedDateTime(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets directory.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by get Directory.</returns>
    ValueTask<string> GetDirectory(CancellationToken cancellationToken = default);
}
