﻿using System.Threading.Tasks;
using FluentAssertions;
using Soenneker.Facts.Local;
using Soenneker.Runners.ZipCode.Utils.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Runners.ZipCode.Tests.Utils;

[Collection("Collection")]
public class UspsDownloadUtilTests : FixturedUnitTest
{
    private readonly IUspsDownloadUtil _util;

    public UspsDownloadUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IUspsDownloadUtil>();
    }

    [LocalFact]
    public async Task Download_should_download()
    {
        string result = await _util.Download();
        result.Should().NotBeNullOrEmpty();
    }
}