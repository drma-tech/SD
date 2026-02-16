using Microsoft.AspNetCore.Components.Authorization;

namespace SD.WEB.Core.Auth
{
    public sealed class CompositeAuthStateProvider(FirebaseAuthStateProvider firebase, SupabaseAuthStateProvider supabase) : AuthenticationStateProvider
    {
        public void OnFirebaseAuthChanged(string? token)
        {
            firebase.NotifyAuthenticationStateChanged(token);
            NotifyAuthenticationStateChanged(firebase.GetAuthenticationStateAsync());
        }

        public void OnSupabaseAuthChanged(string? token)
        {
            supabase.NotifyAuthenticationStateChanged(token);
            NotifyAuthenticationStateChanged(supabase.GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (AppStateStatic.FirebaseToken != null)
                return firebase.GetAuthenticationStateAsync();
            else
                return supabase.GetAuthenticationStateAsync();
        }
    }
}