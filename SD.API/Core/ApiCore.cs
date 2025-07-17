using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace SD.API.Core;

public static class ApiCore
{
    public static async Task<T?> Get<T>(this HttpClient http, string requestUri, CancellationToken cancellationToken)
        where T : class
    {
        var response = await http.GetAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode) throw new UnhandledException(response.ReasonPhrase);

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    public static async Task AddTmdbListItem(this HttpClient http, int listId, int tmdbId, MediaType type, string? token, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.tmdb.org/4/list/{listId}/items");

        request.Headers.Add("Accept", "application/json");
        request.Headers.TryAddWithoutValidation("content-type", "application/json");
        request.Headers.Add("Authorization", $"Bearer {token}");
        var obj = new { items = new[] { new { media_type = type == MediaType.movie ? "movie" : "tv", media_id = tmdbId } } };
        request.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

        var response = await http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new UnhandledException(responseContent);
        }
    }

    public static async Task<T?> GetTrailersByYoutubeSearch<T>(this HttpClient http, CancellationToken cancellationToken) where T : class
    {
        const string id = "UCzcRQ3vRNr6fJ1A9rqFn7QA";
        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://youtube-search-and-download.p.rapidapi.com/channel?id={id}&sort=n");

        request.Headers.TryAddWithoutValidation("X-RapidAPI-Key", ApiStartup.Configurations.RapidAPI?.Key);
        request.Headers.TryAddWithoutValidation("X-RapidAPI-Host", "youtube-search-and-download.p.rapidapi.com");

        var response = await http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.TooManyRequests) return null;

            throw new UnhandledException(response.ReasonPhrase);
        }

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    public static async Task<T?> GetReviewsByImdb8<T>(this HttpClient http, string? tconst, CancellationToken cancellationToken) where T : class
    {
        if (string.IsNullOrEmpty(tconst)) return null;

        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://imdb8.p.rapidapi.com/title/v2/get-metacritic?tconst={tconst}");

        request.Headers.TryAddWithoutValidation("X-RapidAPI-Key", ApiStartup.Configurations.RapidAPI?.Key);
        request.Headers.TryAddWithoutValidation("X-RapidAPI-Host", "imdb8.p.rapidapi.com");

        var response = await http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode) throw new UnhandledException(response.ReasonPhrase);

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    public static async Task<T?> GetFilmShowRatings<T>(this HttpClient http, string? imdbId, CancellationToken cancellationToken) where T : class
    {
        if (string.IsNullOrEmpty(imdbId)) return null;

        using var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri($"https://film-show-ratings.p.rapidapi.com/item/?id={imdbId}");
        request.Headers.Add("x-rapidapi-key", ApiStartup.Configurations.RapidAPI?.Key);
        request.Headers.Add("x-rapidapi-host", "film-show-ratings.p.rapidapi.com");

        var response = await http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode) throw new UnhandledException(response.ReasonPhrase);

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }
}