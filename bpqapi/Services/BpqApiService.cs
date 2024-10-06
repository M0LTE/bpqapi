using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace bpqapi.Services;

public class BpqApiService(HttpClient httpClient, IOptions<BpqApiOptions> options)
{
    public async Task<AccessTokenResponse> GetToken()
    {
        return await httpClient.GetFromJsonAsync<AccessTokenResponse>(new Uri(options.Value.Uri, "api/request_token"));
    }

    public async Task<MHeard[]> GetMheard(string accessToken, string port)
    {
        var requestMsg = new HttpRequestMessage(HttpMethod.Get, new Uri(options.Value.Uri, "api/mheardport?" + port));
        requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(requestMsg);
        response.EnsureSuccessStatusCode();

        var str = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<BpqApiMheard[]>($"[{str}]")!;

        return result.Select(r=>new MHeard(r)).ToArray();
    }
}

public readonly record struct MHeard
{
    public MHeard(BpqApiMheard r) : this()
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

public readonly record struct BpqApiMheard
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
public readonly record struct AccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("scope")]
    public string Scope { get; init; }
}