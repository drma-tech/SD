using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SD.WEB.Modules.Profile
{
    public partial class MyWatchingListPopup
    {
        [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

        [Parameter][EditorRequired] public RenderControlState<WatchingList?> State { get; set; }
        [Parameter][EditorRequired] public WatchingList? Watching { get; set; }
        [Parameter][EditorRequired] public WishList? Wish { get; set; }
        [Parameter][EditorRequired] public string? Culture { get; set; }

        [Parameter] public EventCallback<WatchingList?> WatchingChanged { get; set; }
        [Parameter] public EventCallback<WishList?> WishChanged { get; set; }
        [Parameter][EditorRequired] public MediaType MediaType { get; set; }

        protected override void OnInitialized()
        {
            WatchingApi.DataChanged += model =>
            {
                _ = (State.StartLoading.Invoke(null));
                Watching = model;
                _ = WatchingChanged.InvokeAsync(model);
                _ = (State.FinishLoading.Invoke(model));
                StateHasChanged();
            };
            WishApi.DataChanged += model =>
            {
                Wish = model;
                _ = WishChanged.InvokeAsync(model);
                StateHasChanged();
            };
        }
    }
}