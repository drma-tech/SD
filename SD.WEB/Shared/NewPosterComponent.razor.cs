using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Globalization;

namespace SD.WEB.Shared
{
    public partial class NewPosterComponent
    {
        [Parameter] public MediaDetail? MediaDetail { get; set; }

        [Parameter] public EventCallback<MediaDetail> Clicked { get; set; }
        [Parameter] public bool ForceShowTitle { get; set; }
        [Parameter] public bool FullPage { get; set; } = false;
        [Parameter] public Typo? ForceTypoTitle { get; set; }

        // TOP LEFT

        [Parameter] public bool OnlyYear { get; set; }
        [Parameter] public int? Percent { get; set; }

        // CENTER LEFT/RIGHT

        [Parameter] public bool ShowWished { get; set; } = true;
        [Parameter][EditorRequired] public WishList? WishList { get; set; }
        [Parameter] public bool ShowWatched { get; set; } = true;
        [Parameter] public WatchingList? WatchingList { get; set; }

        //BOTTOM

        [Parameter] public string? CommentsSeparator { get; set; } = ",";
        [Parameter] public bool CommentsIsImage { get; set; }

        private PosterBadge? TopLeft { get; set; }
        private PosterBadge? TopRight { get; set; }
        private PosterBadge? CenterLeft { get; set; }
        private PosterBadge? CenterRight { get; set; }

        protected override void OnParametersSet()
        {
            SetTopLeft();
            SetTopRight();
            SetCenterLeft();
            SetCenterRight();
        }

        private async Task DivClicked()
        {
            if (Clicked.HasDelegate)
            {
                await Clicked.InvokeAsync(MediaDetail);
            }
        }

        private void SetTopLeft()
        {
            if (MediaDetail == null) return;

            if (MediaDetail.runtime.HasValue)
            {
                TopLeft = new PosterBadge
                {
                    Text = MediaDetail.runtime.FormatRuntime(),
                };
            }
            else if (MediaDetail.release_date.HasValue)
            {
                TopLeft = new PosterBadge
                {
                    Text = MediaDetail.release_date.Value < DateTime.Now.AddMonths(-3) || OnlyYear ? MediaDetail.release_date.Value.Year.ToString(System.Globalization.CultureInfo.InvariantCulture) : MediaDetail.release_date.Value.ToShortDateString(),
                };
            }
            else if (Percent.HasValue)
            {
                TopLeft = new PosterBadge
                {
                    Text = string.Create(CultureInfo.InvariantCulture, $"{Percent.Value}%"),
                    Color = Percent == 100 ? Color.Success : Color.Warning,
                };
            }
        }

        private void SetTopRight()
        {
            if (MediaDetail == null) return;

            if (MediaDetail.rating.HasValue)
            {
                if (MediaDetail.rating >= 7.95)
                {
                    TopRight = new PosterBadge
                    {
                        Text = string.Create(CultureInfo.InvariantCulture, $"{MediaDetail.rating.Value:#.#}"),
                        Color = Color.Success,
                    };
                }
                else if (MediaDetail.rating >= 5.95)
                {
                    TopRight = new PosterBadge
                    {
                        Text = string.Create(CultureInfo.InvariantCulture, $"{MediaDetail.rating.Value:#.#}"),
                        Color = Color.Warning,
                    };
                }
                else if (MediaDetail.rating > 0)
                {
                    TopRight = new PosterBadge
                    {
                        Text = string.Create(CultureInfo.InvariantCulture, $"{MediaDetail.rating.Value:#.#}"),
                        Color = Color.Error,
                    };
                }
            }
        }

        private void SetCenterLeft()
        {
            if (MediaDetail == null) return;

            if (ShowWished)
            {
                var wished = WishList?.Contains(MediaDetail.MediaType, MediaDetail.tmdb_id) ?? false;

                if (wished)
                {
                    CenterLeft = new PosterBadge
                    {
                        Icon = Icons.Material.Filled.Bookmark,
                        Color = Color.Dark,
                    };
                }
            }
        }

        private void SetCenterRight()
        {
            if (MediaDetail == null) return;

            if (ShowWatched)
            {
                var watched = MediaDetail.collectionId.HasValue &&
                    (WatchingList?.GetWatchingItems(MediaDetail.MediaType, MediaDetail.collectionId.Value.ToString(CultureInfo.InvariantCulture)).Contains(MediaDetail.tmdb_id ?? "") ?? false);

                if (watched)
                {
                    CenterRight = new PosterBadge
                    {
                        Icon = Icons.Material.Filled.RemoveRedEye,
                        Color = Color.Dark,
                    };
                }
            }
        }
    }

    public class PosterBadge
    {
        public string? Icon { get; set; }
        public string? Text { get; set; }
        public Color Color { get; set; } = Color.Info;
    }
}