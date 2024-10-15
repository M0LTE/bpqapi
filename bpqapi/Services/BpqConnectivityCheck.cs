using Microsoft.Extensions.Options;

namespace bpqapi.Services;

public class BpqConnectivityCheck(BpqApiService bpqApiService, ILogger<BpqConnectivityCheck> logger, IOptions<BpqApiOptions> options) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await bpqApiService.GetToken();
        }
        catch (Exception ex)
        {
            logger.LogError("Could not connect to BPQ at {uri}: {error}", options.Value.Uri, ex.Message);
            Environment.Exit(1);
        }

        logger.LogInformation("Connected to BPQ ok using {uri}", options.Value.Uri);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
