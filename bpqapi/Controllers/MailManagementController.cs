using bpqapi.Models;
using bpqapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

[ApiController]
[Route("[controller]")]
public class MailManagementController(BpqUiService bpqUiService) : ControllerBase
{
    [HttpGet("options")]
    public async Task<ForwardingOptions> GetOptions()
    {
        return await bpqUiService.GetForwardingOptions();
    }

    [HttpGet("partners")]
    public async Task<Dictionary<string, ForwardingStation>> GetMailForwardingPartners()
    {
        var result = (await bpqUiService.GetMailForwardingPartners())
            .ToDictionary(partner => partner.Callsign, partner => partner);

        return result;
    }
}
