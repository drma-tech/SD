namespace SD.API.Core.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true)]
    public class AuthorizeAttribute(params string[] roles) : Attribute
    {
        public string[] Roles { get; } = roles;
    }
}