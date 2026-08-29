using Microsoft.JSInterop;
using SD.WEB.Api.Core;

namespace SD.WEB.Core.Javascript
{
    public class ClerkJs(IJSRuntime js) : JsModuleBase(js, "./js/clerk.js")
    {
        public async Task SignInAsync(CancellationToken cancellationToken)
        {
            ApiCore.ResetCacheVersion();
            await InvokeVoid("authentication.signIn", cancellationToken);
        }

        public Task SignOutAsync(CancellationToken cancellationToken) => InvokeVoid("authentication.signOut", cancellationToken);

        public Task AccountPopup(CancellationToken cancellationToken) => InvokeVoid("authentication.accountPopup", cancellationToken);
    }
}