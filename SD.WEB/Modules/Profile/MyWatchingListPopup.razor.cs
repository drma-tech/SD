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
            WatchingApi.DataChanged += async model =>
            {
                await State.StartLoading.Invoke(null);
                Watching = model;
                await WatchingChanged.InvokeAsync(model);
                await State.FinishLoading.Invoke(model);
                StateHasChanged();
            };
            WishApi.DataChanged += async model =>
            {
                Wish = model;
                await WishChanged.InvokeAsync(model);
                await InvokeAsync(StateHasChanged);
            };
        }
    }
}