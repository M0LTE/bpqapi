using bpqapi.Models;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

[ApiController]
[Route("[controller]")]
public class WebmailController(BpqUiService bpqUiService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(MailItem[]), 200)]
    public async Task<IActionResult> GetMail()
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            return Ok(await bpqUiService.GetMail(header.Value.User, header.Value.Password));
        }
        catch (LoginFailedException)
        {
            return Unauthorized("BPQ rejected that BBS login");
        }
    }
}
