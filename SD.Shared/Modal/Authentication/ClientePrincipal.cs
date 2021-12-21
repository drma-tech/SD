using SD.Shared.Core;
using System.ComponentModel.DataAnnotations;

namespace SD.Shared.Modal.Authentication
{
    public class ClientePrincipal : CosmosBase
    {
        public ClientePrincipal(string UserId, string IdentityProvider, string UserDetails, string Email) : base(CosmosType.Principal)
        {
            this.UserId = UserId;
            this.IdentityProvider = IdentityProvider;
            this.UserDetails = UserDetails;
            this.Email = Email;
        }

        public string UserId { get; set; }
        public string IdentityProvider { get; set; }
        public string UserDetails { get; set; }
        public string[] UserRoles { get; set; } = Array.Empty<string>();

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? Mobile { get; set; }

        public bool Blocked { get; set; }

        public override void SetIds(string IdLoggedUser)
        {
            SetId(IdLoggedUser);
            SetPartitionKey(IdLoggedUser);
            UserId = IdLoggedUser;
        }
    }
}