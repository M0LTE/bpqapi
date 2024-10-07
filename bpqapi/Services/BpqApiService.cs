using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace bpqapi.Services;

public class BpqApiService(HttpClient httpClient, IOptions<BpqApiOptions> options)
{
    public async Task<BpqGetAccessTokenResponse> GetToken()
    {
        return await httpClient.GetFromJsonAsync<BpqGetAccessTokenResponse>(new Uri(options.Value.Uri, "api/request_token"));
    }

    private async Task<HttpResponseMessage> Get(string relativeUri, string accessToken)
    {
        var requestMsg = new HttpRequestMessage(HttpMethod.Get, new Uri(options.Value.Uri, relativeUri));
        requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.SendAsync(requestMsg);
        return response;
    }

    public async Task<MHeard[]> GetMheard(string accessToken, string port)
    {
        var response = await Get("api/mheardport?" + port, accessToken);
        response.EnsureSuccessStatusCode();

        var str = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<BpqApiMheardElement[]>($"[{str}]")!;

        return result.Select(r=>new MHeard(r)).ToArray();
    }

    public async Task<Port[]> GetPorts(string accessToken)
    {
        var response = await Get("api/ports", accessToken);

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
        var obj = await response.Content.ReadFromJsonAsync<BpqGetPortsResponse>();

        return obj.Ports.ToArray();
    }
}

internal readonly record struct BpqGetPortsResponse
{
    public Port[] Ports { get; init; }
}

public readonly record struct Port
{
    public string Id { get; init; }
    public string Driver { get; init; }
    public int Number { get; init; }
    public string State { get; init; }
}

public readonly record struct MHeard
{
    internal MHeard(BpqApiMheardElement r) : this()
    {
        Callsign = r.Callsign;
        Packets = r.Packets;

        // 2024-3-6 17:14:01
        LastHeard = DateTime.ParseExact(r.LastHeard, "yyyy-M-d HH:mm:ss", CultureInfo.InvariantCulture);
    }

    public string Callsign { get; init; }
    public int Packets { get; init; }
    public DateTime LastHeard { get; init; }
}

internal readonly record struct BpqApiMheardElement
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

public readonly record struct BpqGetAccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("scope")]
    public string Scope { get; init; }
}