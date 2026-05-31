using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;

namespace SD.API.Core
{
    public sealed class CosmosWarmupService(CosmosClient cosmosClient) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await cosmosClient.ReadAccountAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}