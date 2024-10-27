using bpqapi.Models;
using bpqapi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace bpqapi_tests;

public class V1MailApiIntegrationTests
{
    private BpqNativeApiService target;

    public V1MailApiIntegrationTests()
    {
        target = new BpqNativeApiService(new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }), new Options(new Uri("http://mainpc-debian-wsl:8008/")));
    }

    [Fact]
    public async Task GetMail()
    {
        var token = await target.RequestMailToken("telnetuser", "telnetpass");
        token.AccessToken.Should().NotBeNullOrWhiteSpace();
        token.ExpiresAt.Should().BeGreaterThan(DateTimeOffset.Now.ToUnixTimeSeconds());
        token.Scope.Should().NotBeNullOrWhiteSpace();

        var mail = await target.GetMessagesV1(token.AccessToken);
        mail.Messages.Should().NotBeNullOrEmpty();

        var fwdConfig = await target.GetForwardConfig(token.AccessToken);

        var dict = new Dictionary<string, NativeV1MailForwardConfigResponse.ForwardingConfig>();

        foreach (var station in fwdConfig.Config)
        {
            var call = station.Keys.Single();
            NativeV1MailForwardConfigResponse.ForwardingConfig config = station[call];

            dict.Add(call, config);
        }

        var ideal = JsonSerializer.Serialize(dict, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });

        Debugger.Break();
    }
}
