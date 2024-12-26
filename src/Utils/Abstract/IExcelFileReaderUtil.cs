using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Runners.ZipCode.Utils.Abstract;

public interface IExcelFileReaderUtil
{
    ValueTask<string> CreateZipCodesFromXls(string path, CancellationToken cancellationToken = default);
}