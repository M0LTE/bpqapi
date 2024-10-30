using bpqapi.Models;
using bpqapi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static bpqapi.Services.NativeV1MailForwardQueueLengthResponse;

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
    }

    [Fact]
    public async Task GetForwardConfig()
    {
        var token = await target.RequestMailToken("telnetuser", "telnetpass");
        var fwdConfig = await target.GetForwardConfig(token.AccessToken);
    }
}