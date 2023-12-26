using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Runners.ZipCode.Utils.Abstract;
using Soenneker.Utils.FileSync;
using Soenneker.Utils.HttpClientCache.Abstract;
using HtmlAgilityPack;

namespace Soenneker.Runners.ZipCode.Utils;

///<inheritdoc cref="IUspsDownloadUtil"/>
public class UspsDownloadUtil : IUspsDownloadUtil
{
    private readonly ILogger<UspsDownloadUtil> _logger;
    private readonly IHttpClientCache _httpClientCache;

    public UspsDownloadUtil(IHttpClientCache httpClientCache,  ILogger<UspsDownloadUtil> logger)
    {
        _httpClientCache = httpClientCache;
        _logger = logger;
    }

    public async ValueTask<string> Download()
    {
        string directory = await GetDirectory();

        var uri = $"https://postalpro.usps.com/mnt/glusterfs/{directory}/ZIP_Locale_Detail.xls";

        _logger.LogInformation("Downloading file from uri ({uri}) ...", uri);

        HttpClient client = await _httpClientCache.Get(nameof(UspsDownloadUtil));

        HttpResponseMessage response = await client.GetAsync(uri);

        string tempFile = FileUtilSync.GetTempFileName() + ".xls";

        await using (var fs = new FileStream(tempFile, FileMode.CreateNew))
        {
            await response.Content.CopyToAsync(fs);
        }

        _logger.LogDebug("Finished downloading file from uri ({uri})", uri);

        return tempFile;
    }

    public async ValueTask<DateTime?> GetLastUpdatedDateTime()
    {
        _logger.LogInformation("Downloading https://postalpro.usps.com/ZIP_Locale_Detail to get the HTML so we can find the last updated date...");

        HttpClient client = await _httpClientCache.Get(nameof(UspsDownloadUtil));
        HttpResponseMessage message = await client.GetAsync("https://postalpro.usps.com/ZIP_Locale_Detail");
        string html = await message.Content.ReadAsStringAsync();

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

    public async ValueTask<string> GetDirectory()
    {
        string result;

        DateTime? retrievedDateTime = await GetLastUpdatedDateTime();

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