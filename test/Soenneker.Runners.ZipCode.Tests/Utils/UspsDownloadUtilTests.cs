using System;
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

    [Test]
    public async ValueTask GetDateFromHtml_skips_breadcrumbs()
    {
        DateTime? result = await _util.GetDateFromHtml(
            "<div class='breadcrumbs mb-2'>Home / ZIP Locale Detail</div><div class='mb-2'>October 01, 2026</div>");

        result.Should().Be(new DateTime(2026, 10, 1));
    }

    //[LocalOnly]
    [Skip("Manual")]
    public async ValueTask Download_should_download()
    {
        string result = await _util.Download();
        result.Should()
              .NotBeNullOrEmpty();
    }
}
