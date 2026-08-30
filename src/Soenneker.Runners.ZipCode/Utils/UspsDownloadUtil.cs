using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using Soenneker.AngleSharp.Parser.Abstract;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Runners.ZipCode.Utils.Abstract;
using Soenneker.Utils.File.Download.Abstract;
using Soenneker.Utils.HttpClientCache.Abstract;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Runners.ZipCode.Utils;

public sealed class UspsDownloadUtil : IUspsDownloadUtil
{
    private readonly ILogger<UspsDownloadUtil> _logger;
    private readonly IHttpClientCache _httpClientCache;
    private readonly IFileDownloadUtil _fileDownloadUtil;
    private readonly IAngleSharpParser _angleSharpParser;

    public UspsDownloadUtil(IHttpClientCache httpClientCache, ILogger<UspsDownloadUtil> logger, IFileDownloadUtil fileDownloadUtil,
        IAngleSharpParser angleSharpParser)
    {
        _httpClientCache = httpClientCache;
        _logger = logger;
        _fileDownloadUtil = fileDownloadUtil;
        _angleSharpParser = angleSharpParser;
    }

    public async ValueTask<string> Download(CancellationToken cancellationToken = default)
    {
        string directory = await GetDirectory(cancellationToken);

        var uri = $"https://postalpro.usps.com/mnt/glusterfs/{directory}/ZIP_Locale_Detail.xls";

        string? path = await _fileDownloadUtil.Download(uri, fileExtension: "xls", cancellationToken: cancellationToken);

        return path ?? throw new InvalidOperationException("The USPS ZIP locale workbook could not be downloaded.");
    }

    public async ValueTask<DateTime?> GetLastUpdatedDateTime(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Downloading https://postalpro.usps.com/ZIP_Locale_Detail to get the HTML so we can find the last updated date...");

        HttpClient client = await _httpClientCache.Get(nameof(UspsDownloadUtil), cancellationToken: cancellationToken).NoSync();
        using HttpResponseMessage message = await client.GetAsync("https://postalpro.usps.com/ZIP_Locale_Detail", cancellationToken).NoSync();
        message.EnsureSuccessStatusCode();
        string html = await message.Content.ReadAsStringAsync(cancellationToken).NoSync();

        DateTime? dateTime = await GetDateFromHtml(html, cancellationToken).NoSync();

        return dateTime;
    }

    public async ValueTask<DateTime?> GetDateFromHtml(string html, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting the last updated date from HTML...");

        HtmlParser parser = await _angleSharpParser.Get(cancellationToken).NoSync();

        try
        {
            using IDocument document = await parser.ParseDocumentAsync(html, cancellationToken).NoSync();
            IElement? dateElement = document.QuerySelector("div.mb-2");

            return dateElement is null ? null : Convert.ToDateTime(dateElement.TextContent);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error parsing page");
        }

        return null;
    }

    public async ValueTask<string> GetDirectory(CancellationToken cancellationToken = default)
    {
        string result;

        DateTime? retrievedDateTime = await GetLastUpdatedDateTime(cancellationToken).NoSync();

        if (retrievedDateTime != null)
            result = retrievedDateTime.Value.ToString("yyyy-MM");
        else
        {
            _logger.LogWarning("Using DateTime.UtcNow for file name guess because we can't retrieve from the page (the layout has changed most likely)");
            result = DateTime.UtcNow.ToString("yyyy-MM");
        }

        return result;
    }
}
