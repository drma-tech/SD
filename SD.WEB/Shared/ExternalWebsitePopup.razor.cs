using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SD.WEB.Shared;

public partial class ExternalWebsitePopup
{
    [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

    [Parameter, EditorRequired] public string ExternalUrl { get; set; } = string.Empty;

    private void Cancel()
    {
        MudDialog?.Cancel();
    }

    private void Continue()
    {
        MudDialog?.Close(DialogResult.Ok(result: true));
    }
}