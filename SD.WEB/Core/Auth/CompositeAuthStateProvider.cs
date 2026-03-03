using Microsoft.AspNetCore.Components.Authorization;

namespace SD.WEB.Core.Auth
{
    public sealed class CompositeAuthStateProvider(SupabaseAuthStateProvider supabase) : AuthenticationStateProvider
    {
        public void OnSupabaseAuthChanged(string? token)
        {
            supabase.NotifyAuthenticationStateChanged(token);
            NotifyAuthenticationStateChanged(supabase.GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return supabase.GetAuthenticationStateAsync();
        }
    }
}