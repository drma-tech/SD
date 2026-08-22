using Newtonsoft.Json;
using SD.Shared.Core.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SD.Shared.Models.Auth;

public class AuthPrincipal(string? id) : MainDocument(new MainIdentity(MainType.Principal, id))
{
    public string? UserId { get; set; } = id;
    public string? DisplayName { get; set; }
    [DataType(DataType.EmailAddress)] public string? Email { get; set; }
    public string? StripeCustomerId { get; set; }

    public string[] AuthProviders { get; set; } = [];
    public ISet<AuthSubscription> Subscriptions { get; set; } = new HashSet<AuthSubscription>();
    public ISet<Event> Events { get; set; } = new HashSet<Event>();

    public AuthSubscription? GetActiveSubscription()
    {
        return Subscriptions.SingleOrDefault(p => p.IsActive());
    }

    public AuthSubscription GetSubscription(string? id, PaymentProvider provider)
    {
        var sub = Subscriptions.SingleOrDefault(s => string.Equals(s.SubscriptionId, id, StringComparison.Ordinal));
        if (sub != null) return sub;

        sub = Subscriptions.OrderBy(p => p.CreatedAt).LastOrDefault(p => p.Provider == provider) ?? throw new NotificationException("No subscriptions found.");
        sub.SubscriptionId = id;
        return sub;
    }

    public void AddSubscription(AuthSubscription subscription)
    {
        if (Subscriptions.Any(p => p.IsActive()))
        {
            throw new NotificationException("There is already an active subscription. Please deactivate the old one first before creating a new one.");
        }

        Subscriptions.Add(subscription);
    }

    public void UpdateSubscription(AuthSubscription subscription, bool validateId = true)
    {
        if (validateId && subscription.SubscriptionId.Empty()) throw new UnhandledException("subscription id is null");

        var sub = Subscriptions.SingleOrDefault(sub => string.Equals(sub.SubscriptionId, subscription.SubscriptionId, StringComparison.Ordinal))
            ?? throw new NotificationException("Subscription not found.");

        if (Subscriptions.Any(p => p.IsActive() && !string.Equals(p.SubscriptionId, sub.SubscriptionId, StringComparison.Ordinal)))
        {
            throw new NotificationException("There is already an active subscription. Please deactivate the old one first before creating a new one.");
        }

        sub.SessionId = subscription.SessionId;
        sub.ExpiresDate = subscription.ExpiresDate;
        sub.Active = subscription.Active;
        sub.Provider = subscription.Provider;
        sub.Product = subscription.Product;
        sub.Cycle = subscription.Cycle;
    }

    protected override object?[] EqualityValues => [Id];
}

public class AuthSubscription : EqualityBase<AuthSubscription>
{
    public string? SubscriptionId { get; set; }
    public string? SessionId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpiresDate { get; set; }
    public bool Active { get; set; }

    public PaymentProvider? Provider { get; set; }
    public AccountProduct? Product { get; set; }
    public AccountCycle? Cycle { get; set; }

    [JsonIgnore]
    [NotMapped]
    public AccountProduct ActiveProduct => IsActive() ? Product ?? AccountProduct.Basic : AccountProduct.Basic;

    public bool IsActive()
    {
        return Provider switch
        {
            PaymentProvider.Paddle => Active,
            PaymentProvider.Microsoft => false,
            PaymentProvider.Google => false,
            PaymentProvider.Apple => ExpiresDate.HasValue && ExpiresDate.Value.AddMinutes(5) > DateTimeOffset.UtcNow,
            PaymentProvider.Stripe => Active,
            _ => throw new UnhandledException("invalid provider"),
        };
    }

    protected override object?[] EqualityValues => [SubscriptionId, SessionId];
}

public class Event(string? origin, string? description, string? ip) : EqualityBase<Event>
{
    public string? Origin { get; set; } = origin;
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
    public string? Description { get; set; } = description;
    public string? Ip { get; set; } = ip;

    protected override object?[] EqualityValues => [Origin, Date];
}