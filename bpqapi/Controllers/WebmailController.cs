using bpqapi.Models;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

[ApiController]
[Route("mail")]
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
            return Ok(await bpqUiService.GetAllMail(header.Value.User, header.Value.Password));
        }
        catch (LoginFailedException)
        {
            return Unauthorized("BPQ rejected that BBS login");
        }
    }

    [HttpGet("inbox")]
    [ProducesResponseType(typeof(MailItem[]), 200)]
    public async Task<IActionResult> Inbox()
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            return Ok(await bpqUiService.GetInbox(header.Value.User, header.Value.Password));
        }
        catch (LoginFailedException)
        {
            return Unauthorized("BPQ rejected that BBS login");
        }
    }

    [HttpGet("sent")]
    [ProducesResponseType(typeof(MailItem[]), 200)]
    public async Task<IActionResult> Sent()
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            return Ok(await bpqUiService.GetSentMail(header.Value.User, header.Value.Password));
        }
        catch (LoginFailedException)
        {
            return Unauthorized("BPQ rejected that BBS login");
        }
    }
}
