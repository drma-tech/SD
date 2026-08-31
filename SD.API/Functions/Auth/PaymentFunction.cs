using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.API.Core.Models;
using SD.Shared.Core.Types;
using SD.Shared.Models.Auth;
using Stripe.Checkout;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SD.API.Functions.Auth;

public class PaymentFunction(CosmosMainRepository repo, IHttpClientFactory factory)
{
    private const string APP_CODE = "sd";
    private const string APP = "app";
    private const string USERID = "userId";

    [Function("PostAppleVerify")]
    public async Task PostAppleVerify(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "apple/verify")] HttpRequestData req, CancellationToken cancellationToken)
    {
        AuthPrincipal? client = null;
        try
        {
            var userId = await req.GetUserIdAsync();
            var ip = req.GetUserIP(includePort: true);

            client = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken) ?? throw new UnhandledException("principal null");

            var raw = await req.ReadAsStringAsync();
            var receipt = JsonSerializer.Deserialize<string>(raw ?? throw new UnhandledException("body not present"));

            var bundleId = ApiStartup.Configurations.Apple?.BundleId;

            var result = await VerifyReceipt(ApiStartup.Configurations.Apple?.Endpoint, receipt, cancellationToken) ?? throw new UnhandledException("AppleResponseReceipt null");
            if (result.status == 21007)
            {
                //when tested with TestFlight
                result = await VerifyReceipt("https://sandbox.itunes.apple.com/", receipt, cancellationToken) ?? throw new UnhandledException("AppleResponseReceipt null");
            }

            if (result.status != 0) throw new UnhandledException(string.Create(CultureInfo.InvariantCulture, $"invalid status: {result.status}"));
            if (!string.Equals(result.receipt!.bundle_id, bundleId, StringComparison.OrdinalIgnoreCase)) throw new UnhandledException("invalid receipt");

            var purchase = result.latest_receipt_info[^1];

            var sub = new AuthSubscription
            {
                Provider = PaymentProvider.Apple,
                Product = AccountProduct.Premium,
                Cycle = purchase.product_id!.Contains("yearly", StringComparison.OrdinalIgnoreCase) ? AccountCycle.Yearly : AccountCycle.Monthly,
                SessionId = receipt, //save receipt before cause it may fail
                SubscriptionId = purchase.original_transaction_id,
                ExpiresDate = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(purchase.expires_date_ms ?? "0", CultureInfo.InvariantCulture)),
            };

            client.AddSubscription(sub);

            //https://developer.apple.com/documentation/appstorereceipts/status
            client.Events.Add(new Event("Apple", string.Create(CultureInfo.InvariantCulture, $"Subscription created with status = {result.status} and id = {purchase.original_transaction_id}"), ip));
        }
        finally
        {
            if (client != null) await repo.UpsertItemAsync(client);
        }
    }

    private async Task<AppleResponseReceipt?> VerifyReceipt(string? endpoint, string? receipt, CancellationToken cancellationToken)
    {
        var http = factory.CreateClient("apple");
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}verifyReceipt");
        request.Content = new StringContent($$"""{"receipt-data":"{{receipt}}","password":"{{ApiStartup.Configurations.Apple?.SharedSecret}}","exclude-old-transactions":true}""", Encoding.UTF8, "application/json");
        var response = await http.SendAsync(request, cancellationToken);
        return await response.Content.ReadFromJsonAsync<AppleResponseReceipt>(cancellationToken);
    }

    [Function("StripeCreateCustomer")]
    public async Task<AuthPrincipal> StripeCreateCustomer(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "stripe/customer")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync();
        var principal = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken) ?? throw new UnhandledException("principal null");

        var customer = await new Stripe.CustomerService().CreateAsync(new Stripe.CustomerCreateOptions
        {
            Name = principal.DisplayName,
            Email = principal.Email,
            Metadata = new Dictionary<string, string>(StringComparer.Ordinal) {
                { APP, APP_CODE },
                { USERID, principal.UserId! },
            },
        }, cancellationToken: cancellationToken);

        principal.StripeCustomerId = customer.Id;

        var ip = req.GetUserIP(includePort: true);
        principal.Events.Add(new Event("Stripe", $"User registered with id:{customer.Id}", ip));

        return await repo.UpsertItemAsync(principal);
    }

    [Function("CreateCheckoutSession")]
    public async Task<string> CreateCheckoutSession(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "stripe/create-checkout-session/{priceId}")] HttpRequestData req, string priceId, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync();
        var ip = req.GetUserIP(includePort: true);
        var url = req.GetQueryParameters()["url"];

        var principal = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken) ?? throw new UnhandledException("principal null");

        if (principal.StripeCustomerId.Empty()) throw new UnhandledException("Stripe customer not available");

        var options = new SessionCreateOptions
        {
            Customer = principal.StripeCustomerId,

            LineItems = [new() { Price = priceId, Quantity = 1, },],
            Mode = "subscription",
            SuccessUrl = url + "?stripe_session_id={CHECKOUT_SESSION_ID}",
            Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { APP, APP_CODE },
                { USERID, principal.UserId! },
            },
            SubscriptionData = new SessionSubscriptionDataOptions
            {
                Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                    { APP, APP_CODE },
                    { USERID, principal.UserId! },
                },
            },
        };

        options.AddExtraParam("managed_payments[enabled]", value: true);

        var service = new SessionService();
        Session session = await service.CreateAsync(options, cancellationToken: cancellationToken);

        AccountCycle? cycle = null;

        if (string.Equals(priceId, ApiStartup.Configurations.Stripe!.Premium!.PriceMonth, StringComparison.OrdinalIgnoreCase))
            cycle = AccountCycle.Monthly;
        else if (string.Equals(priceId, ApiStartup.Configurations.Stripe!.Premium!.PriceYear, StringComparison.OrdinalIgnoreCase))
            cycle = AccountCycle.Yearly;

        var sub = new AuthSubscription()
        {
            Provider = PaymentProvider.Stripe,
            Product = AccountProduct.Premium,
            Cycle = cycle,
            SessionId = session.Id,
        };

        principal.AddSubscription(sub);

        principal.Events.Add(new Event("Stripe", $"Session created with cycle = {cycle} and SessionId = {session.Id}", ip));

        await repo.UpsertItemAsync(principal);

        return session.Url;
    }

    [Function("StripeGePortalLink")]
    public async Task<string> StripeGePortalLink(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "stripe/portal-link")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var url = req.GetQueryParameters()["url"];
        var userId = await req.GetUserIdAsync();
        var principal = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken) ?? throw new UnhandledException("principal null");

        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = principal.StripeCustomerId,
            ReturnUrl = url,
        };
        var service = new Stripe.BillingPortal.SessionService();
        var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

        return session.Url;
    }
}