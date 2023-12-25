using System.Threading.Tasks;

namespace Soenneker.Runners.ZipCode.Utils.Abstract;

public interface IDownloadUtil
{
    ValueTask<string> Download();
}
