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
    /// Executes the download operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string> Download(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets date from html.
    /// </summary>
    /// <param name="html">The html.</param>
    /// <returns>The result of the operation.</returns>
    DateTime? GetDateFromHtml(string html);

    /// <summary>
    /// Gets last updated date time.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<DateTime?> GetLastUpdatedDateTime(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets directory.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string> GetDirectory(CancellationToken cancellationToken = default);
}