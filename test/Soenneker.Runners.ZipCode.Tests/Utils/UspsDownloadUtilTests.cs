using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Runners.ZipCode.Utils.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Runners.ZipCode.Tests.Utils;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class UspsDownloadUtilTests : HostedUnitTest
{
    private readonly IUspsDownloadUtil _util;

    public UspsDownloadUtilTests(Host host) : base(host)
    {
        _util = Resolve<IUspsDownloadUtil>();
    }

    [Test]
    public void Default()
    {
    }

    //[LocalOnly]
    [Skip("Manual")]
    public async Task Download_should_download()
    {
        string result = await _util.Download();
        result.Should()
              .NotBeNullOrEmpty();
    }
}
