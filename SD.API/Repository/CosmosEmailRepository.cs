using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using SD.Shared.Models.Support;

namespace SD.API.Repository
{
    public class CosmosEmailRepository
    {
        public Container Container { get; private set; }

        public CosmosEmailRepository(IConfiguration config)
        {
            var connString = config.GetValue<string>("RepositoryOptions_CosmosConnectionString");
            var databaseId = config.GetValue<string>("RepositoryOptions_DatabaseId");
            var containerId = config.GetValue<string>("RepositoryOptions_ContainerMailId");

            var _client = new CosmosClient(connString, new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });

            Container = _client.GetContainer(databaseId, containerId);
        }

        public async Task<EmailDocument?> Get(string key, CancellationToken cancellationToken)
        {
            try
            {
                var response = await Container.ReadItemAsync<EmailDocument?>(key, new PartitionKey(key), null, cancellationToken);

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<EmailDocument?> Upsert(EmailDocument email, CancellationToken cancellationToken)
        {
            var response = await Container.UpsertItemAsync(email, new PartitionKey(email.Key), null, cancellationToken);

            return response.Resource;
        }
    }
}