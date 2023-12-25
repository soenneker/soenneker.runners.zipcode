using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using Microsoft.Extensions.Logging;
using Soenneker.Data.ZipCode.Utils.Abstract;

namespace Soenneker.Data.ZipCode.Utils;

///<inheritdoc cref="IExcelFileReaderUtil"/>
public class ExcelFileReaderUtil : IExcelFileReaderUtil
{
    private readonly ILogger<ExcelFileReaderUtil> _logger;

    public ExcelFileReaderUtil(ILogger<ExcelFileReaderUtil> logger)
    {
        _logger = logger;

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    }

    public HashSet<string> GetZipCodesFromXls(string path)
    {
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

                    return result;
                }

                throw new Exception("Unable to parse data file, address immediately");
            }
        }
    }
}