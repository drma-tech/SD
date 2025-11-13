using Microsoft.JSInterop;

namespace SD.WEB.Core.Auth
{
    public class FirebaseAuthService(IJSRuntime js)
    {
        public async Task<string?> SignInAsync(string provider)
        {
            ApiCore.ResetCacheVersion();
            return await js.InvokeAsync<string?>("firebaseAuth.signIn", provider);
        }

        public async Task SignOutAsync()
        {
            ApiCore.ResetCacheVersion();
            await js.InvokeVoidAsync("firebaseAuth.signOut");
        }
    }
}