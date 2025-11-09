using Microsoft.JSInterop;
using SD.WEB.Layout;

namespace SD.WEB.Core.Service
{
    public class FirebaseAuthService(IJSRuntime js)
    {
        public async Task<string?> SignInAsync() => await js.InvokeAsync<string?>("firebaseAuth.signIn");

        public async Task SignOutAsync() => await js.InvokeVoidAsync("firebaseAuth.signOut");

        public async Task SubscribeToAuthChanges(DotNetObjectReference<MainLayout> dotNetRef) => await js.InvokeVoidAsync("firebaseAuth.onAuthChanged", dotNetRef);
    }
}