using bpqapi.Models;
using bpqapi.Parsers;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using static System.Net.WebRequestMethods;

namespace bpqapi.Services;

public class BpqUiService(IOptions<BpqApiOptions> options, HttpClient httpClient)
{
    private readonly BpqApiOptions options = options.Value;

    public async Task<ForwardingStation[]> GetMailForwardingPartners(string sysopUser, string password)
    {
        var token = await MailManagementAuth(sysopUser, password);

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

    public async Task<ForwardingOptions> GetForwardingOptions(string sysopUser, string password)
    {
        // get options from http://gb7rdg-node:8008/Mail/FWD?M0000FAB73A83

        var token = await MailManagementAuth(sysopUser, password);

        var pageResponse = await httpClient.GetAsync(new Uri(options.Uri, $"Mail/FWD?{token}"));

        return MailForwardingPageParser.ParseOptions(await pageResponse.Content.ReadAsStringAsync()).EnsureSuccess();
    }

    public async Task<string> MailManagementAuth(string user, string password)
    {
        var authResponse = await httpClient.PostAsync(new Uri(options.Uri, "Mail/Signon?Mail"), new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["user"] = user,
            ["password"] = password
        }));

        authResponse.EnsureSuccessStatusCode();

        var result = MailManagementSignonResponseParser.Parse(await authResponse.Content.ReadAsStringAsync());

        if (result.Success)
        {
            return result.Value;
        }
        else
        {
            throw new LoginFailedException(); // probably...
        }
    }

    /// <summary>
    /// The signon response comes back with a list of mail items and a token to use for further requests.
    /// </summary>
    /// <param name="user">Callsign of BBS user</param>
    /// <param name="password">Password for that callsign</param>
    /// <returns></returns>
    public async Task<(string token, MailListEntity[] items)> WebmailAuth(string user, string password)
    {
        var authResponse = await httpClient.PostAsync(new Uri(options.Uri, "WebMail/Signon"), new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["user"] = user,
            ["password"] = password
        }));

        authResponse.EnsureSuccessStatusCode();

        var html = await authResponse.Content.ReadAsStringAsync();

        var parseResult = WebmailListingParser.Parse(html);

        if (parseResult.Success)
        {
            lastUser = user;
            lassPassword = password;
            lastToken = parseResult.Value.token;
            return parseResult.Value;
        }
        else
        {
            var isLoginPage = WebmailListingParser.ParseMaybeLoginPage(html).EnsureSuccess();

            if (isLoginPage)
            {
                throw new LoginFailedException();
            }
            else
            {
                throw new Exception("Unknown error parsing webmail response");
            }
        }
    }

    private string? lastUser, lassPassword, lastToken;

    /// <summary>
    /// All mail of all types
    /// </summary>
    public async Task<MailListEntity[]> GetWebmailAllMailList(string user, string password)
    {
        return await GetMail(user, password, "WMALL");
    }

    /// <summary>
    /// All bulletins
    /// </summary>
    public async Task<MailListEntity[]> GetWebmailBullsList(string user, string password)
    {
        return await GetMail(user, password, "WMB");
    }

    /// <summary>
    /// All personal mail, not just mine
    /// </summary>
    public async Task<MailListEntity[]> GetWebmailPersonalsList(string user, string password)
    {
        return await GetMail(user, password, "WMP");
    }

    /// <summary>
    /// Mail to/from me
    /// </summary>
    public async Task<MailListEntity[]> GetMyMail(string user, string password)
    {
        return await GetMail(user, password, "WMMine");
    }

    /// <summary>
    /// Mail from me
    /// </summary>
    public async Task<MailListEntity[]> GetWebmailSentMail(string user, string password)
    {
        return await GetMail(user, password, "WMfromMe");
    }

    /// <summary>
    /// Mail to me
    /// </summary>
    public async Task<MailListEntity[]> GetWebmailInbox(string user, string password)
    {
        return await GetMail(user, password, "WMtoMe");
    }

    private async Task<MailListEntity[]> GetMail(string user, string password, string path)
    {
        // http://gb7rdg-node:8008/WebMail/WMfromMe?W3B8745EB
        await AssureToken(user, password);
        var html = await httpClient.GetStringAsync(new Uri(options.Uri, $"WebMail/{path}?{lastToken}"));
        var (_, mail) = WebmailListingParser.Parse(html).EnsureSuccess();
        return mail;
    }

    private async Task AssureToken(string user, string password)
    {
        if (lastUser != user || lassPassword != password || lastToken == null)
        {
            await WebmailAuth(user, password);
        }
    }

    public async Task<MailEntity?> GetWebmailItem(string user, string password, int id)
    {
        await AssureToken(user, password);

        // http://gb7rdg-node:8008/WebMail/WM?W3B8745EB&6925

        var html = await httpClient.GetStringAsync(new Uri(options.Uri, $"WebMail/WM?{lastToken}&{id}"));
        var mail = WebmailParser.Parse(html).EnsureSuccess();
        return mail;
    }
}

public class LoginFailedException : Exception
{
    public LoginFailedException() : base("Login failed") { }
}