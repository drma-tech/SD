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

                if (ratings.tmdb.Empty())
                {
                    ratings.tmdb = result?.result?.ratings?.TMDB?.audience?.rating.ToString(CultureInfo.InvariantCulture);
                    ratings.tmdbLink ??= result?.result?.links?.TMDB;
                }

                if (ratings.imdb.Empty())
                {
                    ratings.imdb = result?.result?.ratings?.IMDb?.audience?.rating.ToString(CultureInfo.InvariantCulture);
                    ratings.imdbLink ??= result?.result?.links?.IMDb;
                }

                if (ratings.metacritic.Empty())
                {
                    ratings.metacritic = result?.result?.ratings?.Metacritic?.audience?.rating.ToString(CultureInfo.InvariantCulture);
                    ratings.metacriticLink ??= result?.result?.links?.Metacritic;
                }

                if (ratings.rottenTomatoes.Empty())
                {
                    var rottenTomatoesRating = result?.result?.ratings?.RottenTomatoes?.audience?.rating;
                    if (rottenTomatoesRating.HasValue) rottenTomatoesRating /= 10;

                    ratings.rottenTomatoes = rottenTomatoesRating?.ToString(CultureInfo.InvariantCulture);
                    ratings.rottenTomatoesLink ??= result?.result?.links?.RottenTomatoes;
                }

                if (ratings.filmAffinity.Empty())
                {
                    ratings.filmAffinity = result?.result?.ratings?.FilmAffinity?.audience?.rating.ToString(CultureInfo.InvariantCulture);
                    ratings.filmAffinityLink ??= result?.result?.links?.FilmAffinity;
                }

                if (ratings.letterboxd.Empty())
                {
                    var letterboxdRating = result?.result?.ratings?.Letterboxd?.audience?.rating;
                    if (letterboxdRating.HasValue) letterboxdRating *= 2;

                    ratings.letterboxd = letterboxdRating?.ToString(CultureInfo.InvariantCulture);
                    ratings.letterboxdLink ??= result?.result?.links?.Letterboxd;
                }
            }
            catch (Exception ex)
            {
                req.LogError(ex);
            }
        }

        /// <summary>
        /// https://rapidapi.com/vannguyenv12/api/unified-movie-api
        /// 1109ms Latency
        /// 500 / Month (Hard Limit)
        /// </summary>
        /// <param name="ratings"></param>
        /// <param name="factory"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ProcessApiUnifiedMovie(this HttpRequestData req, IHttpClientFactory factory, Ratings ratings, CancellationToken cancellationToken)
        {
            try
            {
                var canProcess = false;
                if (ratings.tmdb.Empty()) canProcess = true;
                if (ratings.imdb.Empty()) canProcess = true;
                if (ratings.metacritic.Empty()) canProcess = true;
                if (ratings.rottenTomatoes.Empty()) canProcess = true;
                if (!canProcess) return;

                var client = factory.CreateClient("rapidapi");
                var result = await client.GetUnifiedMovie<Shared.Models.List.UnifiedMovie.Root>(ratings.tmdbId, cancellationToken);

                if (ratings.tmdb.Empty())
                {
                    var rating = result?.data?.ratings?.ratings?.FirstOrDefault(p => p.source == "tmdb")?.score;
                    if (rating.HasValue) rating /= 10;
                    ratings.tmdb = rating?.ToString(CultureInfo.InvariantCulture);
                }

                if (ratings.imdb.Empty())
                {
                    var rating = result?.data?.ratings?.ratings?.FirstOrDefault(p => p.source == "imdb")?.score;
                    if (rating.HasValue) rating /= 10;
                    ratings.imdb = rating?.ToString(CultureInfo.InvariantCulture);
                }

                if (ratings.metacritic.Empty())
                {
                    var rating = result?.data?.ratings?.ratings?.FirstOrDefault(p => p.source == "metacritic")?.score;
                    if (rating.HasValue) rating /= 10;
                    ratings.metacritic = rating?.ToString(CultureInfo.InvariantCulture);
                }

                if (ratings.rottenTomatoes.Empty())
                {
                    var rating = result?.data?.ratings?.ratings?.FirstOrDefault(p => p.source == "rottenTomatoes")?.score;
                    if (rating.HasValue) rating /= 10;
                    ratings.rottenTomatoes = rating?.ToString(CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                req.LogError(ex);
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

                if (ratings.imdb.Empty())
                {
                    ratings.imdb = result?.ratings?.imdb?.score?.ToString(CultureInfo.InvariantCulture);
                    ratings.imdbLink ??= result?.ratings?.imdb?.url;
                }

                if (ratings.metacritic.Empty())
                {
                    ratings.imdb = result?.ratings?.metacritic?.userScore?.ToString(CultureInfo.InvariantCulture);
                    ratings.imdbLink ??= result?.ratings?.metacritic?.url;
                }

                if (ratings.rottenTomatoes.Empty())
                {
                    var rottenTomatoesRating = result?.ratings?.rotten_tomatoes?.audienceScore;
                    if (rottenTomatoesRating.HasValue) rottenTomatoesRating /= 10;

                    ratings.rottenTomatoes = rottenTomatoesRating?.ToString(CultureInfo.InvariantCulture);
                    ratings.rottenTomatoesLink ??= result?.ratings?.rotten_tomatoes?.url;
                }

                if (ratings.letterboxd.Empty())
                {
                    var letterboxdRating = result?.ratings?.letterboxd?.score;
                    if (letterboxdRating.HasValue) letterboxdRating *= 2;

                    ratings.letterboxd = letterboxdRating?.ToString(CultureInfo.InvariantCulture);
                    ratings.letterboxdLink ??= result?.ratings?.letterboxd?.url;
                }
            }
            catch (Exception ex)
            {
                req.LogError(ex);
            }
        }
    }
}