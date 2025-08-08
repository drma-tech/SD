using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core
{
    public class SuperData
    {
        public ClientePrincipal? Principal { get; set; }
        public ClienteLogin? Login { get; set; }
    }
}