using Microsoft.JSInterop;

namespace SD.WEB.Core.Helper.Javascript
{
    public class SupabaseJs(IJSRuntime js) : JsModuleBase(js, "./js/supabase.js")
    {
        public Task<string> CreateUserAsync(string? id, string? email, string? name, CancellationToken cancellationToken) => Invoke<string>("authentication.createUser", cancellationToken, id, email, name);

        public async Task SignInAsync(string providerName, string? returnUrl, CancellationToken cancellationToken)
        {
            ApiCore.ResetCacheVersion();
            await InvokeVoid("authentication.signIn", cancellationToken, providerName, returnUrl);
        }

        public async Task ConfirmCode(string email, string code, CancellationToken cancellationToken)
        {
            ApiCore.ResetCacheVersion();
            await InvokeVoid("authentication.confirmCode", cancellationToken, email, code);
        }

        public Task SignOutAsync(CancellationToken cancellationToken) => InvokeVoid("authentication.signOut", cancellationToken);
    }
}