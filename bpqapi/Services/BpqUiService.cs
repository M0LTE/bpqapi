using bpqapi.Models;
using bpqapi.Parsers;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

namespace bpqapi.Services;

public class BpqUiService(IOptions<BpqApiOptions> options, HttpClient httpClient, ILogger<BpqUiService> logger, BpqMailCache cache)
{
    private readonly BpqApiOptions options = options.Value;

    public async Task<ForwardingStation[]> GetMailForwardingPartners(string sysopUser, string password)
    {
        var token = await MailManagementAuth(sysopUser, password);
        return await GetMailForwardingPartners(token);
    }

    public async Task<ForwardingStation[]> GetMailForwardingPartners(string token)
    {
        // post http://gb7rdg-node:8008/Mail/FwdList.txt?M0000FB748BFF

        var listResponse = await httpClient.PostAsync(new Uri(options.Uri, $"Mail/FwdList.txt?{token}"), new FormUrlEncodedContent([]));
        listResponse.EnsureSuccessStatusCode();
        var partners = MailForwardingPageParser.ParsePartnerList(await listResponse.Content.ReadAsStringAsync()).EnsureSuccess();

        // post http://gb7rdg-node:8008/Mail/FwdDetails?M0000FB748BFF

        var partnerDetails = new List<ForwardingStation>();
        foreach (var partner in partners)
        {
            var detail = await GetForwardingStationDetails(token, partner);
            partnerDetails.Add(detail);
        }

        return [.. partnerDetails];
    }

