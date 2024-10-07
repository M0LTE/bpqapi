using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

public class NodeController(BpqApiService bpqApiService) : ControllerBase
{
    [HttpGet("mheard")]
    [ProducesResponseType(200, Type = typeof(MHeard[]))]
    public async Task<IActionResult> Mheard(string port)
    {
        var token = (await bpqApiService.GetToken()).AccessToken;
        return Ok(await bpqApiService.GetMheard(token, port));
    }

    [HttpGet("ports")]
    [ProducesResponseType(200, Type = typeof(Port[]))]
    public async Task<IActionResult> Ports()
    {
        var token = (await bpqApiService.GetToken()).AccessToken;
        return Ok(await bpqApiService.GetPorts(token));
    }
}
