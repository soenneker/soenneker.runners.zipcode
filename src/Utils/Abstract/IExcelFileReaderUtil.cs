using System.Collections.Generic;

namespace Soenneker.Runners.ZipCode.Utils.Abstract;

public interface IExcelFileReaderUtil
{
    HashSet<string> GetZipCodesFromXls(string path);
}