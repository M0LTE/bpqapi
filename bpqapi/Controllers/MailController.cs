using bpqapi.Models;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

/// <summary>
/// Opinionated wrapper around BPQ mail
/// </summary>
[ApiController]
[Route("mail")]
public class MailController(BpqUiService bpqUiService, BpqTelnetClient bpqTelnetClient, ILogger<MailController> logger, MailService mailService) : ControllerBase
{
    /// <summary>
    /// Retrieve mail items by comma separated id.
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpGet("{ids}")]
    [ProducesResponseType(typeof(MailEntity[]), 200)]
    public async Task<IActionResult> GetMailItems(string ids)
    {
        logger.LogInformation("GET /mail/{ids}", ids);

        var idsSplit = ids.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var id in idsSplit)
        {
            if (!int.TryParse(id, out _))
            {
                return BadRequest("Invalid ID " + id);
            }
        }

        var idInts = idsSplit.Select(int.Parse).ToArray();

        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            var items = await mailService.GetMail(header.Value.User, header.Value.Password, idInts);
            logger.LogInformation("Call to /mail/{{ids}} returned {Count} items", items.Count);
            return Ok(items);
        }
        catch (LoginFailedException)
        {
            return Unauthorized("BPQ rejected that BBS login");
        }
    }

    /// <summary>
    /// Get a list of all mail items of all types
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(MailListEntity[]), 200)]
    public async Task<IActionResult> GetAllTypesList(DataSource? dataSource = DataSource.Ui)
    {
        logger.LogInformation("GET /mail");

        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        if (dataSource == DataSource.Telnet)
        {
            var loginResult = await bpqTelnetClient.Login(header.Value.User, header.Value.Password);
            if (loginResult != TelnetLoginResult.Success)
            {
                return Unauthorized("BPQ rejected that telnet login");
            }

            var items = await bpqTelnetClient.MessageList();
            return Ok(items);
        }
        else if (dataSource == DataSource.Ui)
        {
            try
            {
                var items = await bpqUiService.GetWebmailAllMailList(header.Value.User, header.Value.Password);
                logger.LogInformation("Call to /mail returned {Count} items", items.Length);
                return Ok(items);
            }
            catch (LoginFailedException)
            {
                return Unauthorized("BPQ rejected that webmail login");
            }
        }
        else
        {
            return BadRequest("Data source must be specified");
        }
    }

    /// <summary>
    /// Get a list of all bulletins
    /// </summary>
    /// <returns></returns>
    [HttpGet("bulletins")]
    [ProducesResponseType(typeof(MailListEntity[]), 200)]
    public async Task<IActionResult> GetBulletinsList()
    {
        logger.LogInformation("GET /mail/bulletins");

        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            var items = await bpqUiService.GetWebmailBullsList(header.Value.User, header.Value.Password);
            logger.LogInformation("Call to /mail/bulletins returned {Count} items", items.Length);
            return Ok(items);
        }
        catch (LoginFailedException)
        {
            return Unauthorized("BPQ rejected that BBS login");
        }
    }


    /// <summary>
    /// Get a list of all personal mail, not just mine
    /// </summary>
    /// <returns></returns>
    [HttpGet("personal")]
    [ProducesResponseType(typeof(MailListEntity[]), 200)]
    public async Task<IActionResult> GetPersonalMailList()
    {
        logger.LogInformation("GET /mail/personal");

        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            var items = await bpqUiService.GetWebmailPersonalsList(header.Value.User, header.Value.Password);
            logger.LogInformation("Call to /mail/personal returned {Count} items", items.Length);
            return Ok(items);
        }
        catch (LoginFailedException)
        {
            return Unauthorized("BPQ rejected that BBS login");
        }
    }

    /// <summary>
    /// Inbox listing, i.e. BPQ "My Rxed" mail
    /// </summary>
    /// <returns></returns>
    [HttpGet("inbox")]
    [ProducesResponseType(typeof(MailListEntity[]), 200)]
    public async Task<IActionResult> GetMyInbox()
    {
        logger.LogInformation("GET /mail/inbox");

        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            var items = await bpqUiService.GetWebmailInbox(header.Value.User, header.Value.Password);
            logger.LogInformation("Call to /mail/inbox returned {Count} items", items.Length);
            return Ok(items);
        }
        catch (LoginFailedException)
        {
            return Unauthorized("BPQ rejected that BBS login");
        }
    }

    /// <summary>
    /// Sent items, i.e. BPQ "My Sent" mail
    /// </summary>
    /// <returns></returns>
    [HttpGet("sent")]
    [ProducesResponseType(typeof(MailListEntity[]), 200)]
    public async Task<IActionResult> GetMySentList()
    {
        logger.LogInformation("GET /mail/sent");

        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            var items = await bpqUiService.GetWebmailSentMail(header.Value.User, header.Value.Password);
            logger.LogInformation("Call to /mail/sent returned {Count} items", items.Length);
            return Ok(items);
        }
        catch (LoginFailedException)
        {
            return Unauthorized("BPQ rejected that BBS login");
        }
    }

    /// <summary>
    /// Send a message
    /// </summary>
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMailEntity mail)
    {
        logger.LogInformation("POST /mail/send");

        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            await bpqUiService.SendWebmail(header.Value.User, header.Value.Password, mail);
            return Ok();
        }
        catch (LoginFailedException)
        {
            return Unauthorized("BPQ rejected that BBS login");
        }
    }

    /// <summary>
    /// Set message state
    /// </summary>
    /// <param name="id"></param>
    /// <param name="read"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<IActionResult> MarkReadState(int id, [FromQuery]bool? read)
    {
        logger.LogInformation("PATCH /mail/{id}", id);

        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        if (read != null)
        {
            if (!await mailService.SetReadState(id, read.Value))
            {
                return NotFound();
            }
        }

        return NoContent();
    }
}

public enum DataSource
{
    Telnet, Ui
}