    public async Task<Dictionary<string, List<int>>> GetQueues(string sysopUser, string sysopPassword)
    {
        var token = await MailManagementAuth(sysopUser, sysopPassword);

        var msg = await httpClient.PostAsync(new Uri(options.Uri, "Mail/MsgInfo.txt?" + token), new FormUrlEncodedContent([]));
        msg.EnsureSuccessStatusCode();
        var messageDetails = (await msg.Content.ReadAsStringAsync()).Split("|", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(w => int.TryParse(w, out _))
            .Select(int.Parse)
            .Select(async messageId => await GetMessageDetails(token, messageId));
        
        Dictionary<string, List<int>> queues = [];

        foreach (var detailsTask in messageDetails)
        {
            var messageDetail = await detailsTask;

            foreach (var station in messageDetail.ForwardingStatus.Where(t => t.Value == MessageDetails.QueueStatus.Pending))
            {
                if (!queues.ContainsKey(station.Key))
                {
                    queues.Add(station.Key, []);
                }

                queues[station.Key].Add(messageDetail.Id);
            }
        }

        return queues;
    }

    private async Task<ForwardingStation> GetForwardingStationDetails(string token, string partner)
    {
        var partnerResponse = await httpClient.PostAsync(new Uri(options.Uri, $"Mail/FwdDetails?{token}"), new ByteArrayContent(Encoding.UTF8.GetBytes(partner)));
        partnerResponse.EnsureSuccessStatusCode();
        var partnerDetail = MailForwardingPageParser.ParseStationResponse(await partnerResponse.Content.ReadAsStringAsync()).EnsureSuccess();
        return partnerDetail;
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
                DebugWriter.LogAndThrow<WebmailListingParser>(html, new Exception("Unknown error parsing webmail response"));
                return default;
            }
        }
    }

    private static string? lastUser, lassPassword, lastToken;

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
            logger.LogInformation("Authenticating {user} for webmail", user);
            await WebmailAuth(user, password);
        }
    }

    public async Task<List<MailEntity>> GetWebmailItems(string user, string password, int[] ids)
    {
        await AssureToken(user, password);

        var result = new List<MailEntity>();
        try
        {
            await GetData(ids, result);
        }
        catch (LoginFailedException)
        {
            lastToken = null;
            result.Clear();
            await GetData(ids, result);
        }

        return result;
    }

    private async Task GetData(int[] ids, List<MailEntity> result)
    {
        foreach (var id in ids)
        {
            result.Add(await GetMail(id));
        }
    }

    private async Task<MailEntity> GetMail(int id)
    {
        if (cache.TryGetValue(id, out var cached))
        {
            return cached!;
        }
        else
        {
            var html = await httpClient.GetStringAsync(new Uri(options.Uri, $"WebMail/WM?{lastToken}&{id}"));
            var result = WebmailParser.Parse(html, logger);

            MailEntity mail;

            if (result.Success)
            {
                mail = result.Value;
            }
            else
            {
                mail = new MailEntity
                {
                    Id = id,
                    Subject = "Error",
                    From = "unknown",
                    To = "unknown",
                    Date = MonthAndDay.UtcToday,
                    DateTime = DateTime.UtcNow,
                    Time = TimeOnly.FromDateTime(DateTime.UtcNow),
                    Body = $"There was an error while parsing this mail with id {id}:\n\n" + result.Exception?.ToString()
                };
            }

            cache.Add(id, mail);
            return mail;
        }
    }

    public async Task SendWebmail(string user, string password, SendMailEntity mail)
    {
        try
        {
            logger.LogInformation("Sending mail {@mail} from {from}", mail, user);

            await AssureToken(user, password);

            logger.LogInformation("Token for {user} assured", user);

            var form = new MultipartFormDataContent()
        {
            { BuildStringContent("To", mail.To), "To" },
            { BuildStringContent("Subj", mail.Subject), "Subj" },
            //{ new ByteArrayContent([]),"myFile[]","dummy.txt"},
            { BuildStringContent("Type", mail.Type.ToString()), "Type" },
            { BuildStringContent("BID", mail.Bid), "BID" },
            //{ new ByteArrayContent([]),"myFile2[]","dummy.txt"},
            //{ new ByteArrayContent([]),"myFile3[]","dummy.txt"},
            { BuildStringContent("Msg", mail.Body), "Msg" },
            { BuildStringContent("Send", "Send"), "Send" },
        };

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(options.Uri, $"WebMail/EMSave?{lastToken}"));
            request.Content = form;

            logger.LogInformation("Sending request to BPQ");
            var listResponse = await httpClient.SendAsync(request);

            var response = await listResponse.Content.ReadAsStringAsync();
            //logger.LogInformation("Received {statusCode} response from BPQ: {response}", (int)listResponse.StatusCode, response);
            logger.LogInformation("Received {statusCode} response from BPQ", (int)listResponse.StatusCode);
            listResponse.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception in BpqUiService.SendWebmail");
            throw;
        }
    }

    private static StringContent BuildStringContent(string name, string content)
    {
        var sc = new StringContent(content, Encoding.UTF8, "text/plain");
        sc.Headers.Remove("Content-Type");
        sc.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = $"\"{name}\"", // quotes are necessary else we crash BPQ
        };
        return sc;
    }

    public async Task<bool> StartForwardingSession(string user, string password, string callsign)
    {
        var token = await MailManagementAuth(user, password);

        // bpq seems to track the last forwarding partner page that was looked at, in order to 
        // know who to start a session with, rather than passing it in the request.
        await GetForwardingStationDetails(token, callsign.ToUpper());

        // POST http://gb7rdg-node:8008/Mail/FWDSave?M0000001360E7

        var postResponse = await httpClient.PostAsync(new Uri(options.Uri, "Mail/FWDSave?" + token), new StringContent("StartForward"));
        return postResponse.IsSuccessStatusCode;
        /*
POST /Mail/FWDSave?M0000001360E7 HTTP/1.1
Host: gb7rdg-node:8008
Connection: keep-alive
Content-Length: 12
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36
Content-Type: text/plain;charset=UTF-8
Accept: x
Origin: http://gb7rdg-node:8008
Referer: http://gb7rdg-node:8008/Mail/FWD?M0000001360E7
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.9

StartForward
         */
    }

    public async Task<MessageDetails> GetMessageDetails(string mailManagementToken, int messageId)
    {
        // http://gb7rdg-node:8008/Mail/MsgDetails?M0000015743C4

        var pageResponse = await httpClient.PostAsync(new Uri(options.Uri, $"Mail/MsgDetails?{mailManagementToken}"), new StringContent(messageId.ToString()));
        pageResponse.EnsureSuccessStatusCode();

        var details = MsgDetailsResponseParser.Parse(messageId, await pageResponse.Content.ReadAsStringAsync()).EnsureSuccess();

        return details;
    }
}

public class LoginFailedException : Exception
{
    public LoginFailedException() : base("Login failed") { }
}