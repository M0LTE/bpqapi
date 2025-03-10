﻿using Aspects.Cache;
using bpqapi.Models;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;
using System.Text;

namespace bpqapi.Controllers;

[ApiController]
[Route("metrics")]
public class MetricsController(BpqUiService bpqUiService) : ControllerBase
{
    private const string AuthError = "Sysop username and password required as basic auth header";
    private const string LoginError = "BPQ rejected that sysop login";

    [HttpGet("line-protocol")]
    public async Task<IActionResult> GetInfluxDbMetrics()
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(AuthError);
        }

        try
        {
            var (partners, dt) = await GetCachedMailPartners(header.Value.User, header.Value.Password);

            /*
             * timestamp is nanoseconds
             * 
    citibike,station_id=4703 eightd_has_available_keys=false,is_installed=1,is_renting=1,is_returning=1,legacy_id="4703",num_bikes_available=6,num_bikes_disabled=2,num_docks_available=26,num_docks_disabled=0,num_ebikes_available=0,station_status="active" 1641505084000000000
    citibike,station_id=4704 eightd_has_available_keys=false,is_installed=1,is_renting=1,is_returning=1,legacy_id="4704",num_bikes_available=10,num_bikes_disabled=2,num_docks_available=36,num_docks_disabled=0,num_ebikes_available=0,station_status="active" 1641505084000000000
    citibike,station_id=4711 eightd_has_available_keys=false,is_installed=1,is_renting=1,is_returning=1,legacy_id="4711",num_bikes_available=9,num_bikes_disabled=0,num_docks_available=36,num_docks_disabled=0,num_ebikes_available=1,station_status="active" 1641505084000000000
             */

            var sb = new StringBuilder();

            var ns = (dt - DateTime.UnixEpoch).TotalNanoseconds;

            foreach (var partner in partners)
            {
                sb.Append($"packetmail,partner={partner.Callsign} queue_length={partner.QueueLength} {ns:0}\n");
            }

            return Ok(sb.ToString());
        }
        catch (LoginFailedException)
        {
            return Unauthorized(LoginError);
        }
    }

    [HttpGet("prometheus")]
    public async Task<IActionResult> GetPromethiusMetrics()
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(AuthError);
        }

        try
        {
            var (partners, timestamp) = await GetCachedMailPartners(header.Value.User, header.Value.Password);

            var ts = (timestamp - DateTime.UnixEpoch).TotalMilliseconds;
            /*
    # HELP http_requests_total The total number of HTTP requests.
    # TYPE http_requests_total counter
    http_requests_total{method="post",code="200"} 1027 1395066363000
    http_requests_total{method="post",code="400"}    3 1395066363000
             */

            var sb = new StringBuilder();
            sb.AppendLine("# HELP packetmail_queue_length The number of messages in the packetmail queue");
            sb.AppendLine("# TYPE packetmail_queue_length gauge");

            foreach (var partner in partners)
            {
                sb.Append($"packetmail_queue_length{{partner=\"{partner.Callsign}\"}} {partner.QueueLength} {ts:0}\n");
            }

            return Ok(sb.ToString());
        }
        catch (LoginFailedException)
        {
            return Unauthorized(LoginError);
        }
    }

    [HttpGet("json/mail")]
    public async Task<IActionResult> GetJsonMetrics()
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(AuthError);
        }

        try
        {
            var (partners, _) = await GetCachedMailPartners(header.Value.User, header.Value.Password);

            return Ok(partners.ToDictionary(item => item.Callsign, item => new { queueLength = item.QueueLength }));
        }
        catch (LoginFailedException)
        {
            return Unauthorized(LoginError);
        }
    }

    [MemoryCache(59)]
    private async Task<(ForwardingStation[], DateTime)> GetCachedMailPartners(string user, string password)
    {
        var partners = await bpqUiService.GetMailForwardingPartners(user, password);

        return (partners, DateTime.UtcNow);
    }
}
