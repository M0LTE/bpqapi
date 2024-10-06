using Microsoft.Extensions.Options;

namespace bpqapi.Services;

public class ConfigCheckService(IOptions<BpqApiOptions> options, ILogger<ConfigCheckService> logger, BpqUiService bpqUiService) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (options.Value.Uri == null)
        {
            logger.LogError("No value specified for bpq__uri in configuration. Exiting.");
            Environment.Exit(1);
        }

        if (string.IsNullOrWhiteSpace(options.Value.SysopUsername))
        {
            logger.LogError("No value specified for bpq__sysopUsername in configuration. Exiting.");
            Environment.Exit(1);
        }

        if (string.IsNullOrWhiteSpace(options.Value.SysopPassword))
        {
            logger.LogError("No value specified for bpq__sysopPassword in configuration. Exiting.");
            Environment.Exit(1);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
