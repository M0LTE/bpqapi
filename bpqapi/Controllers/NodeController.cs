using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

public class NodeController(BpqApiService bpqApiService) : ControllerBase
{
    [HttpGet("mheard")]
    public async Task<IActionResult> Mheard(string port)
    {
        var token = (await bpqApiService.GetToken()).AccessToken;
        return Ok(await bpqApiService.GetMheard(token, port));
    }
}
