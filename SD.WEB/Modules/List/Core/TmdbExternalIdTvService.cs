namespace SD.WEB.Modules.List.Core
{
    public static class TmdbExternalIdTvService
    {
        public static async Task<string?> GetTmdbId(HttpClient Http, string? imdb_id)
        {
            if (imdb_id == null) throw new ArgumentNullException(nameof(imdb_id));

            return await Http.Get<string>($"TMDB/GetTmdbId/{imdb_id}", true);
        }
    }
}