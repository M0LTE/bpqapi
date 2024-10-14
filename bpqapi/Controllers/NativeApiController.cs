using bpqapi.Models.BpqApi;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

/// <summary>
/// Makes a rough attempt at passing through the underlying API structure, where possible.
/// </summary>
/// <param name="bpqApiService"></param>
[Route("native")]
public class NativeApiController(BpqApiService bpqApiService) : ControllerBase
{
    private async Task<string> GetToken() => (await bpqApiService.GetToken()).AccessToken;

    [HttpGet("ports")]
    [ProducesResponseType(200, Type = typeof(GetPortsResponse))]
    public async Task<IActionResult> Ports() => Ok(await bpqApiService.GetPorts(await GetToken()));

    [HttpGet("nodes")]
    [ProducesResponseType(200, Type = typeof(GetNodesResponse))]
    public async Task<IActionResult> Nodes() => Ok(await bpqApiService.GetNodes(await GetToken()));

    [HttpGet("users")]
    [ProducesResponseType(200, Type = typeof(GetUsersResponse))]
    public async Task<IActionResult> Users() => Ok(await bpqApiService.GetUsers(await GetToken()));

    [HttpGet("info")]
    [ProducesResponseType(200, Type = typeof(GetInfoResponse))]
    public async Task<IActionResult> Info() => Ok(await bpqApiService.GetInfo(await GetToken()));

    [HttpGet("links")]
    [ProducesResponseType(200, Type = typeof(GetLinksResponse))]
    public async Task<IActionResult> Links() => Ok(await bpqApiService.GetLinks(await GetToken()));

    [HttpGet("mheard/{port}")]
    [ProducesResponseType(200, Type = typeof(BpqApiMheardElement[]))]
    public async Task<IActionResult> Mheard(int port) => Ok(await bpqApiService.GetMheard(await GetToken(), port));
}
