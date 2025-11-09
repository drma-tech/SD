using Microsoft.JSInterop;

namespace SD.WEB.Core.Auth
{
    public class FirebaseAuthService(IJSRuntime js)
    {
        public async Task<string?> SignInAsync() => await js.InvokeAsync<string?>("firebaseAuth.signIn");

        public async Task SignOutAsync() => await js.InvokeVoidAsync("firebaseAuth.signOut");
    }
}