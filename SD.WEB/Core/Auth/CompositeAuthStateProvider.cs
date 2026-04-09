using Microsoft.AspNetCore.Components.Authorization;

namespace SD.WEB.Core.Auth
{
    public sealed class CompositeAuthStateProvider(SupabaseAuthStateProvider supabase) : AuthenticationStateProvider
    {
        public void OnSupabaseAuthChanged(string? token)
        {
            supabase.GenerateClaimsIdentity(token);
            NotifyAuthenticationStateChanged(supabase.GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return supabase.GetAuthenticationStateAsync();
        }
    }
}