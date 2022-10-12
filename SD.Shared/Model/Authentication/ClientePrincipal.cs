using SD.Shared.Core;
using System.ComponentModel.DataAnnotations;

namespace SD.Shared.Modal.Authentication
{
    public class ClientePrincipal : CosmosBase
    {
        public ClientePrincipal() : base(CosmosType.Principal)
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

        public override void SetIds(string? IdLoggedUser)
        {
            if (IdLoggedUser == null) return;

            SetId(IdLoggedUser);
            SetPartitionKey(IdLoggedUser);
            UserId = IdLoggedUser;
        }
    }
}