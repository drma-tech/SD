using Microsoft.JSInterop;

namespace SD.WEB.Core.Auth
{
    public class FirebaseAuthService(IJSRuntime js)
    {
        public async Task SignInAsync(string provider)
        {
            ApiCore.ResetCacheVersion();
            await js.InvokeVoidAsync("window.firebaseAuth.signIn", provider);
        }

        public async Task SignOutAsync()
        {
            await js.InvokeVoidAsync("window.firebaseAuth.signOut");
        }
    }
}