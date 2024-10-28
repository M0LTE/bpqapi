using bpqapi.Models.BpqApi;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

/// <summary>
/// Makes a rough attempt at passing through the underlying API structure, where possible.
/// </summary>
/// <param name="nativeApiService"></param>
[Route("native")]
public class NativeApiController(BpqNativeApiService nativeApiService) : ControllerBase
{
    private async Task<string> GetLegacyToken() => (await nativeApiService.RequestLegacyToken()).AccessToken;

    private async Task<string> GetMailToken()
    {
        var header = HttpContext.ParseBasicAuthHeader();
        if (header is null)
        {
            throw new Exception("No basic auth header found");
        }
        return (await nativeApiService.RequestMailToken(header.Value.User, header.Value.Password)).AccessToken;
    }

    [HttpGet("v1/ports")]
    [ProducesResponseType(200, Type = typeof(NativeGetPortsResponse))]
    public async Task<IActionResult> Ports() => Ok(await nativeApiService.GetPorts(await GetLegacyToken()));

    [HttpGet("v1/nodes")]
    [ProducesResponseType(200, Type = typeof(NativeGetNodesResponse))]
    public async Task<IActionResult> Nodes() => Ok(await nativeApiService.GetNodes(await GetLegacyToken()));

    [HttpGet("v1/users")]
    [ProducesResponseType(200, Type = typeof(NativeGetUsersResponse))]
    public async Task<IActionResult> Users() => Ok(await nativeApiService.GetUsers(await GetLegacyToken()));

    [HttpGet("v1/info")]
    [ProducesResponseType(200, Type = typeof(NativeGetInfoResponse))]
    public async Task<IActionResult> Info() => Ok(await nativeApiService.GetInfo(await GetLegacyToken()));

    [HttpGet("v1/links")]
    [ProducesResponseType(200, Type = typeof(NativeGetLinksResponse))]
    public async Task<IActionResult> Links() => Ok(await nativeApiService.GetLinks(await GetLegacyToken()));

    [HttpGet("v1/mheard/{port}")]
    [ProducesResponseType(200, Type = typeof(NativeMheardElement[]))]
    public async Task<IActionResult> Mheard(int port) => Ok(await nativeApiService.GetMheard(await GetLegacyToken(), port));

    [HttpGet("v1/mail/msgs")]
    [ProducesResponseType(200, Type = typeof(NativeV1MailMessagesResponse))]
    public async Task<IActionResult> MailMessages() => Ok(await nativeApiService.GetMessagesV1(await GetMailToken()));

    [HttpGet("v1/mail/fwdqlen")]
    [ProducesResponseType(200, Type = typeof(NativeV1MailForwardQueueLengthResponse))]
    public async Task<IActionResult> MailQueueLengths() => Ok(await nativeApiService.GetQueueLengths(await GetMailToken()));

    [HttpGet("v1/mail/fwdconfig")]
    [ProducesResponseType(200, Type = typeof(NativeMailForwardConfigV1Response))]
    public async Task<IActionResult> MailFwdConfig() => Ok(await nativeApiService.GetForwardConfig(await GetMailToken())); // token not actually required at the moment
}
