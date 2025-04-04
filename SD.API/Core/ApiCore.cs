﻿using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace SD.API.Core
{
    public static class ApiCore
    {
        public static async Task<T?> Get<T>(this HttpClient http, string request_uri, CancellationToken cancellationToken) where T : class
        {
            var response = await http.GetAsync(request_uri, cancellationToken);

            if (!response.IsSuccessStatusCode) throw new NotificationException(response.ReasonPhrase);

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
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
                throw new NotificationException(responseContent);
            }
        }

        public static async Task<T?> GetTrailersByYoutubeSearch<T>(this HttpClient http, CancellationToken cancellationToken) where T : class
        {
            string id = "UCzcRQ3vRNr6fJ1A9rqFn7QA";
            using var request = new HttpRequestMessage(HttpMethod.Get, $"https://youtube-search-and-download.p.rapidapi.com/channel?id={id}&sort=n");

            request.Headers.TryAddWithoutValidation("X-RapidAPI-Key", "36af8735e3msh39423dcd3a94067p1975bdjsn4536c4c2ed8a");
            request.Headers.TryAddWithoutValidation("X-RapidAPI-Host", "youtube-search-and-download.p.rapidapi.com");

            var response = await http.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    return null;
                }
                else
                {
                    throw new NotificationException(response.ReasonPhrase);
                }
            }

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }

        public static async Task<T?> GetReviewsByImdb8<T>(this HttpClient http, string? tconst, CancellationToken cancellationToken) where T : class
        {
            if (string.IsNullOrEmpty(tconst)) return null;

            using var request = new HttpRequestMessage(HttpMethod.Get, $"https://imdb8.p.rapidapi.com/title/v2/get-metacritic?tconst={tconst}");

            request.Headers.TryAddWithoutValidation("X-RapidAPI-Key", "36af8735e3msh39423dcd3a94067p1975bdjsn4536c4c2ed8a");
            request.Headers.TryAddWithoutValidation("X-RapidAPI-Host", "imdb8.p.rapidapi.com");

            var response = await http.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) throw new NotificationException(response.ReasonPhrase);

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }

        public static async Task<T?> GetFilmShowRatings<T>(this HttpClient http, string? imdbId, CancellationToken cancellationToken) where T : class
        {
            if (string.IsNullOrEmpty(imdbId)) return null;

            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://film-show-ratings.p.rapidapi.com/item/?id={imdbId}"),
                Headers = { { "x-rapidapi-key", "36af8735e3msh39423dcd3a94067p1975bdjsn4536c4c2ed8a" }, { "x-rapidapi-host", "film-show-ratings.p.rapidapi.com" }, },
            };

            var response = await http.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) throw new NotificationException(response.ReasonPhrase);

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
    }
}