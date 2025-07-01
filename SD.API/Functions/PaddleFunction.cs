using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Subscription;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SD.API.Functions;

public class PaddleFunction(CosmosRepository repo)
{
    [Function("GetSubscription")]
    public async Task<RootSubscription?> GetSubscription(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/paddle/subscription")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var id = req.GetQueryParameters()["id"];

            var endpoint = ApiStartup.Configurations.Paddle?.Endpoint;
            var key = ApiStartup.Configurations.Paddle?.Key;

            ApiStartup.HttpClientPaddle.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}subscriptions/{id}");

            var response = await ApiStartup.HttpClientPaddle.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) throw new UnhandledException(response.ReasonPhrase);

            return await response.Content.ReadFromJsonAsync<RootSubscription>(cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("GetSubscriptionUpdate")]
    public async Task<RootSubscription?> GetSubscriptionUpdate(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/paddle/subscription/update")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var id = req.GetQueryParameters()["id"];

            var endpoint = ApiStartup.Configurations.Paddle?.Endpoint;
            var key = ApiStartup.Configurations.Paddle?.Key;

            ApiStartup.HttpClientPaddle.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}subscriptions/{id}/update-payment-method-transaction");

            var response = await ApiStartup.HttpClientPaddle.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) throw new UnhandledException(response.ReasonPhrase);

            return await response.Content.ReadFromJsonAsync<RootSubscription>(cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("PostSubscription")]
    public async Task PostSubscription(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/paddle/subscription")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var validSignature = await req.ValidPaddleSignature(ApiStartup.Configurations.Paddle?.Signature, cancellationToken);

            if (!validSignature) throw new UnhandledException("wrong paddle signature");

            var body = await req.GetPublicBody<RootEvent>(cancellationToken) ?? throw new UnhandledException("body null");
            if (body.data == null) throw new UnhandledException("body.data null");

            await Task.Delay(200, cancellationToken);

            var result = await repo.Query<ClientePrincipal>(x => x.ClientePaddle != null && x.ClientePaddle.CustomerId == body.data.customer_id, DocumentType.Principal, 
                cancellationToken) ?? throw new UnhandledException("ClientePrincipal null");
            var client = result.FirstOrDefault() ?? throw new UnhandledException($"client null - customer_id:{body.data.customer_id}");
            if (client.ClientePaddle == null) throw new UnhandledException("client.ClientePaddle null");

            client.ClientePaddle.SubscriptionId = body.data.id;
            client.ClientePaddle.IsPaidUser = body.data.status is "active" or "trialing";

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
    public static PaddleConfigurations Configurations(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/paddle/configurations")]
        HttpRequestData req)
    {
        try
        {
            var config = new PaddleConfigurations
            {
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