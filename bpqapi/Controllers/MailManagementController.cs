using bpqapi.Models;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

[ApiController]
[Route("mail")]
public class MailManagementController(BpqUiService bpqUiService) : ControllerBase
{
    [HttpGet("options/bpq")]
    [ProducesResponseType(typeof(ForwardingOptions), 200)]
    public async Task<IActionResult> GetOptions()
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(Resources.AuthError);
        }

        try
        {
            return Ok(await bpqUiService.GetForwardingOptions(header.Value.User, header.Value.Password));
        }
        catch (LoginFailedException)
        {
            return Unauthorized(Resources.LoginError);
        }
    }

    [HttpGet("partners")]
    [ProducesResponseType(typeof(Dictionary<string, ForwardingStation>), 200)]
    public async Task<IActionResult> GetMailForwardingPartners()
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(Resources.AuthError);
        }

        try
        {
            var result = (await bpqUiService.GetMailForwardingPartners(header.Value.User, header.Value.Password))
                .ToDictionary(partner => partner.Callsign, partner => partner);

            return Ok(result);
        }
        catch (LoginFailedException)
        {
            return Unauthorized(Resources.LoginError);
        }
    }

    /// <summary>
    /// Get a dictionary of all mail forwarding partner stations with outstanding messages in their queues, with the 
    /// message IDs of the messages in each queue. Warning- this is slow due to underlying calls.
    /// </summary>
    /// <returns></returns>
    [HttpGet("outbound-queues")]
    [ProducesResponseType(typeof(Dictionary<string, List<int>>), 200)]
    public async Task<IActionResult> GetOutboundMailQueues()
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(Resources.AuthError);
        }

        try
        {
            var result = await bpqUiService.GetQueues(header.Value.User, header.Value.Password);
            return Ok(result);
        }
        catch (LoginFailedException)
        {
            return Unauthorized(Resources.LoginError);
        }
    }

    [HttpPost("partners/{callsign}/start-session")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> StartForwardingSession(string callsign)
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(Resources.AuthError);
        }

        try
        {
            var result = await bpqUiService.StartForwardingSession(header.Value.User, header.Value.Password, callsign);
            if (result)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, "Failed to start forwarding session");
            }
        }
        catch (LoginFailedException)
        {
            return Unauthorized(Resources.LoginError);
        }
    }
}
