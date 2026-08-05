using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth
{
    public partial class RegisterUser
    {
        private bool _terms1 { get; set; } 
        private bool _terms2 { get; set; } 

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                if (AppStateStatic.IsAuthenticated)
                {
                    if (AppStateStatic.Principal == null) //first access
                    {
                        //wait the user answer the terms of use (call CreateAccount)
                    }
                    else //just do login
                    {
                        await RegisterLogin(firstAccess: false);
                    }
                }
                else
                {
                    Navigation.NavigateTo($"/{Culture}");
                }
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task CreateAccount()
        {
            try
            {
                if (AppStateStatic.Principal != null) throw new InvalidOperationException("Principal is already set.");
                if (AppStateStatic.User == null) throw new InvalidOperationException("User is null.");

                AppStateStatic.Principal = new AuthPrincipal(AppStateStatic.UserId)
                {
                    AuthProviders = [AppStateStatic.User.FindFirst("idp")!.Value],
                    DisplayName = AppStateStatic.User.FindFirst("name")?.Value,
                    Email = AppStateStatic.User.FindFirst("email")?.Value,
                };

                if (_terms1)
                {
                    AppStateStatic.Principal.Events.Add(new Event(AppInfo.Title, "Terms of use accepted.", ip: null));
                }

                var platform = await AppStateStatic.GetPlatform(JsRuntime, Cts.Token);
                var country = await AppStateStatic.GetCountry(IpInfoApi, JsRuntime, Cts.Token);

                AppStateStatic.Principal = await PrincipalApi.Add(AppStateStatic.Principal, platform, country, Cts.Token) ?? throw new InvalidOperationException("Failed to create principal.");

                await RegisterLogin(firstAccess: true);
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task RegisterLogin(bool firstAccess)
        {
            if (!firstAccess)
            {
                var platform = await AppStateStatic.GetPlatform(JsRuntime, Cts.Token);
                var country = await AppStateStatic.GetCountry(IpInfoApi, JsRuntime, Cts.Token);

                await LoginApi.Add(platform, country, Cts.Token);
            }

            if (AppStateStatic.Principal!.AuthProviders.Empty() || !AppStateStatic.Principal!.AuthProviders.Contains(AppStateStatic.User!.FindFirst("idp")!.Value)) //if its a new auth provider
            {
                AppStateStatic.Principal.AuthProviders = [.. AppStateStatic.Principal.AuthProviders.Union([AppStateStatic.User!.FindFirst("idp")!.Value], StringComparer.OrdinalIgnoreCase)];
                await PrincipalApi.Update(AppStateStatic.Principal, Cts.Token);
            }

            Navigation.NavigateTo($"/{Culture}/profile");
        }

        private async Task Logout()
        {
            await JsRuntime.Supabase().SignOutAsync(Cts.Token);

            Navigation.NavigateTo($"/{Culture}");

            await JsRuntime.Utils().ClearAllStorage();
        }
    }
}