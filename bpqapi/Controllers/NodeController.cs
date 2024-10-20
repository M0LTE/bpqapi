using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace bpqapi.Controllers;

[Route("node")]
public class NodeController(BpqNativeApiService bpqApiService, BpqTelnetClient bpqTelnetClient) : ControllerBase
{
    private async Task<string> GetToken() => (await bpqApiService.RequestLegacyToken()).AccessToken;

    [HttpGet("info")]
    [ProducesResponseType(200, Type = typeof(GetInfoResponse))]
    public async Task<IActionResult> Info()
    {
        var data = await bpqApiService.GetInfo(await GetToken());
        return Ok(new GetInfoResponse
        {
            Alias = data.Info.Alias,
            Locator = data.Info.Locator,
            NodeCall = data.Info.NodeCall,
            Software = "BPQ",
            Version = data.Info.Version
        });
    }

    [HttpGet("mheard")]
    [ProducesResponseType(200, Type = typeof(MHeardMultiportDetails[]))]
    public async Task<IActionResult> MHeard()
    {
        var token = await GetToken();
        var ports = await bpqApiService.GetPorts(token);

        var results = new List<MHeardMultiportDetails>();
        foreach (var port in ports.Ports)
        {
            var data = await bpqApiService.GetMheard(token, port.Number);
            results.AddRange(data.Select(item => new MHeardMultiportDetails
            {
                Callsign = item.Callsign,
                LastHeard = DateTime.ParseExact(item.LastHeard, "yyyy-M-d HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                Packets = item.Packets,
                Port = port.Number
            }));
        }

        return Ok(results.OrderByDescending(m=>m.LastHeard));
    }

    [HttpGet("mheard/{portNumber}")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, MHeardMonoportDetails>))]
    public async Task<IActionResult> Mheard(int portNumber)
    {
        var data = await bpqApiService.GetMheard(await GetToken(), portNumber);

        return Ok(data.ToDictionary(item => item.Callsign, item => new MHeardMonoportDetails
        {
            LastHeard = DateTime.ParseExact(item.LastHeard, "yyyy-M-d HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
            Packets = item.Packets
        }));
    }

    [HttpPost("port/{portNum}/ninomode/{ninoMode}")]
    public async Task<IActionResult> SetNinoMode(int portNum, int ninoMode)
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(Resources.AuthError);
        }

        var loginResult = await bpqTelnetClient.Login(header.Value.User, header.Value.Password);
        if (loginResult != TelnetLoginResult.Success)
        {
            return Unauthorized(Resources.LoginError);
        }

        bool result = await bpqTelnetClient.SendKissCommand(portNum, 6, ninoMode);

        return result ? Ok() : StatusCode(500, "Failed to send KISS command");
    }
}

public readonly record struct GetInfoResponse
{
    public string? NodeCall { get; init; }
    public string? Alias { get; init; }
    public string? Locator { get; init; }
    public string? Software { get; init; }
    public string? Version { get; init; }
}

public readonly record struct MHeardMonoportDetails
{
    public DateTime LastHeard { get; init; }
    public int? Packets { get; init; }
}

public readonly record struct MHeardMultiportDetails
{
    public int Port { get; init; }
    public string Callsign { get; init; }
    public DateTime LastHeard { get; init; }
    public int? Packets { get; init; }
}