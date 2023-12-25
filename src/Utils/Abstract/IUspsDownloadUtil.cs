using System.Threading.Tasks;

namespace Soenneker.Runners.ZipCode.Utils.Abstract;

public interface IUspsDownloadUtil
{
    ValueTask<string> Download();
}