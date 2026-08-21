using SD.Shared.Models.List.Tmdb;
using SD.WEB.Api.Core;
using System.Globalization;

namespace SD.WEB.Api.Module.External;

public class TmdbApi(IHttpClientFactory factory) : ApiExternal(factory)
{
    public async Task<MediaDetail> GetMediaDetail(string? tmdbId, MediaType type, string language, RenderControlState<MediaDetail>? state, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tmdbId);

        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", language },
            { "append_to_response", "videos" },
        };

        if (state != null) await state.StartLoading.Invoke(null);

        var objReturn = new MediaDetail();

        if (type == MediaType.movie)
        {
            var item = await GetAsync<MovieDetail>(TmdbOptions.BaseUri + "movie/" + tmdbId.ConfigureParameters(parameter), setNewVersion: false, state: null, cancellationToken);

            if (item != null)
            {
                objReturn = new MediaDetail
                {
                    tmdb_id = item.id.ToString(CultureInfo.InvariantCulture),
                    title = item.title,
                    original_title = item.original_title,
                    original_language = item.original_language,
                    plot = string.IsNullOrEmpty(item.overview) ? Translations.Module.Media.NoPlot : item.overview,
                    release_date = item.release_date?.GetDate(),
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_average,
                    runtime = item.runtime,
                    homepage = item.homepage,
                    Videos = item.videos?.results.Select(s => new Video { id = s.id, key = s.key, name = s.name, type = s.type }).ToList() ?? [],
                    Genres = [.. item.genres.Select(s => s.name ?? "")],
                    MediaType = MediaType.movie,
                };

                if (item.belongs_to_collection != null)
                {
                    var collection = await GetCollection(item.belongs_to_collection.id.ToString(CultureInfo.InvariantCulture), parameter, cancellationToken);

                    if (collection != null)
                    {
                        objReturn.collectionId = collection.id;
                        objReturn.collectionName = collection.name;
                        objReturn.collectionLogo = collection.poster_path;

                        foreach (var part in collection.parts)
                            objReturn.Collection.Add(part.ConvertToCollection());
                    }
                }
            }
        }
        else
        {
            var item = await GetAsync<TVDetail>(TmdbOptions.BaseUri + "tv/" + tmdbId.ConfigureParameters(parameter), setNewVersion: false, state: null, cancellationToken);

            if (item != null)
            {
                objReturn = new MediaDetail
                {
                    tmdb_id = item.id.ToString(CultureInfo.InvariantCulture),
                    title = item.name,
                    original_title = item.original_name,
                    original_language = item.original_language,
                    plot = string.IsNullOrEmpty(item.overview) ? Translations.Module.Media.NoPlot : item.overview,
                    release_date = item.first_air_date?.GetDate(),
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_average,
                    runtime = item.episode_run_time.FirstOrDefault(),
                    homepage = item.homepage,
                    Videos = item.videos?.results.Select(s => new Video { id = s.id, key = s.key, name = s.name, type = s.type }).ToList() ?? [],
                    Genres = [.. item.genres.Select(s => s.name ?? "")],
                    MediaType = MediaType.tv,
                };

                foreach (var season in item.seasons) objReturn.Collection.Add(season.ConvertToCollection());
            }
        }

        if (state != null) await state.FinishLoading.Invoke(objReturn);
        return objReturn;
    }

    public async Task<TmdbCollection?> GetCollection(string? collectionId, IDictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        if (collectionId == null) return null;

        return await GetAsync<TmdbCollection>(TmdbOptions.BaseUri + "collection/" + collectionId.ConfigureParameters(parameters), setNewVersion: false, state: null, cancellationToken);
    }

    public async Task<MediaProviders?> GetWatchProvidersList(string? tmdbId, MediaType? type, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tmdbId);
        if (!type.HasValue)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
        };

        if (type == MediaType.movie)
            return await GetAsync<MediaProviders>(TmdbOptions.BaseUri + $"movie/{tmdbId}/watch/providers".ConfigureParameters(parameter), setNewVersion: false, state: null, cancellationToken);

        //tv
        return await GetAsync<MediaProviders>(TmdbOptions.BaseUri + $"tv/{tmdbId}/watch/providers".ConfigureParameters(parameter), setNewVersion: false, state: null, cancellationToken);
    }

    public async Task<TmdbSeason?> GetSeason(string? tmdbId, int? seasonNumber, IDictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        if (tmdbId == null) return null;
        if (seasonNumber == null) return null;

        return await GetAsync<TmdbSeason>(TmdbOptions.BaseUri + string.Create(CultureInfo.InvariantCulture, $"tv/{tmdbId}/season/{seasonNumber}").ConfigureParameters(parameters), setNewVersion: false, state: null, cancellationToken);
    }
}

public static class TmdbApiHelper
{
    public static Collection ConvertToCollection(this Part part)
    {
        return new Collection
        {
            id = part.id.ToString(CultureInfo.InvariantCulture),
            title = part.title,
            release_date = part.release_date.GetDate(),
            poster_small = string.IsNullOrEmpty(part.poster_path)
                ? null
                : TmdbOptions.SmallPosterPath + part.poster_path,
        };
    }

    public static Collection ConvertToCollection(this Season season)
    {
        return new Collection
        {
            id = season.id.ToString(CultureInfo.InvariantCulture),
            title = season.name,
            SeasonNumber = season.season_number,
            release_date = season.air_date?.GetDate(),
            poster_small = string.IsNullOrEmpty(season.poster_path)
                ? null
                : TmdbOptions.SmallPosterPath + season.poster_path,
        };
    }
}

internal sealed class Order
{
    public MediaType Type { get; set; }
    public int Id { get; set; }
    public double Popularity { get; set; }
}