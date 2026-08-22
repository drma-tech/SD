using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SD.WEB.Shared
{
    public partial class CompleteListPopup
    {
        [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

        [Parameter][EditorRequired] public string? TitleHead { get; set; }
        [Parameter][EditorRequired] public WatchingList? Watching { get; set; }
        [Parameter][EditorRequired] public WishList? Wish { get; set; }
        [Parameter][EditorRequired] public string? Culture { get; set; }
        [Parameter] public EventCallback<WatchingList?> WatchingChanged { get; set; }
        [Parameter] public EventCallback<WishList?> WishChanged { get; set; }

        [Parameter] public ISet<MediaDetail> Items { get; set; } = new HashSet<MediaDetail>();
        [Parameter] public EventCallback<ISet<MediaDetail>> ItemsChanged { get; set; }
        [Parameter] public RenderControlState<ISet<MediaDetail>> State { get; set; } = new(new HashSet<MediaDetail>(), list => list == null || list.Empty());

        [Parameter] public IMediaListApi? MediaListApi { get; set; }
        [Parameter] public EnumLists? List { get; set; }
        [Parameter] public bool IsImdb { get; set; }
        [Parameter] public MediaType? TypeSelected { get; set; }
        [Parameter] public IDictionary<string, string> StringParameters { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        [Parameter] public bool CommentsIsImage { get; set; }

        protected override void OnInitialized()
        {
            if (List == null && string.IsNullOrEmpty(TitleHead)) throw new NotificationException("Title is required");

            WatchingListApi.DataChanged += model =>
            {
                Watching = model;
                _ = WatchingChanged.InvokeAsync(model);
                StateHasChanged();
            };
            WishListApi.DataChanged += model =>
            {
                Wish = model;
                _ = WishChanged.InvokeAsync(model);
                StateHasChanged();
            };
        }

        public void HideModal()
        {
            MudDialog?.Close();
        }
    }
}