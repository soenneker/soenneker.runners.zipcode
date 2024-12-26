using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Runners.ZipCode.Utils.Abstract;

public interface IUspsDownloadUtil
{
    ValueTask<string> Download(CancellationToken cancellationToken = default);

    DateTime? GetDateFromHtml(string html);

    ValueTask<DateTime?> GetLastUpdatedDateTime();

    ValueTask<string> GetDirectory();
}