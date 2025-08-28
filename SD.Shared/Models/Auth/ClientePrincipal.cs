using System.ComponentModel.DataAnnotations;

namespace SD.Shared.Models.Auth;

public class ClientePrincipal() : PrivateMainDocument(DocumentType.Principal)
{
    public string? UserId { get; set; }
    public string? IdentityProvider { get; set; }
    public string? DisplayName { get; set; }
    [DataType(DataType.EmailAddress)] public string? Email { get; set; }

    public ClientePaddle? ClientePaddle { get; set; }

    public override void Initialize(string userId)
    {
        base.Initialize(userId);
        UserId = userId;
    }
}
