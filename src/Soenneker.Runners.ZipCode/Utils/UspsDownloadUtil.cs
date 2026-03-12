using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
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

///<inheritdoc cref="IUspsDownloadUtil"/>
public sealed class UspsDownloadUtil : IUspsDownloadUtil
{
    private readonly ILogger<UspsDownloadUtil> _logger;
    private readonly IHttpClientCache _httpClientCache;
    private readonly IFileDownloadUtil _fileDownloadUtil;

    public UspsDownloadUtil(IHttpClientCache httpClientCache,  ILogger<UspsDownloadUtil> logger, IFileDownloadUtil fileDownloadUtil)
    {
        _httpClientCache = httpClientCache;
        _logger = logger;
        _fileDownloadUtil = fileDownloadUtil;
    }

    public async ValueTask<string> Download(CancellationToken cancellationToken = default)
    {
        string directory = await GetDirectory(cancellationToken);

        var uri = $"https://postalpro.usps.com/mnt/glusterfs/{directory}/ZIP_Locale_Detail.xls";

        return (await _fileDownloadUtil.Download(uri, fileExtension: "xls", cancellationToken: cancellationToken))!;
    }

    public async ValueTask<DateTime?> GetLastUpdatedDateTime(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Downloading https://postalpro.usps.com/ZIP_Locale_Detail to get the HTML so we can find the last updated date...");

        HttpClient client = await _httpClientCache.Get(nameof(UspsDownloadUtil), cancellationToken: cancellationToken).NoSync();
        HttpResponseMessage message = await client.GetAsync("https://postalpro.usps.com/ZIP_Locale_Detail", cancellationToken).NoSync();
        string html = await message.Content.ReadAsStringAsync(cancellationToken).NoSync();

        DateTime? dateTime = GetDateFromHtml(html);

        return dateTime;
    }

    public DateTime? GetDateFromHtml(string html)
    {
        _logger.LogInformation("Getting the last updated date from HTML...");

        var htmlDoc = new HtmlDocument();

        try
        {
            htmlDoc.LoadHtml(html);

            HtmlNode myDivNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='mb-2']");

            return Convert.ToDateTime(myDivNode.InnerText);
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