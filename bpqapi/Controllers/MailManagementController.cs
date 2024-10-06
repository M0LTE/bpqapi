using bpqapi.Models;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

[ApiController]
[Route("[controller]")]
public class MailManagementController(BpqUiService bpqUiService) : ControllerBase
{
    private const string AuthError = "Sysop username and password required as basic auth header";
    private const string LoginError = "BPQ rejected that sysop login";

    [HttpGet("options")]
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
}
