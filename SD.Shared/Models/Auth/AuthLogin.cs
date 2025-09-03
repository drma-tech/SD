namespace SD.Shared.Models.Auth;

public class AuthLogin() : PrivateMainDocument(DocumentType.Login)
{
    public string? UserId { get; set; }

    public Access[] Accesses { get; set; } = [];

    public override void Initialize(string userId)
    {
        base.Initialize(userId);
        UserId = userId;
    }
}

public class Access
{
    public DateTimeOffset Date { get; set; }
    public string? Platform { get; set; }
    public string? Ip { get; set; }
}
