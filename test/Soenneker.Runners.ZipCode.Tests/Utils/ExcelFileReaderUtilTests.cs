using System.IO;
using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Runners.ZipCode.Utils.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Runners.ZipCode.Tests.Utils;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class ExcelFileReaderUtilTests : HostedUnitTest
{
    private readonly IExcelFileReaderUtil _util;

    public ExcelFileReaderUtilTests(Host host) : base(host)
    {
        _util = Resolve<IExcelFileReaderUtil>();
    }

    [Skip("Manual")]
    //[LocalOnly]
    public async ValueTask GetZipCodesFromXls_should_parse()
    {
        string result = await _util.CreateZipCodesFromXls(Path.Combine("Resources", "ZIP_Locale_Detail.xls"), System.Threading.CancellationToken.None);
        result.Should().NotBeNull();
    }
}

