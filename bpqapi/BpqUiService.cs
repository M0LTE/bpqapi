using bpqapi.Models;
using bpqapi.Parsers;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;

namespace bpqapi;

public class BpqUiService(IOptions<BpqApiOptions> options, HttpClient httpClient)
{
    private readonly BpqApiOptions options = options.Value;

    public async Task<ForwardingStation[]> GetMailForwardingPartners()
    {
        var token = await Authenticate();

        // post http://gb7rdg-node:8008/Mail/FwdList.txt?M0000FB748BFF

        var listResponse = await httpClient.PostAsync(new Uri(options.Uri, $"Mail/FwdList.txt?{token}"), new FormUrlEncodedContent([]));
        listResponse.EnsureSuccessStatusCode();
        var partners = MailForwardingPageParser.ParsePartnerList(await listResponse.Content.ReadAsStringAsync()).EnsureSuccess();

        // post http://gb7rdg-node:8008/Mail/FwdDetails?M0000FB748BFF

        var partnerDetails = new List<ForwardingStation>();
        foreach (var partner in partners)
        {
            var partnerResponse = await httpClient.PostAsync(new Uri(options.Uri, $"Mail/FwdDetails?{token}"), new ByteArrayContent(Encoding.UTF8.GetBytes(partner)));
            partnerResponse.EnsureSuccessStatusCode();
            var partnerDetail = MailForwardingPageParser.ParseStationResponse(await partnerResponse.Content.ReadAsStringAsync()).EnsureSuccess();
            partnerDetails.Add(partnerDetail);
        }

        return [.. partnerDetails];
    }

    public async Task<ForwardingOptions> GetForwardingOptions()
    {
        // get options from http://gb7rdg-node:8008/Mail/FWD?M0000FAB73A83

        var token = await Authenticate();

        var pageResponse = await httpClient.GetAsync(new Uri(options.Uri, $"Mail/FWD?{token}"));

        return MailForwardingPageParser.ParseOptions(await pageResponse.Content.ReadAsStringAsync()).EnsureSuccess();
    }

    private async Task<string> Authenticate()
    {
        var authResponse = await httpClient.PostAsync(new Uri(options.Uri, "Mail/Signon?Mail"), new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["user"] = options.SysopUsername,
            ["password"] = options.SysopPassword
        }));

        authResponse.EnsureSuccessStatusCode();

        return MailSignonResponseParser.Parse(await authResponse.Content.ReadAsStringAsync()).EnsureSuccess();
    }
}
