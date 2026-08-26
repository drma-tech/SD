using Microsoft.Azure.Functions.Worker.Http;
using SD.Shared.Models.List;
using System.Globalization;

namespace SD.API.Core.Scraping
{
    public static class NewScrapingRatings
    {
        /// <summary>
        /// https://rapidapi.com/MostPopular/api/film-show-ratings
        /// 335ms Latency
        /// 100 / Day (Hard Limit)
        /// </summary>
        /// <param name="ratings"></param>
        /// <param name="factory"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ProcessApiFilmShowRatings(this HttpRequestData req, IHttpClientFactory factory, Ratings ratings, CancellationToken cancellationToken)
        {
            try
            {
                var canProcess = false;
                if (ratings.tmdb.Empty()) canProcess = true;
                if (ratings.imdb.Empty()) canProcess = true;
                if (ratings.metacritic.Empty()) canProcess = true;
                if (ratings.rottenTomatoes.Empty()) canProcess = true;
                if (ratings.filmAffinity.Empty()) canProcess = true;
                if (ratings.letterboxd.Empty()) canProcess = true;
                if (!canProcess) return;

                var client = factory.CreateClient("rapidapi-gzip");
                var result = await client.GetFilmShowRatings<RatingApiRoot>(ratings.imdbId, cancellationToken);

                ratings.tmdb ??= result?.result?.ratings?.TMDB?.audience?.rating.ToString(CultureInfo.InvariantCulture);
                ratings.tmdbLink ??= result?.result?.links?.TMDB;

                ratings.imdb ??= result?.result?.ratings?.IMDb?.audience?.rating.ToString(CultureInfo.InvariantCulture);
                ratings.imdbLink ??= result?.result?.links?.IMDb;

                ratings.metacritic ??= result?.result?.ratings?.Metacritic?.audience?.rating.ToString(CultureInfo.InvariantCulture);
                ratings.metacriticLink ??= result?.result?.links?.Metacritic;

                var rottenTomatoesRating = result?.result?.ratings?.RottenTomatoes?.audience?.rating;
                if (rottenTomatoesRating.HasValue) rottenTomatoesRating /= 10;

                ratings.rottenTomatoes ??= rottenTomatoesRating?.ToString(CultureInfo.InvariantCulture);
                ratings.rottenTomatoesLink ??= result?.result?.links?.RottenTomatoes;

                ratings.filmAffinity ??= result?.result?.ratings?.FilmAffinity?.audience?.rating.ToString(CultureInfo.InvariantCulture);
                ratings.filmAffinityLink ??= result?.result?.links?.FilmAffinity;

                var letterboxdRating = result?.result?.ratings?.Letterboxd?.audience?.rating;
                if (letterboxdRating.HasValue) letterboxdRating *= 2;

                ratings.letterboxd ??= letterboxdRating?.ToString(CultureInfo.InvariantCulture);
                ratings.letterboxdLink ??= result?.result?.links?.Letterboxd;
            }
            catch (OperationCanceledException)
            {
                //do nothing
            }
            catch (ObjectDisposedException)
            {
                //do nothing
            }
            catch (Exception ex)
            {
                if (string.Equals(ex.Message, "Not Found", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (string.Equals(ex.Message, "Bad Gateway", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                req.LogError(ex);

                if (ex.Message.Contains("Too Many Requests", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ApiRateLimitException(ex.Message);
                }
            }
        }

        /// <summary>
        /// https://rapidapi.com/pierregoutheraud/api/movies-ratings2
        /// 2439ms Latency
        /// 10 / Day (Hard Limit)
        /// </summary>
        /// <param name="ratings"></param>
        /// <param name="factory"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ProcessApiMoviesRatings2(this HttpRequestData req, IHttpClientFactory factory, Ratings ratings, CancellationToken cancellationToken)
        {
            try
            {
                var canProcess = false;
                if (ratings.imdb.Empty()) canProcess = true;
                if (ratings.metacritic.Empty()) canProcess = true;
                if (ratings.rottenTomatoes.Empty()) canProcess = true;
                if (ratings.letterboxd.Empty()) canProcess = true;
                if (!canProcess) return;

                var client = factory.CreateClient("rapidapi");
                var result = await client.GetMoviesRatings2<Shared.Models.List.MoviesRatings2.Root>(ratings.imdbId, cancellationToken);

                ratings.imdb ??= result?.ratings?.imdb?.score?.ToString(CultureInfo.InvariantCulture);
                ratings.imdbLink ??= result?.ratings?.imdb?.url;

                ratings.imdb ??= result?.ratings?.metacritic?.userScore?.ToString(CultureInfo.InvariantCulture);
                ratings.imdbLink ??= result?.ratings?.metacritic?.url;

                var rottenTomatoesRating = result?.ratings?.rotten_tomatoes?.audienceScore;
                if (rottenTomatoesRating.HasValue) rottenTomatoesRating /= 10;

                ratings.rottenTomatoes ??= rottenTomatoesRating?.ToString(CultureInfo.InvariantCulture);
                ratings.rottenTomatoesLink ??= result?.ratings?.rotten_tomatoes?.url;

                var letterboxdRating = result?.ratings?.letterboxd?.score;
                if (letterboxdRating.HasValue) letterboxdRating *= 2;

                ratings.letterboxd ??= letterboxdRating?.ToString(CultureInfo.InvariantCulture);
                ratings.letterboxdLink ??= result?.ratings?.letterboxd?.url;
            }
            catch (OperationCanceledException)
            {
                //do nothing
            }
            catch (ObjectDisposedException)
            {
                //do nothing
            }
            catch (Exception ex)
            {
                if (string.Equals(ex.Message, "Not Found", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (string.Equals(ex.Message, "Bad Gateway", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                req.LogError(ex);

                if (string.Equals(ex.Message, "Too Many Requests", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ApiRateLimitException(ex.Message);
                }
            }
        }
    }
}