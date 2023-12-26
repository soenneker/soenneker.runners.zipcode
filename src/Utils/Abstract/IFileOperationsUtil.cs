using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soenneker.Runners.ZipCode.Utils.Abstract;

public interface IFileOperationsUtil
{
    ValueTask Process(HashSet<string> hashSet);
}
