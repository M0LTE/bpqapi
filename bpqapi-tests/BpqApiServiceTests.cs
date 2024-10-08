using bpqapi;
using bpqapi.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;

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

        var mheard = await target.GetMheard(token.AccessToken, 2);

        mheard.Should().NotBeEmpty();
    }

    [Fact]
    public async Task TestGetPorts()
    {
        var token = await target.GetToken();
        var response = await target.GetPorts(token.AccessToken);
        response.Ports.Should().HaveCountGreaterThan(0);

        response.Ports.All(p => !string.IsNullOrWhiteSpace(p.Driver)).Should().BeTrue();
        response.Ports.All(p => !string.IsNullOrWhiteSpace(p.Id)).Should().BeTrue();
        response.Ports.All(p => !string.IsNullOrWhiteSpace(p.State)).Should().BeTrue();
        response.Ports.All(p => p.Number != 0).Should().BeTrue();
    }
}

internal class Options : IOptions<BpqApiOptions>
{
    public BpqApiOptions Value => new() { Uri = new Uri("http://gb7rdg-node:8008") };
}