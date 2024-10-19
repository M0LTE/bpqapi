using bpqapi;
using bpqapi.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.Net;

namespace bpqapi_tests;

public class BpqApiServiceTests
{
    private BpqNativeApiService target;

    public BpqApiServiceTests()
    {
        target = new BpqNativeApiService(new HttpClient(), new Options(new Uri("http://localhost")));
    }

    [Fact]
    public async Task TestLogin()
    {
        var token = await target.RequestLegacyToken();
        token.AccessToken.Should().NotBeNullOrWhiteSpace();
        token.ExpiresIn.Should().BeGreaterThan(0);
        token.Scope.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task TestMheard()
    {
        var token = await target.RequestLegacyToken();

        var mheard = await target.GetMheard(token.AccessToken, 2);

        mheard.Should().NotBeEmpty();
    }

    [Fact]
    public async Task TestGetPorts()
    {
        var token = await target.RequestLegacyToken();
        var response = await target.GetPorts(token.AccessToken);
        response.Ports.Should().HaveCountGreaterThan(0);

        response.Ports.All(p => !string.IsNullOrWhiteSpace(p.Driver)).Should().BeTrue();
        response.Ports.All(p => !string.IsNullOrWhiteSpace(p.Id)).Should().BeTrue();
        response.Ports.All(p => !string.IsNullOrWhiteSpace(p.State)).Should().BeTrue();
        response.Ports.All(p => p.Number != 0).Should().BeTrue();
    }

    [Fact]
    public async Task TestSendMail()
    {
        var bpqUiService = new BpqUiService(new Options(new Uri("http://gb7rdg-node:8008")), new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }));
        await bpqUiService.SendWebmail("m0lte", "xxx", new bpqapi.Models.SendMailEntity
        {
            To = "test",
            Bid = "",
            Body = "test",
            Subject = "test23467",
            Type = 'P'
        });
    }
}

internal class Options(Uri uri, int? telnetTcpPort = null, string? ctext = null) : IOptions<BpqApiOptions>
{
    public BpqApiOptions Value => new() { Uri = uri, TelnetTcpPort = telnetTcpPort, Ctext = ctext };
}