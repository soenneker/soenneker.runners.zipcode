using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Soenneker.Managers.Runners.Abstract;
using Soenneker.Runners.ZipCode.Utils.Abstract;

namespace Soenneker.Runners.ZipCode;

/// <summary>
/// Represents the console hosted service.
/// </summary>
public class ConsoleHostedService : IHostedService
{
    private readonly ILogger<ConsoleHostedService> _logger;

    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IRunnersManager _runnersManager;
    private readonly IExcelFileReaderUtil _excelFileReaderUtil;
    private readonly IUspsDownloadUtil _uspsDownloadUtil;

    private int? _exitCode;

    public ConsoleHostedService(ILogger<ConsoleHostedService> logger, IHostApplicationLifetime appLifetime, IExcelFileReaderUtil excelFileReaderUtil,
        IUspsDownloadUtil uspsDownloadUtil, IRunnersManager runnersManager)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _excelFileReaderUtil = excelFileReaderUtil;
        _uspsDownloadUtil = uspsDownloadUtil;
        _runnersManager = runnersManager;
    }

    /// <summary>
    /// Executes the start async operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                _logger.LogInformation("Running console hosted service ...");

                try
                {
                    string fileName = await _uspsDownloadUtil.Download(cancellationToken);
                    string filePath = await _excelFileReaderUtil.CreateZipCodesFromXls(fileName, cancellationToken);

                    await _runnersManager.PushIfChangesNeeded(filePath, Constants.FileName, Constants.Library,
                        $"https://github.com/soenneker/{Constants.Library}", false, cancellationToken);

                    _logger.LogInformation("Complete!");

                    _exitCode = 0;
                }
                catch (Exception e)
                {
                    if (Debugger.IsAttached)
                        Debugger.Break();

                    _logger.LogError(e, "Unhandled exception");

                    await Task.Delay(2000, cancellationToken);
                    _exitCode = 1;
                }
                finally
                {
                    // Stop the application once the work is done
                    _appLifetime.StopApplication();
                }
            }, cancellationToken);
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes the stop async operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Exiting with return code: {exitCode}", _exitCode);

        // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
        return Task.CompletedTask;
    }
}