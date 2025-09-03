using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core
{
    public class SuperData
    {
        public AuthPrincipal? Principal { get; set; }
        public AuthLogin? Login { get; set; }
    }
}