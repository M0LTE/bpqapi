using bpqapi;
using bpqapi.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bpqapi_tests;

public class BpqApiServiceTests
{
    private BpqApiService target;

    public BpqApiServiceTests()
    {
        target = new BpqApiService(new HttpClient(), new Options());
    }

    [Fact]
    public async Task TestLogin()
    {
        var token = await target.GetToken();
        token.AccessToken.Should().NotBeNullOrWhiteSpace();
        token.ExpiresIn.Should().BeGreaterThan(0);
        token.Scope.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task TestMheard()
    {
        var token = await target.GetToken();

        var mheard = await target.GetMheard(token.AccessToken, "2");

        mheard.Should().NotBeEmpty();
    }
}

internal class Options : IOptions<BpqApiOptions>
{
    public BpqApiOptions Value => new BpqApiOptions { Uri = new Uri("http://gb7rdg-node:8008") };
}