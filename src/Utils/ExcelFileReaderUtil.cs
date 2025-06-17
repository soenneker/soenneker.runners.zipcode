using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.Extensions.Logging;
using Soenneker.Runners.ZipCode.Utils.Abstract;
using Soenneker.Utils.File.Abstract;
using Soenneker.Utils.Path.Abstract;

namespace Soenneker.Runners.ZipCode.Utils;

///<inheritdoc cref="IExcelFileReaderUtil"/>
public class ExcelFileReaderUtil : IExcelFileReaderUtil
{
    private readonly ILogger<ExcelFileReaderUtil> _logger;
    private readonly IFileUtil _fileUtil;
    private readonly IPathUtil _pathUtil;

    public ExcelFileReaderUtil(ILogger<ExcelFileReaderUtil> logger, IFileUtil fileUtil, IPathUtil pathUtil)
    {
        _logger = logger;
        _fileUtil = fileUtil;
        _pathUtil = pathUtil;

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }
    
    public async ValueTask<string> CreateZipCodesFromXls(string path, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving unique Zip codes from XLS...");

        var result = new HashSet<string>();

        using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            // Use ExcelDataReader to read the XLS file
            using (IExcelDataReader? reader = ExcelReaderFactory.CreateReader(stream))
            {
                // The result can be read into a DataSet
                DataSet dataSet = reader.AsDataSet();

                // Alternatively, you can loop through the rows and columns
                // in each DataTable in the DataSet
                foreach (DataTable table in dataSet.Tables)
                {
                    if (table.TableName != "ZIP_DETAIL") 
                        continue;
                    
                    var count = 0;

                    foreach (DataRow row in table.Rows)
                    {
                        count++;

                        if (count == 1)
                        {
                            if (row[4].ToString() != "DELIVERY ZIPCODE")
                                throw new Exception("The format of the file has changed, address immediately");

                            continue;
                        }

                        var deliveryZip = row[4].ToString();

                        result.Add(deliveryZip!);
                    }

                    _logger.LogDebug("Completed parsing Zip codes");

                    string linesPath = await _pathUtil.GetRandomTempFilePath("txt", cancellationToken);

                    await _fileUtil.WriteAllLines(linesPath, result, true, cancellationToken);

                    return linesPath;
                }

                throw new Exception("Unable to parse data file, address immediately");
            }
        }
    }
}