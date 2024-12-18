using System.Collections.Generic;
using System.IO;
using FluentAssertions;
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
    public void GetZipCodesFromXls_should_parse()
    {
       HashSet<string> result = _util.GetZipCodesFromXls(Path.Combine("Resources", "ZIP_Locale_Detail.xls"));
       result.Should().NotBeNullOrEmpty();
    }
}