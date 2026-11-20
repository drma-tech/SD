using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Runtime.CompilerServices;

namespace SD.WEB.Core.Component;

/// <summary>
/// There is a memory cost when implementing this class. Use it when necessary.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ComponentCore<T> : ComponentBase, IDisposable where T : class
{
    [Inject] private ILogger<T> Logger { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Inject] protected IDialogService DialogService { get; set; } = null!;
    [Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] protected NavigationManager Navigation { get; set; } = null!;

    protected CancellationTokenSource Cts { get; } = new();
    protected TaskHelper TaskHelper { get; } = new();
    protected virtual bool ShowExceptions => false;

    /// <summary>
    /// To load static data that does not change and does not depend on parameters.
    ///
    /// Note: Omitting the await (_ = task) can cause static page generation to fail.
    /// </summary>
    /// <returns></returns>
    protected virtual Task LoadStaticDataAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// To process data that depends on the DOM, JavaScript, or element references.
    ///
    /// Note: Returns true when the component state changed and a re-render is required.
    /// </summary>
    /// <returns></returns>
    protected virtual Task<bool> LoadInteropDataAsync(Microsoft.JSInterop.IJSRuntime JsRuntime)
    {
        return Task.FromResult(false);
    }

    /// <summary>
    /// Exclusive for data associated with authenticated users (will be called every time the state changes)
    ///
    /// Note: All APIs should check if the user is logged in or not.
    /// </summary>
    /// <returns></returns>
    protected virtual Task LoadAuthenticatedDataAsync(CancellationToken token)
    {
        return Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();

            AppStateStatic.BreakpointChanged.Subscribe((bp) => StateHasChanged(), Cts.Token);
            AppStateStatic.UserStateChanged.Subscribe(async () =>
            {
                await TaskHelper.RunSingleAsync("LoadAuthenticatedDataAsync", AppStateStatic.IsAuthenticated, LoadAuthenticatedDataAsync, Cts.Token); //AuthenticationStateChanged
                await InvokeAsync(StateHasChanged);
            }, Cts.Token);

            await LoadStaticDataAsync();
            await TaskHelper.RunSingleAsync("LoadAuthenticatedDataAsync", AppStateStatic.IsAuthenticated, LoadAuthenticatedDataAsync, Cts.Token); //start or route changes
        }
        catch (Exception ex)
        {
            await ProcessException(ex, ShowExceptions);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && await LoadInteropDataAsync(JsRuntime))
            {
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            await ProcessException(ex, ShowExceptions);
        }
    }

    protected static bool IsSafeExternalUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) || string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase));
    }

    public async Task OpenExternalWebsite(string? url)
    {
        try
        {
            if (!IsSafeExternalUrl(url)) return;

            var parameters = new DialogParameters<ExternalWebsitePopup>
            {
                { x => x.ExternalUrl, url! },
            };

            var dialog = await DialogService.ShowAsync<ExternalWebsitePopup>(
                Translations.Notification.ExternalWebsiteTitle,
                parameters,
                PopupHelper.Options(MaxWidth.Small));

            var result = await dialog.Result;

            if (result?.Data is true)
            {
                var targetUrl = AddReferralParameter(url!);
                await JsRuntime.Window().InvokeVoidAsync("open", Cts.Token, targetUrl, "_blank");
            }
        }
        catch (Exception ex)
        {
            await ProcessException(ex);
        }
    }

    private static string AddReferralParameter(string url)
    {
        var uri = new UriBuilder(url);
        var query = uri.Query.TrimStart('?');
        var referral = $"ref={Uri.EscapeDataString($"{AppInfo.Domain}.com")}";

        uri.Query = string.IsNullOrEmpty(query) ? referral : $"{query}&{referral}";

        return uri.Uri.ToString();
    }

    #region notification module

    protected async Task ShowInfo(string message)
    {
        if (!message.CanShowSnackbar()) return;

        Snackbar.Add(message, Severity.Info);

        await JsRuntime.Utils().PlayBeep(600, 120, "sine", CancellationToken.None);
        await JsRuntime.Utils().Vibrate([50], CancellationToken.None);
    }

    protected async Task ShowInfo(RenderFragment message)
    {
        Snackbar.Add(message, Severity.Info);

        await JsRuntime.Utils().PlayBeep(600, 120, "sine", CancellationToken.None);
        await JsRuntime.Utils().Vibrate([50], CancellationToken.None);
    }

    protected async Task ShowSuccess(string message)
    {
        if (!message.CanShowSnackbar()) return;

        Snackbar.Add(message, Severity.Success);

        await JsRuntime.Utils().PlayBeep(880, 100, "sine", CancellationToken.None);
        await JsRuntime.Utils().Vibrate([40], CancellationToken.None);
    }

    protected async Task ShowWarning(string message)
    {
        if (!message.CanShowSnackbar()) return;

        Snackbar.Add(message, Severity.Warning);

        await JsRuntime.Utils().PlayBeep(440, 200, "triangle", CancellationToken.None);
        await JsRuntime.Utils().Vibrate([100, 80, 100], CancellationToken.None);
    }

    protected async Task ShowError(string message)
    {
        if (!message.CanShowSnackbar()) return;

        Snackbar.Add(message, Severity.Error);

        await JsRuntime.Utils().PlayBeep(220, 400, "square", CancellationToken.None);
        await JsRuntime.Utils().Vibrate([200, 100, 200], CancellationToken.None);
    }

    protected async Task ProcessException(Exception ex, bool showMessage = true, [CallerMemberName] string caller = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        if (ex is NotificationException exc)
        {
            Logger.Warning(exc.Message);
            if (showMessage) await ShowWarning(exc.Message);
        }
        else if (ex is OperationCanceledException or TaskCanceledException or ObjectDisposedException)
        {
            //ignored
        }
        else
        {
            ex.Data["ProcessException.caller"] = caller;
            ex.Data["ProcessException.file"] = Path.GetFileName(file);
            ex.Data["ProcessException.line"] = line;

            Logger.Error(ex, ex.Message);
            if (showMessage) await ShowError(ex.Message);
        }
    }

    #endregion notification module

    #region Dispose

    private bool isDisposed;

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed) return;

        if (disposing)
        {
            Cts.Cancel();
            Cts.Dispose();
        }

        isDisposed = true;
    }

    #endregion Dispose
}
