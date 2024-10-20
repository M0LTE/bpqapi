using bpqapi.Services;
using System.Net;

namespace bpqapi_tests;

public class BpqUiServiceIntegrationTests
{
    private readonly BpqUiService target;
    public BpqUiServiceIntegrationTests()
    {
        target = new BpqUiService(new Options(new Uri("http://gb7rdg-node:8008")), new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }));
    }

    [Fact]
    public async Task GetMsgDetails()
    {
        var queues = await target.GetQueues("tf", "xx");

    }
}
