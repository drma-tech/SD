using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Globalization;

namespace SD.WEB.Shared
{
    public partial class PosterComponent
    {
        [Parameter] public MediaType? MediaType { get; set; }
        [Parameter] public string? Poster { get; set; }
        [Parameter] public string? Title { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }
        [Parameter] public bool ForceShowTitle { get; set; }
        [Parameter] public bool FullPage { get; set; } = false;
        [Parameter] public Typo? ForceTypoTitle { get; set; }

        // TOP LEFT

        [Parameter] public DateTime? Date { get; set; }
        [Parameter] public bool OnlyYear { get; set; }
        [Parameter] public int? Runtime { get; set; }
        [Parameter] public int? Percent { get; set; }

        // TOP RIGHT

        [Parameter] public double? Rating { get; set; }

        // CENTER LEFT/RIGHT

        [Parameter] public string? TmdbId { get; set; }
        [Parameter] public bool ShowWished { get; set; } = true;
        [Parameter][EditorRequired] public WishList? WishList { get; set; }
        [Parameter] public bool ShowWatched { get; set; } = true;
        [Parameter] public string? CollectionId { get; set; }
        [Parameter] public WatchingList? WatchingList { get; set; }

        //BOTTOM

        [Parameter] public string? Comments { get; set; }
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

        private void SetTopLeft()
        {
            if (Runtime.HasValue)
            {
                TopLeft = new PosterBadge
                {
                    Text = @Runtime.FormatRuntime(),
                };
            }
            else if (Date.HasValue)
            {
                TopLeft = new PosterBadge
                {
                    Text = Date.Value < DateTime.Now.AddMonths(-3) || OnlyYear ? Date.Value.Year.ToString(System.Globalization.CultureInfo.InvariantCulture) : Date.Value.ToShortDateString(),
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
            if (Rating.HasValue)
            {
                if (Rating >= 7.95)
                {
                    TopRight = new PosterBadge
                    {
                        Text = string.Create(CultureInfo.InvariantCulture, $"{Rating.Value:#.#}"),
                        Color = Color.Success,
                    };
                }
                else if (Rating >= 5.95)
                {
                    TopRight = new PosterBadge
                    {
                        Text = string.Create(CultureInfo.InvariantCulture, $"{Rating.Value:#.#}"),
                        Color = Color.Warning,
                    };
                }
                else if (Rating > 0)
                {
                    TopRight = new PosterBadge
                    {
                        Text = string.Create(CultureInfo.InvariantCulture, $"{Rating.Value:#.#}"),
                        Color = Color.Error,
                    };
                }
            }
        }

        private void SetCenterLeft()
        {
            if (ShowWished)
            {
                var wished = WishList?.Contains(MediaType, TmdbId) ?? false;

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
            if (ShowWatched)
            {
                var watched = CollectionId.NotEmpty() && (WatchingList?.GetWatchingItems(MediaType, CollectionId).Contains(TmdbId!) ?? false);

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
}