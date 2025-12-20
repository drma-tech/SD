using Newtonsoft.Json;
using SD.Shared.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SD.Shared.Models.Auth;

public class AuthPrincipal() : PrivateMainDocument(DocumentType.Principal)
{
    public string? UserId { get; set; }
    public string[] AuthProviders { get; set; } = [];
    public string? DisplayName { get; set; }
    [DataType(DataType.EmailAddress)] public string? Email { get; set; }

    public HashSet<AuthSubscription> Subscriptions { get; set; } = [];
    public List<Event> Events { get; set; } = [];

    public override void Initialize(string userId)
    {
        base.Initialize(userId);
        UserId = userId;
    }

    public AuthSubscription? GetActiveSubscription()
    {
        return Subscriptions.SingleOrDefault(p => p.IsActive());
    }

    public AuthSubscription GetSubscription(string? id, PaymentProvider provider)
    {
        var sub = Subscriptions.SingleOrDefault(s => s.SubscriptionId == id);
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
        else
        {
            Subscriptions.Add(subscription);
        }
    }

    public void UpdateSubscription(AuthSubscription subscription)
    {
        if (subscription.SubscriptionId.Empty()) throw new UnhandledException("subscription id is null");

        var sub = Subscriptions.SingleOrDefault(sub => sub.SubscriptionId == subscription.SubscriptionId);

        if (sub == null)
        {
            throw new NotificationException("Subscription not found.");
        }
        else if (Subscriptions.Any(p => p.IsActive() && p.SubscriptionId != sub.SubscriptionId))
        {
            throw new NotificationException("There is already an active subscription. Please deactivate the old one first before creating a new one.");
        }
        else
        {
            sub.CustomerId = subscription.CustomerId;
            sub.SessionId = subscription.SessionId;
            sub.ExpiresDate = subscription.ExpiresDate;
            sub.Active = subscription.Active;
            sub.Provider = subscription.Provider;
            sub.Product = subscription.Product;
            sub.Cycle = subscription.Cycle;
        }
    }
}

public class AuthSubscription
{
    public string? SubscriptionId { get; set; }
    public string? CustomerId { get; set; }
    public string? SessionId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpiresDate { get; set; }
    public bool Active { get; set; } = false;

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

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not AuthSubscription other) return false;

        return string.Equals(SubscriptionId, other.SubscriptionId, StringComparison.Ordinal) && string.Equals(SessionId, other.SessionId, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SubscriptionId, SessionId);
    }
}

public class Event(string? origin, string? description, string? ip)
{
    public string? Origin { get; set; } = origin;
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
    public string? Description { get; set; } = description;
    public string? Ip { get; set; } = ip;
}