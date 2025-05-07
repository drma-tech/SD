using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Subscription;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SD.API.Functions
{
    public class PaddleFunction(CosmosRepository repo, IConfiguration configuration)
    {
        [Function("GetSubscription")]
        public async Task<RootSubscription?> GetSubscription(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/paddle/subscription")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];

                var endpoint = configuration.GetValue<string>("Paddle:Endpoint");
                var key = configuration.GetValue<string>("Paddle:Key");

                ApiStartup.HttpClientPaddle.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

                using var request = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}subscriptions/{id}");

                var response = await ApiStartup.HttpClientPaddle.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode) throw new UnhandledException(response.ReasonPhrase);

                return await response.Content.ReadFromJsonAsync<RootSubscription>(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("GetSubscriptionUpdate")]
        public async Task<RootSubscription?> GetSubscriptionUpdate(
          [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/paddle/subscription/update")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];

                var endpoint = configuration.GetValue<string>("Paddle:Endpoint");
                var key = configuration.GetValue<string>("Paddle:Key");

                ApiStartup.HttpClientPaddle.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

                using var request = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}subscriptions/{id}/update-payment-method-transaction");

                var response = await ApiStartup.HttpClientPaddle.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode) throw new UnhandledException(response.ReasonPhrase);

                return await response.Content.ReadFromJsonAsync<RootSubscription>(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("PostSubscription")]
        public async Task PostSubscription(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "public/paddle/subscription")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var validSignature = await req.ValidPaddleSignature(configuration["Paddle:Signature"], cancellationToken);

                if (!validSignature) throw new UnhandledException("wrong paddle signature");

                var body = await req.GetPublicBody<RootEvent>(cancellationToken) ?? throw new UnhandledException("body null");
                if (body.data == null) throw new UnhandledException("body.data null");

                var result = await repo.Query<ClientePrincipal>(x => x.ClientePaddle != null && x.ClientePaddle.CustomerId == body.data.customer_id, DocumentType.Principal, cancellationToken) ?? throw new UnhandledException("ClientePrincipal null");
                var client = result.FirstOrDefault() ?? throw new UnhandledException($"client null - customer_id:{body.data.customer_id}");
                if (client.ClientePaddle == null) throw new UnhandledException("client.ClientePaddle null");

                client.ClientePaddle.SubscriptionId = body.data.id;
                client.ClientePaddle.IsPaidUser = (body.data.status is "active" or "trialing");

                await repo.Upsert(client, cancellationToken);
                req.LogWarning($"{body.data.id} - {body.data.status}");
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("Configurations")]
        public Configurations Configurations([HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/paddle/configurations")] HttpRequestData req)
        {
            try
            {
                var config = new Configurations
                {
                    Token = configuration.GetValue<string>("Paddle:Token"),
                    ProductStandard = configuration.GetValue<string>("Paddle:Standard:Product"),
                    ProductPremium = configuration.GetValue<string>("Paddle:Premium:Product"),
                    PriceStandardMonth = configuration.GetValue<string>("Paddle:Standard:PriceMonth"),
                    PriceStandardYear = configuration.GetValue<string>("Paddle:Standard:PriceYear"),
                    PricePremiumMonth = configuration.GetValue<string>("Paddle:Premium:PriceMonth"),
                    PricePremiumYear = configuration.GetValue<string>("Paddle:Premium:PriceYear")
                };

                return config;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }
    }
}