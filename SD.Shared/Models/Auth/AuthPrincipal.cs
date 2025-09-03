using System.ComponentModel.DataAnnotations;

namespace SD.Shared.Models.Auth;

public class AuthPrincipal() : PrivateMainDocument(DocumentType.Principal)
{
    public string? UserId { get; set; }
    public string? IdentityProvider { get; set; }
    public string? DisplayName { get; set; }
    [DataType(DataType.EmailAddress)] public string? Email { get; set; }

    public AuthPaddle? AuthPaddle { get; set; }
    public Event[] Events { get; set; } = [];

    public override void Initialize(string userId)
    {
        base.Initialize(userId);
        UserId = userId;
    }
}

public class Event
{
    public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;
    public string? Description { get; set; }
}