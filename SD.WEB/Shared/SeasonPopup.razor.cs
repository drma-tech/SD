using Microsoft.AspNetCore.Components;
using MudBlazor;
using SD.Shared.Models.List.Tmdb;

namespace SD.WEB.Shared
{
    public partial class SeasonPopup
    {
        [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

        [Parameter] public string? TmdbId { get; set; }
        [Parameter] public int? SeasonNumber { get; set; }

        public RenderControlState<TmdbSeason> Actions { get; set; } = new(obj => obj == null || obj.episodes.Empty());
        public TmdbSeason? Season { get; set; }

        protected override async Task<bool> LoadInteropDataAsync(Microsoft.JSInterop.IJSRuntime JsRuntime)
        {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", (await AppStateStatic.GetContentLanguage(JsRuntime, Cts.Token)).GetFieldSettings(false).Name ?? "en-US" },
            };

            await Actions.StartLoading.Invoke(null);
            Season = await TmdbApi.GetSeason(TmdbId, SeasonNumber, parameters, Cts.Token);
            await Actions.FinishLoading.Invoke(Season);

            return true;
        }

        public void HideModal()
        {
            MudDialog?.Close();
        }
    }
}