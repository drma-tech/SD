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

    public AuthSubscription? Subscription { get; set; }
    public Event[] Events { get; set; } = [];

    public override void Initialize(string userId)
    {
        base.Initialize(userId);
        UserId = userId;
    }
}

public class AuthSubscription
{
    public string? SubscriptionId { get; set; }
    public string? CustomerId { get; set; }
    public string? LatestReceipt { get; set; }
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
            PaymentProvider.Apple => ExpiresDate.HasValue && ExpiresDate.Value > DateTimeOffset.UtcNow,
            _ => throw new NotificationException("invalid provider"),
        };
    }
}

public class Event
{
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
    public string? Description { get; set; }
}