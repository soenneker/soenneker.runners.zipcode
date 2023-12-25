using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Runners.ZipCode.Utils.Abstract;
using Soenneker.Utils.FileSync;
using Soenneker.Utils.HttpClientCache.Abstract;

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
        HttpClient client = await _httpClientCache.Get(nameof(UspsDownloadUtil));

        var uri = $"https://postalpro.usps.com/mnt/glusterfs/{GetDirectory()}/ZIP_Locale_Detail.xls";

        _logger.LogInformation("Downloading file from uri ({uri}) ...", uri);

        HttpResponseMessage response = await client.GetAsync(uri);

        string tempFile = FileUtilSync.GetTempFileName() + ".xls";

        using (var fs = new FileStream(tempFile, FileMode.CreateNew))
        {
            await response.Content.CopyToAsync(fs);
        }

        _logger.LogDebug("Finished downloading file from uri ({uri})", uri);

        return tempFile;
    }

    public static string GetDirectory()
    {
        var result = DateTime.UtcNow.ToString("yyyy-MM");

        return result;
    }
}