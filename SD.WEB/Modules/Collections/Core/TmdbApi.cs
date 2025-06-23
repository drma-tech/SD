using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Collections.Resources;

namespace SD.WEB.Modules.Collections.Core;

public class TmdbApi(IHttpClientFactory factory) : ApiExternal(factory)
{
    public async Task<MediaDetail> GetMediaDetail(string? tmdbId, MediaType type, string? language = null)
    {
        ArgumentNullException.ThrowIfNull(tmdbId);

        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", language ?? AppStateStatic.Language.GetName(false) ?? "en-US" },
            { "append_to_response", "videos" }
        };

        var objReturn = new MediaDetail();

        if (type == MediaType.movie)
        {
            var item = await GetAsync<MovieDetail>(TmdbOptions.BaseUri + "movie/" +
                                                   tmdbId.ConfigureParameters(parameter));

            if (item != null)
            {
                objReturn = new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = item.title,
                    original_title = item.original_title,
                    original_language = item.original_language,
                    plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
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
                    Videos = item.videos?.results.Select(s => new Video { id = s.id, key = s.key, name = s.name })
                        .ToList() ?? [],
                    Genres = item.genres.Select(s => s.name ?? "").ToList(),
                    MediaType = MediaType.movie
                };

                if (item.belongs_to_collection != null)
                {
                    var collection =
                        await GetCollection(item.belongs_to_collection.id.ToString(),
                            parameter); // await GetAsync<TmdbCollection>(TmdbOptions.BaseUri + "collection/" + item.belongs_to_collection.id.ToString().ConfigureParameters(parameter), true);

                    if (collection != null)
                    {
                        objReturn.collectionId = collection.id;
                        objReturn.collectionName = collection.name;
                        objReturn.collectionLogo = collection.poster_path;

                        foreach (var part in collection.parts)
                            objReturn.Collection.Add(ConvertToCollection(part));
                    }
                }
            }
        }
        else
        {
            var item = await GetAsync<TVDetail>(TmdbOptions.BaseUri + "tv/" + tmdbId.ConfigureParameters(parameter));

            if (item != null)
            {
                objReturn = new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = item.name,
                    original_title = item.original_name,
                    original_language = item.original_language,
                    plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
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
                    Videos = item.videos?.results.Select(s => new Video { id = s.id, key = s.key, name = s.name })
                        .ToList() ?? [],
                    Genres = item.genres.Select(s => s.name ?? "").ToList(),
                    MediaType = MediaType.tv
                };

                foreach (var season in item.seasons) objReturn.Collection.Add(ConvertToCollection(season));
            }
        }

        return objReturn;
    }

    public static Collection ConvertToCollection(Part part)
    {
        return new Collection
        {
            id = part.id.ToString(),
            title = part.title,
            release_date = part.release_date.GetDate(),
            poster_small = string.IsNullOrEmpty(part.poster_path)
                ? null
                : TmdbOptions.SmallPosterPath + part.poster_path
        };
    }

    public static Collection ConvertToCollection(Season season)
    {
        return new Collection
        {
            id = season.id.ToString(),
            title = season.name,
            SeasonNumber = season.season_number,
            release_date = season.air_date?.GetDate(),
            poster_small = string.IsNullOrEmpty(season.poster_path)
                ? null
                : TmdbOptions.SmallPosterPath + season.poster_path
        };
    }

    public async Task<TmdbCollection?> GetCollection(string? collectionId, Dictionary<string, string> parameters)
    {
        if (collectionId == null) return null;

        return await GetAsync<TmdbCollection>(TmdbOptions.BaseUri + "collection/" +
                                              collectionId.ConfigureParameters(parameters));
    }

    public async Task<TmdbSeason?> GetSeason(string? tmdbId, int? seasonNumber, Dictionary<string, string> parameters)
    {
        if (tmdbId == null) return null;
        if (seasonNumber == null) return null;

        return await GetAsync<TmdbSeason>(TmdbOptions.BaseUri +
                                          $"tv/{tmdbId}/season/{seasonNumber}".ConfigureParameters(parameters));
    }

    public async Task<MediaProviders?> GetWatchProvidersList(string? tmdbId, MediaType? type)
    {
        ArgumentNullException.ThrowIfNull(tmdbId);
        ArgumentNullException.ThrowIfNull(type);

        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey }
        };

        if (type == MediaType.movie)
            return await GetAsync<MediaProviders>(TmdbOptions.BaseUri +
                                                  $"movie/{tmdbId}/watch/providers".ConfigureParameters(parameter));

        //tv
        return await GetAsync<MediaProviders>(TmdbOptions.BaseUri +
                                              $"tv/{tmdbId}/watch/providers".ConfigureParameters(parameter));
    }
}

internal sealed class Ordem
{
    public MediaType Type { get; set; }
    public int Id { get; set; }
    public double Popularity { get; set; }
}