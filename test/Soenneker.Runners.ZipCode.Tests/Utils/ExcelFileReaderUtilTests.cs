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

    [Test]
    public async ValueTask GetZipCodesFromXls_should_parse()
    {
        string result = await _util.CreateZipCodesFromXls(Path.Combine("Resources", "ZIP_Locale_Detail.xls"), System.Threading.CancellationToken.None);
        result.Should().NotBeNull();
    }

    [Test]
    public async ValueTask GetZipCodesFromXlsx_finds_column_and_preserves_leading_zeroes()
    {
        string result = await _util.CreateZipCodesFromXls(Path.Combine("Resources", "ZIP_Locale_Detail.xlsx"));
        try
        {
            string[] codes = await File.ReadAllLinesAsync(result);
            codes.Should().BeEquivalentTo("00501", "90210");
        }
        finally
        {
            File.Delete(result);
        }
    }
}

