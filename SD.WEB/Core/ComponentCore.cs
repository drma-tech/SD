using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using SD.WEB.Modules.Auth.Core;

namespace SD.WEB.Core;

/// <summary>
///     if you implement the OnAfterRenderAsync method, call 'await base.OnAfterRenderAsync(firstRender);'
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ComponentCore<T> : ComponentBase where T : class
{
    [Inject] protected ILogger<T> Logger { get; set; } = null!;
    [Inject] protected ISnackbar Snackbar { get; set; } = null!;
    [Inject] protected IDialogService DialogService { get; set; } = null!;
    [Inject] protected NavigationManager Navigation { get; set; } = null!;
    [Inject] protected PrincipalApi PrincipalApi { get; set; } = null!;
    [Inject] protected CacheSettingsApi CacheSettingsApi { get; set; } = null!;

    protected virtual Task LoadDataRender()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     if you implement the OnAfterRenderAsync method, call 'await base.OnAfterRenderAsync(firstRender);'
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await LoadDataRender();

                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            ex.ProcessException(Snackbar, Logger);
        }
    }
}

/// <summary>
///     if you implement the OnAfterRenderAsync method, call 'await base.OnAfterRenderAsync(firstRender);'
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class PageCore<T> : ComponentCore<T> where T : class
{
    [CascadingParameter] protected Task<AuthenticationState>? AuthenticationState { get; set; }

    protected bool IsAuthenticated { get; set; }
    protected ClaimsPrincipal? User { get; set; }
    protected string? UserId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationState is not null)
        {
            var authState = await AuthenticationState;

            User = authState.User;
            IsAuthenticated = User?.Identity is not null && User.Identity.IsAuthenticated;
            UserId = User?.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}