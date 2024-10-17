using bpqapi.Models;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

[ApiController]
[Route("mail")]
public class MailManagementController(BpqUiService bpqUiService) : ControllerBase
{
    private const string AuthError = "Sysop username and password required as basic auth header";
    private const string LoginError = "BPQ rejected that sysop login";

    [HttpGet("options/bpq")]
    [ProducesResponseType(typeof(ForwardingOptions), 200)]
    public async Task<IActionResult> GetOptions()
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(AuthError);
        }

        try
        {
            return Ok(await bpqUiService.GetForwardingOptions(header.Value.User, header.Value.Password));
        }
        catch (LoginFailedException)
        {
            return Unauthorized(LoginError);
        }
    }

    [HttpGet("partners")]
    [ProducesResponseType(typeof(Dictionary<string, ForwardingStation>), 200)]
    public async Task<IActionResult> GetMailForwardingPartners()
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(AuthError);
        }

        try
        {
            var result = (await bpqUiService.GetMailForwardingPartners(header.Value.User, header.Value.Password))
                .ToDictionary(partner => partner.Callsign, partner => partner);

            return Ok(result);
        }
        catch (LoginFailedException)
        {
            return Unauthorized(LoginError);
        }
    }

    [HttpPost("partners/{callsign}/start-session")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> StartForwardingSession(string callsign)
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest(AuthError);
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
            return Unauthorized(LoginError);
        }
    }
}
