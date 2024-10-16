using bpqapi.Models;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

/// <summary>
/// Opinionated wrapper around BPQ mail
/// </summary>
[ApiController]
[Route("mail")]
public class MailController(BpqUiService bpqUiService, BpqTelnetClient bpqTelnetClient) : ControllerBase
{
    /// <summary>
    /// Retrieve mail item by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MailEntity), 200)]
    public async Task<IActionResult> GetMailItem(int id)
    {
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            return Ok(await bpqUiService.GetWebmailItem(header.Value.User, header.Value.Password, id));
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
                return Ok(await bpqUiService.GetWebmailAllMailList(header.Value.User, header.Value.Password));
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
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            return Ok(await bpqUiService.GetWebmailBullsList(header.Value.User, header.Value.Password));
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
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            return Ok(await bpqUiService.GetWebmailPersonalsList(header.Value.User, header.Value.Password));
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
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            return Ok(await bpqUiService.GetWebmailInbox(header.Value.User, header.Value.Password));
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
        var header = HttpContext.ParseBasicAuthHeader();

        if (header == null)
        {
            return BadRequest("BBS callsign and password required as basic auth header");
        }

        try
        {
            return Ok(await bpqUiService.GetWebmailSentMail(header.Value.User, header.Value.Password));
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
}

public enum DataSource
{
    Telnet, Ui
}