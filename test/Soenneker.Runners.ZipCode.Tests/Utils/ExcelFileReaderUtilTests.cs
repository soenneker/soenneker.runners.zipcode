using System.IO;
using System.Threading.Tasks;
using AwesomeAssertions;
using Soenneker.Runners.ZipCode.Utils.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Runners.ZipCode.Tests.Utils;

[Collection("Collection")]
public class ExcelFileReaderUtilTests : FixturedUnitTest
{
    private readonly IExcelFileReaderUtil _util;

    public ExcelFileReaderUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IExcelFileReaderUtil>();
    }

    [Fact]
    public async ValueTask GetZipCodesFromXls_should_parse()
    {
        string result = await _util.CreateZipCodesFromXls(Path.Combine("Resources", "ZIP_Locale_Detail.xls"), CancellationToken);
        result.Should().NotBeNull();
    }
}