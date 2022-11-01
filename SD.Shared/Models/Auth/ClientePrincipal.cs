using System.ComponentModel.DataAnnotations;

namespace SD.Shared.Model.Auth
{
    public class ClientePrincipal : DocumentBase
    {
        public ClientePrincipal() : base(DocumentType.Principal, true)
        {
        }

        public string? UserId { get; set; }
        public string? IdentityProvider { get; set; }
        public string? UserDetails { get; set; }
        public string[] UserRoles { get; set; } = Array.Empty<string>();

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? Mobile { get; set; }

        public bool Blocked { get; set; }

        public override void SetIds(string id)
        {
            SetValues(id);
            UserId = id.ToString();
        }
    }
}