using Microsoft.AspNetCore.Components;

namespace SD.WEB.Modules.Profile
{
    public partial class WishlistComponent
    {
        [Parameter][EditorRequired] public RenderControlState<WishList> MovieState { get; set; }
        [Parameter][EditorRequired] public RenderControlState<WishList> TvState { get; set; }
        [Parameter][EditorRequired] public bool ShowHeader { get; set; }
        [Parameter][EditorRequired] public bool FullScreen { get; set; }
        [Parameter][EditorRequired] public WatchingList? Watching { get; set; }
        [Parameter][EditorRequired] public WishList? Wish { get; set; }
        [Parameter][EditorRequired] public string? Culture { get; set; }

        [Parameter] public MediaType? TypeParam { get; set; }
        [Parameter] public string? CustomTitle { get; set; }

        private MediaType _type { get; set; } = MediaType.movie;

        private ISet<WishListItem> Items(MediaType type) => type == MediaType.movie ? Wish?.Movies ?? new HashSet<WishListItem>() : Wish?.Shows ?? new HashSet<WishListItem>();

        private int GetTotalItems => FullScreen ? AccountProduct.Premium.GetRestrictions().Wishlist : 7;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MovieState.CustomMessageWarning = Translations.Module.Profile.AddTitlesWishlist;
            TvState.CustomMessageWarning = Translations.Module.Profile.AddTitlesWishlist;
        }

        private async Task OpenCompleteList(MediaType type)
        {
            await DialogService.MyWishListPopup(type == MediaType.movie ? MovieState : TvState, Watching, Wish, type, Culture);
        }

        public async Task ShowMediaPopup(MediaType type, string? tmdbId, string? name)
        {
            if (tmdbId.NotEmpty()) await DialogService.MediaPopup(Watching, Wish, type, tmdbId, Culture);
        }
    }
}