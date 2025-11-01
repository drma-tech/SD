using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Subscription;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SD.API.Functions;

public class PaddleFunction(CosmosRepository repo, IHttpClientFactory factory)
{
    [Function("GetSubscription")]
    public async Task<RootSubscription?> GetSubscription(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/paddle/subscription")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var id = req.GetQueryParameters()["id"];
            if (id.Empty()) throw new UnhandledException("id null");

            var endpoint = ApiStartup.Configurations.Paddle?.Endpoint;
            var key = ApiStartup.Configurations.Paddle?.Key;

            var http = factory.CreateClient("paddle");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}subscriptions/{id}");

            var response = await http.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) throw new UnhandledException(response.ReasonPhrase);

            return await response.Content.ReadFromJsonAsync<RootSubscription>(cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("PostPaddleSubscription")]
    public async Task PostPaddleSubscription(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/paddle/subscription")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var validSignature = await req.ValidPaddleSignature(ApiStartup.Configurations.Paddle?.Signature, cancellationToken);

            if (!validSignature) throw new UnhandledException("wrong paddle signature");

            var body = await req.GetPublicBody<RootEvent>(cancellationToken) ?? throw new UnhandledException("body null");
            if (body.data == null) throw new UnhandledException("body.data null");

            await Task.Delay(1000, cancellationToken); //wait for user be updated in cosmos

            var result = await repo.Query<AuthPrincipal>(x => x.AuthPaddle != null && x.AuthPaddle.CustomerId == body.data.customer_id, DocumentType.Principal, cancellationToken) ??
                throw new UnhandledException("AuthPrincipal null");
            var client = result.LastOrDefault() ?? throw new UnhandledException($"client null - customer_id:{body.data.customer_id}");
            if (client.AuthPaddle == null) throw new UnhandledException("client.AuthPaddle null");

            client.AuthPaddle.SubscriptionId = body.data.id;
            client.AuthPaddle.IsPaidUser = body.data.status is "active" or "trialing";

            client.Events = client.Events.Union([new Event { Description = $"subscription = {body.data.status}" }]).ToArray();

            await repo.UpsertItemAsync(client, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("PostAppleSubscription")]
    public async Task PostAppleSubscription(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/apple/subscription")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            //var validSignature = await req.ValidPaddleSignature(ApiStartup.Configurations.Paddle?.Signature, cancellationToken);

            //if (!validSignature) throw new UnhandledException("wrong paddle signature");

            //var body = await req.GetPublicBody<RootEvent>(cancellationToken) ?? throw new UnhandledException("body null");
            //if (body.data == null) throw new UnhandledException("body.data null");

            //await Task.Delay(1000, cancellationToken); //wait for user be updated in cosmos

            //var result = await repo.Query<AuthPrincipal>(x => x.AuthPaddle != null && x.AuthPaddle.CustomerId == body.data.customer_id, DocumentType.Principal, cancellationToken) ??
            //    throw new UnhandledException("AuthPrincipal null");
            //var client = result.LastOrDefault() ?? throw new UnhandledException($"client null - customer_id:{body.data.customer_id}");
            //if (client.AuthPaddle == null) throw new UnhandledException("client.AuthPaddle null");

            //client.AuthPaddle.SubscriptionId = body.data.id;
            //client.AuthPaddle.IsPaidUser = body.data.status is "active" or "trialing";

            //client.Events = client.Events.Union([new Event { Description = $"subscription = {body.data.status}" }]).ToArray();

            //await repo.UpsertItemAsync(client, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("Configurations")]
    public static PaddleConfigurations Configurations(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/paddle/configurations")] HttpRequestData req)
    {
        try
        {
            var config = new PaddleConfigurations
            {
                CustomerPortalEndpoint = ApiStartup.Configurations.Paddle?.CustomerPortalEndpoint,
                Token = ApiStartup.Configurations.Paddle?.Token,
                ProductStandard = ApiStartup.Configurations.Paddle?.Standard?.Product,
                ProductPremium = ApiStartup.Configurations.Paddle?.Premium?.Product,
                PriceStandardMonth = ApiStartup.Configurations.Paddle?.Standard?.PriceMonth,
                PriceStandardYear = ApiStartup.Configurations.Paddle?.Standard?.PriceYear,
                PricePremiumMonth = ApiStartup.Configurations.Paddle?.Premium?.PriceMonth,
                PricePremiumYear = ApiStartup.Configurations.Paddle?.Premium?.PriceYear
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