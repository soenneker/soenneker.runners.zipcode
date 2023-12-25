using System.Collections.Generic;

namespace Soenneker.Data.ZipCode.Utils.Abstract;

public interface IExcelFileReaderUtil
{
    HashSet<string> GetZipCodesFromXls(string path);
}