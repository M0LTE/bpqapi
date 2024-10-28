using bpqapi.Models;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace bpqapi.Controllers;

[Route("node")]
public class NodeController(BpqNativeApiService nativeApiService, BpqTelnetClient bpqTelnetClient) : ControllerBase
{
    private async Task<string> GetToken() => (await nativeApiService.RequestLegacyToken()).AccessToken;

    [HttpGet("info")]
    [ProducesResponseType(200, Type = typeof(GetInfoResponse))]
    public async Task<IActionResult> Info()
    {
        var data = await nativeApiService.GetInfo(await GetToken());
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
        var ports = await nativeApiService.GetPorts(token);

        var results = new List<MHeardMultiportDetails>();
        foreach (var port in ports.Ports)
        {
            var data = await nativeApiService.GetMheard(token, port.Number);
            results.AddRange(data.Mheard.Select(item => new MHeardMultiportDetails
            {
                Callsign = item.Callsign,
                LastHeard = DateTime.ParseExact(item.LastHeard, "yyyy-M-d HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                Packets = item.Packets,
                Port = port.Number
            }));
        }

        return Ok(results.OrderByDescending(m => m.LastHeard));
    }

    [HttpGet("mheard/{portNumber}")]
    [ProducesResponseType(200, Type = typeof(Dictionary<string, MHeardMonoportDetails>))]
    public async Task<IActionResult> Mheard(int portNumber)
    {
        var data = await nativeApiService.GetMheard(await GetToken(), portNumber);

        return Ok(data.Mheard.ToDictionary(item => item.Callsign, item => new MHeardMonoportDetails
        {
            LastHeard = DateTime.ParseExact(item.LastHeard, "yyyy-M-d HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
            Packets = item.Packets
        }));
    }

    [HttpPost("port/{portNum}/ninomode")]
    [ProducesResponseType<object>(200, "application/json")]
    public async Task<IActionResult> SetNinoMode(int portNum,
        [FromQuery] int? id,
        [FromQuery] NinoModeEnum? name,
        [FromQuery] NinoModeUsage? usage,
        [FromQuery] bool persist = false)
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(Resources.AuthError);
        }

        var count = (id == null ? 0 : 1)
            + (name == null ? 0 : 1)
            + (usage == null ? 0 : 1);

        if (count == 0)
        {
            return BadRequest($"{nameof(id)}, {nameof(name)}, or {nameof(usage)} must be specified");
        }
        else if (count > 1)
        {
            return BadRequest($"Only one of {nameof(id)}, {nameof(name)}, or {nameof(usage)} must be specified");
        }

        var loginResult = await bpqTelnetClient.Login(header.Value.User, header.Value.Password);
        if (loginResult != TelnetLoginResult.Success)
        {
            return Unauthorized(Resources.LoginError);
        }

        int value = 0;

        if (id != null)
        {
            value = id.Value;
        }
        else if (name != null)
        {
            value = (int)name!.Value;
        }
        else if (usage != null)
        {
            value = (int)usage!.Value;
        }

        if (!persist)
        {
            value += 16;
        }

        bool result = await bpqTelnetClient.SendKissCommand(portNum, 6, value);

        return result ? Ok(new { sentValue = value }) : StatusCode(500, "Failed to send KISS command");
    }
}

public enum NinoModeEnum
{
    Gfsk9600 = 0,
    C4Fsk19200Il2pc = 1,
    Gfsk9600Il2pc = 2,
    C4Fsk9600Il2pc = 3,
    Gfsk4800Il2pc = 4,
    Qpsk3600Il2pc = 5,
    Afsk1200 = 6,
    Afsk1200Il2p = 7,
    Bpsk300Il2pc = 8,
    Qpsk600Il2pc = 9,
    Bpsk1200Il2pc = 10,
    Qpsk2400Il2pc = 11,
    Afsk300 = 12,
    Afsk300Il2p = 13,
    Afsk300Il2pc = 14,
}

public enum NinoModeUsage
{
    Nvis40mUkSlot1 = 14,
    Nvis40mUkSlot3 = 8,
    ClassicHf = 12,
    Aprs = 6,
    G3ruh9k6 = 0,
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