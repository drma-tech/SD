using Microsoft.AspNetCore.Components.Authorization;

namespace SD.WEB.Core.Auth
{
    public sealed class CompositeAuthStateProvider : AuthenticationStateProvider
    {
        private readonly FirebaseAuthStateProvider _firebase;
        private readonly SupabaseAuthStateProvider _supabase;

        public CompositeAuthStateProvider(FirebaseAuthStateProvider firebase, SupabaseAuthStateProvider supabase)
        {
            _firebase = firebase;
            _supabase = supabase;

            AppStateStatic.FirebaseAuthChanged += OnFirebaseAuthChanged;
            AppStateStatic.SupabaseAuthChanged += OnSupabaseAuthChanged;
        }

        public void OnFirebaseAuthChanged(string? token)
        {
            _firebase.NotifyAuthenticationStateChanged(token);
            NotifyAuthenticationStateChanged(_firebase.GetAuthenticationStateAsync());
        }

        public void OnSupabaseAuthChanged(string? token)
        {
            _supabase.NotifyAuthenticationStateChanged(token);
            NotifyAuthenticationStateChanged(_supabase.GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (AppStateStatic.FirebaseToken != null)
                return _firebase.GetAuthenticationStateAsync();
            else
                return _supabase.GetAuthenticationStateAsync();
        }
    }
}