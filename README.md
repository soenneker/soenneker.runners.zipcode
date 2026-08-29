[![](https://img.shields.io/github/actions/workflow/status/soenneker/Soenneker.Runners.ZipCode/build-and-test.yml?style=for-the-badge)](https://github.com/soenneker/Soenneker.Runners.ZipCode/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/Soenneker.Runners.ZipCode/daily-automatic-update.yml?style=for-the-badge&label=Daily%20Update)](https://github.com/soenneker/Soenneker.Runners.ZipCode/actions/workflows/daily-automatic-update.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/Soenneker.Runners.ZipCode/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/Soenneker.Runners.ZipCode/actions/workflows/codeql.yml)

# Soenneker.Runners.ZipCode

Defines the excel file reader util contract.

> This is an automation runner, not a package intended for application consumption.

## What the runner does

- `IExcelFileReaderUtil.CreateZipCodesFromXls(path, cancellationToken)` — Creates zip codes from xls.
- `IUspsDownloadUtil.Download(cancellationToken)` — Downloads usps Download.
- `IUspsDownloadUtil.GetDateFromHtml(html, cancellationToken)` — Gets date from html.
- `IUspsDownloadUtil.GetLastUpdatedDateTime(cancellationToken)` — Gets last updated date time.
- `IUspsDownloadUtil.GetDirectory(cancellationToken)` — Gets directory.

## What you get

- `IExcelFileReaderUtil` — Defines the excel file reader util contract.
- `IUspsDownloadUtil` — Defines the usps download util contract.
- `Constants` — Represents the constants.
- `ConsoleHostedService` — Represents the console hosted service.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `IUspsDownloadUtil.Download(cancellationToken)` | Downloads usps Download. | A task whose result is the text returned by download. |
| `ConsoleHostedService.StartAsync(cancellationToken)` | Starts the Console Hosted Service and begins its background work. | A task that completes after the Console Hosted Service has started. |
| `ConsoleHostedService.StopAsync(cancellationToken)` | Stops the Console Hosted Service and waits for its background work to finish. | A task that completes after the Console Hosted Service has stopped. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
