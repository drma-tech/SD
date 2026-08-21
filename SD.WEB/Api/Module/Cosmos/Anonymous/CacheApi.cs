using SD.Shared.Models.Franchise;
using SD.Shared.Models.List;
using SD.Shared.Models.List.Imdb;
using SD.Shared.Models.News;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Trailers;
using SD.WEB.Api.Core;
using System.Globalization;

namespace SD.WEB.Api.Module.Cosmos.Anonymous;

public class ImdbPopularApi(IHttpClientFactory factory) : ApiCosmos<MostPopularDataCache>(factory, ApiType.Anonymous, key: null, [], ApiContext.Default.MostPopularDataCache), IMediaListApi
{
    public async Task<(ISet<MediaDetail> list, bool lastPage)> GetList(ISet<MediaDetail> currentList, RenderControlState<ISet<MediaDetail>>[] states,
        MediaType? type = null, IDictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1, CancellationToken cancellationToken = default)
    {
        if (currentList.Empty())
        {
            foreach (var state in states)
            {
                await state.StartLoading(null);
            }
        }

        if (type == MediaType.movie)
        {
            var result = await GetAsync("public/cache/imdb-popular-movies".ConfigureParameters(stringParameters), setNewVersion: false, states: [], cancellationToken);

            foreach (var item in result?.Data?.Items ?? [])
            {
                currentList.Add(BuildMediaDetail(item, MediaType.movie));
            }
        }
        else if (type == MediaType.tv)
        {
            var result = await GetAsync("public/cache/imdb-popular-tv".ConfigureParameters(stringParameters), setNewVersion: false, states: [], cancellationToken);

            foreach (var item in result?.Data?.Items ?? [])
            {
                //TODO: tv api has wrong poster definitions
                //var shortImage = !string.IsNullOrEmpty(item.Image) && item.Image.Contains("_V1_")
                //    ? item.Image?.Remove(item.Image.IndexOf("_V1_", StringComparison.Ordinal)) + "_V1_QL75_UY207_CR13,0,140,207_.jpg"
                //    : item.Image;

                currentList.Add(BuildMediaDetail(item, MediaType.tv));
            }
        }

        foreach (var state in states)
        {
            await state.FinishLoading(currentList);
        }

        return (currentList, true);
    }

    private static MediaDetail BuildMediaDetail(MostPopularDataDetail item, MediaType type)
    {
        return new MediaDetail
        {
            tmdb_id = item.Id,
            title = item.Title,
            //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
            release_date = string.IsNullOrEmpty(item.Year)
                          ? DateTime.MaxValue
                          : new DateTime(int.Parse(item.Year, CultureInfo.InvariantCulture), 1, 1, 0, 0, 0, DateTimeKind.Local),
            poster_small = item.Image,
            rating = string.IsNullOrEmpty(item.IMDbRating)
                          ? 0
                          : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
            MediaType = type,
        };
    }
}

public class FranchiseApi(IHttpClientFactory http) : ApiCosmos<FranchiseCache>(http, ApiType.Anonymous, key: null, [], ApiContext.Default.FranchiseCache)
{
    public async Task<FranchiseCache?> GetItems(RenderControlState<FranchiseCache> state, CancellationToken cancellationToken)
    {
        return await GetAsync("public/cache/franchise", setNewVersion: false, [state], cancellationToken);
    }
}

public class CacheFlixsterApi(IHttpClientFactory http) : ApiCosmos<NewsCache>(http, ApiType.Anonymous, key: null, [], ApiContext.Default.NewsCache)
{
    public async Task<NewsCache?> GetNews(string mode, string category, RenderControlState<NewsCache> state, CancellationToken cancellationToken)
    {
        return await GetAsync($"public/cache/news?mode={mode}&category={category}", setNewVersion: false, [state], cancellationToken);
    }
}

public class CacheYoutubeApi(IHttpClientFactory http) : ApiCosmos<YoutubeCache>(http, ApiType.Anonymous, key: null, [], ApiContext.Default.YoutubeCache)
{
    public async Task<YoutubeCache?> GetTrailers(string mode, RenderControlState<YoutubeCache> state, CancellationToken cancellationToken)
    {
        return await GetAsync($"public/cache/trailers?mode={mode}", setNewVersion: false, [state], cancellationToken);
    }
}

public class CacheRatingsApi(IHttpClientFactory http) : ApiCosmos<RatingsCache>(http, ApiType.Anonymous, key: null, [], ApiContext.Default.RatingsCache)
{
    public async Task<RatingsCache?> GetMovieRatings(string? id, string? tmdbId, string? title, DateTime? releaseDate, string? tmdbRating, RenderControlState<RatingsCache> state, CancellationToken cancellationToken)
    {
        return await GetAsync($"public/cache/ratings/movie?id={id}&tmdb_id={tmdbId}&title={title}&release_date={releaseDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&tmdb_rating={tmdbRating}", setNewVersion: false, [state], cancellationToken);
    }

    public async Task<RatingsCache?> GetShowRatings(string? id, string? tmdbId, string? title, DateTime? releaseDate, string? tmdbRating, RenderControlState<RatingsCache> state, CancellationToken cancellationToken)
    {
        return await GetAsync($"public/cache/ratings/show?id={id}&tmdb_id={tmdbId}&title={title}&release_date={releaseDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&tmdb_rating={tmdbRating}", setNewVersion: false, [state], cancellationToken);
    }
}

public class CacheMetaCriticApi(IHttpClientFactory http) : ApiCosmos<MetaCriticCache>(http, ApiType.Anonymous, key: null, [], typeInfo: ApiContext.Default.MetaCriticCache)
{
    public async Task<MetaCriticCache?> GetMovieReviews(string? id, string? title, DateTime? releaseDate, RenderControlState<MetaCriticCache> state, CancellationToken cancellationToken)
    {
        return await GetAsync($"public/cache/reviews/movies?id={id}&title={title}&release_date={releaseDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}", setNewVersion: false, [state], cancellationToken);
    }
}