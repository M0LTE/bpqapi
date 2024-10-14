using bpqapi.Controllers;
using bpqapi.Models.BpqApi;
using Microsoft.Extensions.Options;
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

    public async Task<NativeMheardElement[]> GetMheard(string accessToken, int port)
    {
        var response = await Get("api/mheardport?" + port, accessToken);
        response.EnsureSuccessStatusCode();

        var str = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<NativeMheardElement[]>($"[{str}]")!; // fix up mistake

        return result.ToArray();
    }

    public async Task<NativeGetPortsResponse> GetPorts(string accessToken)
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
        return await response.Content.ReadFromJsonAsync<NativeGetPortsResponse>();
    }

    public async Task<NativeGetNodesResponse> GetNodes(string accessToken)
    {
        var response = await Get("api/nodes", accessToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NativeGetNodesResponse>();
    }

    public async Task<NativeGetUsersResponse> GetUsers(string accessToken)
    {
        var response = await Get("api/users", accessToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        json = json.Replace("\"Call\",", "\"Call\":"); // fix up mistake

        return JsonSerializer.Deserialize<NativeGetUsersResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<NativeGetInfoResponse> GetInfo(string accessToken)
    {
        var response = await Get("api/info", accessToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NativeGetInfoResponse>();
    }

    public async Task<NativeGetLinksResponse> GetLinks(string accessToken)
    {
        var response = await Get("api/links", accessToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NativeGetLinksResponse>();
    }
}
