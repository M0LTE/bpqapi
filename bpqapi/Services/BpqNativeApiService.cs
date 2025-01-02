using bpqapi.Models.BpqApi;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace bpqapi.Services;

public class BpqNativeApiService(HttpClient httpClient, IOptions<BpqApiOptions> options)
{
    public async Task<BpqGetAccessTokenResponse> RequestLegacyToken()
    {
        return await httpClient.GetFromJsonAsync<BpqGetAccessTokenResponse>(new Uri(options.Value.Uri, "api/request_token"));
    }

    private async Task<HttpResponseMessage> Get(string relativeUri, string bearerHeaderValue)
    {
        var requestMsg = new HttpRequestMessage(HttpMethod.Get, new Uri(options.Value.Uri, relativeUri));
        requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerHeaderValue);
        var response = await httpClient.SendAsync(requestMsg);
        return response;
    }

    private async Task<T> Get<T>(string relativeUri, string? bearerHeaderValue)
    {
        var requestMsg = new HttpRequestMessage(HttpMethod.Get, new Uri(options.Value.Uri, relativeUri));
        if (!string.IsNullOrWhiteSpace(bearerHeaderValue))
        {
            requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerHeaderValue);
        }
        var response = await httpClient.SendAsync(requestMsg);
        try
        {
            var value = await response.Content.ReadFromJsonAsync<T>();
            return value ?? throw new Exception("Could not deserialise response");
        }
        catch (Exception ex)
        {
            if (!Debugger.IsAttached)
            {
                throw;
            }

            var str = await response.Content.ReadAsStringAsync();
            Debugger.Break();
            throw new NotImplementedException();
        }
    }

    public async Task<NativeMheardResponse> GetMheard(string legacyAccessToken, int portNumber)
    {
        var response = await Get("api/mheard/" + portNumber, legacyAccessToken);
        response.EnsureSuccessStatusCode();
        var str = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<NativeMheardResponse>(str);
        return result;
    }

    public readonly record struct NativeMheardResponse
    {
        [JsonPropertyName("mheard")]
        public NativeMheardResponseElement[] Mheard { get; init; }
        public readonly record struct NativeMheardResponseElement
        {
            [JsonPropertyName("callSign")]
            public string Callsign { get; init; }
            [JsonPropertyName("port")]
            public string Port { get; init; }
            [JsonPropertyName("packets")]
            public int Packets { get; init; }
            [JsonPropertyName("lastHeard")]
            public string LastHeard { get; init; }
        }
    }

    public async Task<NativeGetPortsResponse> GetPorts(string legacyAccessToken)
    {
        var response = await Get("api/ports", legacyAccessToken);

        /*
{"ports":[
 {"ID":"144.950 FM QPSK3600 IL2Pc", "Driver":"ASYNC", "Number":1,"State":"Open"},
 {"ID":"432.625 GFSK9600 IL2P+CRC", "Driver":"ASYNC", "Number":2,"State":"Open"},
 {"ID":"7052.75kHz BPSK300 IL2P+CRC", "Driver":"ASYNC", "Number":3,"State":"Open"},
 {"ID":"Telnet", "Driver":"TELNET", "Number":7,"State":"Open"},
 {"ID":"OARC-AXIP", "Driver":"BPQAXIP", "Number":8,"State":"Open"}
]}
         */

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NativeGetPortsResponse>();
    }

    public async Task<NativeGetNodesResponse> GetNodes(string legacyAccessToken)
    {
        var response = await Get("api/nodes", legacyAccessToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NativeGetNodesResponse>();
    }

    public async Task<NativeGetUsersResponse> GetUsers(string legacyAccessToken)
    {
        var response = await Get("api/users", legacyAccessToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        json = json.Replace("\"Call\",", "\"Call\":"); // fix up mistake

        return JsonSerializer.Deserialize<NativeGetUsersResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<NativeGetInfoResponse> GetInfo(string legacyAccessToken)
    {
        var response = await Get("api/info", legacyAccessToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NativeGetInfoResponse>();
    }

    public async Task<NativeGetLinksResponse> GetLinks(string legacyAccessToken)
    {
        var response = await Get("api/links", legacyAccessToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NativeGetLinksResponse>();
    }

    public async Task<NativeV1MailLoginResponse> RequestMailToken(string user, string password)
    {
        var response = await httpClient.GetAsync(new Uri(options.Value.Uri, $"api/v1/mail/login?{user}&{password}"));

        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        try
        {
            return JsonSerializer.Deserialize<NativeV1MailLoginResponse>(content)!;
        }
        catch(Exception ex)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            throw;
        }

        //var response = await httpClient.GetFromJsonAsync<NativeV1MailLoginResponse>(
            //new Uri(options.Value.Uri, $"api/v1/mail/login?{user}&{password}"));
        //return response;
    }

    public async Task<NativeV1MailMessagesResponse> GetMessagesV1(string mailAccessToken)
    {
        var response = await Get<NativeV1MailMessagesResponse>("api/v1/mail/msgs", mailAccessToken);
        return response;
    }

    public async Task<NativeV1MailForwardQueueLengthResponse> GetQueueLengths(string mailAccessToken)
    {
        var response = await Get<NativeV1MailForwardQueueLengthResponse>("api/v1/mail/FwdQLen", mailAccessToken);
        return response;
    }

    public async Task<NativeMailForwardConfigV1Response> GetForwardConfig(string mailAccessToken)
    {
        var response = await Get<NativeMailForwardConfigV1Response>("api/v1/mail/FwdConfig", mailAccessToken);
        return response;
    }
}

public class NativeMailForwardConfigV1Response : Dictionary<string, NativeMailForwardConfigV1Response.NativeForwardingConfigV1>
{
    public readonly record struct NativeForwardingConfigV1
    {
        [JsonPropertyName("queueLength")]
        public int QueueLength { get; init; }

        [JsonPropertyName("to")]
        public string[] To { get; init; }

        [JsonPropertyName("at")]
        public string[] At { get; init; }

        [JsonPropertyName("hrp")]
        public string[] Hrp { get; init; }

        [JsonPropertyName("hrb")]
        public string[] Hrb { get; init; }

        [JsonPropertyName("times")]
        public string[] Times { get; init; }

        [JsonPropertyName("connectScript")]
        public string[] ConnectScript { get; init; }

        [JsonPropertyName("bbsHa")]
        public string BbsHa { get; init; }

        [JsonPropertyName("enableForwarding")]
        public bool EnableForwarding { get; init; }

        [JsonPropertyName("forwardingInterval")]
        public TimeSpan ForwardingInterval { get; init; }

        [JsonPropertyName("requestReverse")]
        public bool RequestReverse { get; init; }

        [JsonPropertyName("reverseInterval")]
        public TimeSpan ReverseInterval { get; init; }

        [JsonPropertyName("sendNewMessagesWithoutWaiting")]
        public bool SendNewMessagesWithoutWaiting { get; init; }

        [JsonPropertyName("fbbBlocked")]
        public bool FbbBlocked { get; init; }

        [JsonPropertyName("maxBlock")]
        public int MaxBlock { get; init; }

        [JsonPropertyName("sendPersonalMailOnly")]
        public bool SendPersonalMailOnly { get; init; }

        [JsonPropertyName("allowBinary")]
        public bool AllowBinary { get; init; }

        [JsonPropertyName("useB1Protocol")]
        public bool UseB1Protocol { get; init; }

        [JsonPropertyName("useB2Protocol")]
        public bool UseB2Protocol { get; init; }

        [JsonPropertyName("incomingConnectTimeout")]
        public TimeSpan IncomingConnectTimeout { get; init; }
    }
}

/*
{"forwardqueuelength":[
{"call": "GB7RDG","queueLength": "0"},
{"call": "M0LTE","queueLength": "0"},
{"call": "TEST","queueLength": "0"}
]}
 */
public readonly record struct NativeV1MailForwardQueueLengthResponse
{
    [JsonPropertyName("forwardqueuelength")]
    public ForwardQueueLengthElement[] Queues { get; init; }

    public record struct ForwardQueueLengthElement
    {
        [JsonPropertyName("call")]
        public string Call { get; init; }
        [JsonPropertyName("queueLength")]
        public string QueueLength { get; init; }
    }
}

public readonly record struct NativeV1MailLoginResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("expires_at")]
    public long ExpiresAt { get; init; }

    [JsonPropertyName("scope")]
    public string Scope { get; init; }
}

public readonly record struct NativeV1MailMessagesResponse
{
    [JsonPropertyName("msgs")]
    public Message[] Messages { get; init; }

    public readonly record struct Message
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }
        [JsonPropertyName("mid")]
        public string Mid { get; init; }
        [JsonPropertyName("rcvd")]
        public long Received { get; init; }
        [JsonPropertyName("type")]
        public char Type { get; init; }
        [JsonPropertyName("status")]
        public char Status { get; init; }
        [JsonPropertyName("to")]
        public string To { get; init; }
        [JsonPropertyName("from")]
        public string From { get; init; }
        [JsonPropertyName("size")]
        public int Size { get; init; }
        [JsonPropertyName("subject")]
        public string Subject { get; init; }
    }
}