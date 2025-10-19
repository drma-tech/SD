using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Security.Claims;
using System.Text.Json;

namespace SD.WEB.Core
{
    public class CustomUserFactory(IAccessTokenProviderAccessor accessor) : AccountClaimsPrincipalFactory<RemoteUserAccount>(accessor)
    {
        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);
            if (account?.AdditionalProperties == null)
                return user;

            var identity = (ClaimsIdentity)user.Identity!;

            if (account.AdditionalProperties.TryGetValue("roles", out var rolesObj))
            {
                if (rolesObj is JsonElement je)
                {
                    if (je.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in je.EnumerateArray())
                        {
                            var role = item.GetString();
                            if (!string.IsNullOrWhiteSpace(role))
                                identity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                    }
                    else if (je.ValueKind == JsonValueKind.String)
                    {
                        var s = je.GetString() ?? "";
                        foreach (var role in s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                            identity.AddClaim(new Claim(ClaimTypes.Role, role.Trim()));
                    }
                }
                else if (rolesObj is string s)
                {
                    foreach (var role in s.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        identity.AddClaim(new Claim(ClaimTypes.Role, role.Trim()));
                }
            }

            if (account.AdditionalProperties.TryGetValue("oid", out var id))
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id.ToString()!));
            }

            return user;
        }
    }
}