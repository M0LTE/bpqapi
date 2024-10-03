using bpqapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace bpqapi.Controllers;

[ApiController]
[Route("[controller]")]
public class MailManagementController : ControllerBase
{
    [HttpGet("options")]
    public ForwardingOptions GetOptions()
    {
        // get options from http://gb7rdg-node:8008/Mail/FWD?M0000FAB73A83

        throw new NotImplementedException();
    }

    [HttpGet("partners")]
    public ForwardingStation[] GetPartners()
    {
        // get list with post to http://gb7rdg-node:8008/Mail/FwdList.txt?M0000FAB73A83

        // for each one, post to http://gb7rdg-node:8008/Mail/FwdDetails?M0000FAB73A83

        throw new NotImplementedException();
    }
}
