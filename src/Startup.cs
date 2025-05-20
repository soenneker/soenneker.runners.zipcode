using Microsoft.Extensions.DependencyInjection;
using Soenneker.Managers.Runners.Registrars;
using Soenneker.Runners.ZipCode.Utils;
using Soenneker.Runners.ZipCode.Utils.Abstract;
using Soenneker.Utils.File.Download.Registrars;

namespace Soenneker.Runners.ZipCode;

/// <summary>
/// Console type startup
/// </summary>
public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    public static void ConfigureServices(IServiceCollection services)
    {
        SetupIoC(services);
    }

    public static IServiceCollection SetupIoC(IServiceCollection services)
    {
        services.AddHostedService<ConsoleHostedService>()
                .AddScoped<IExcelFileReaderUtil, ExcelFileReaderUtil>()
                .AddScoped<IUspsDownloadUtil, UspsDownloadUtil>()
                .AddFileDownloadUtilAsScoped()
                .AddRunnersManagerAsScoped();

        return services;
    }
}