using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Suggestions.Resources;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class TmdbApi(IHttpClientFactory factory) : ApiExternal(factory)
    {
        public async Task<MediaDetail> GetMediaDetail(string? tmdb_id, MediaType? type)
        {
            ArgumentNullException.ThrowIfNull(tmdb_id);
            ArgumentNullException.ThrowIfNull(type);

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
                { "append_to_response", "videos" }
            };

            var obj_return = new MediaDetail();

            if (type == MediaType.movie)
            {
                var item = await GetAsync<MovieDetail>(TmdbOptions.BaseUri + "movie/" + tmdb_id.ConfigureParameters(parameter));

                if (item != null)
                {
                    obj_return = new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.title,
                        original_title = item.original_title,
                        plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_average,
                        runtime = item.runtime,
                        homepage = item.homepage,
                        Videos = item.videos?.results.Select(s => new Video { id = s.id, key = s.key, name = s.name }).ToList() ?? [],
                        Genres = item.genres.Select(s => s.name ?? "").ToList(),
                        MediaType = MediaType.movie
                    };

                    if (item.belongs_to_collection != null)
                    {
                        var collection = await GetCollection(item.belongs_to_collection.id.ToString(), parameter);// await GetAsync<TmdbCollection>(TmdbOptions.BaseUri + "collection/" + item.belongs_to_collection.id.ToString().ConfigureParameters(parameter), true);

                        if (collection != null)
                        {
                            obj_return.collectionId = collection.id;
                            obj_return.collectionName = collection.name;
                            obj_return.collectionLogo = collection.poster_path;

                            foreach (var part in collection.parts ?? [])
                            {
                                obj_return.Collection.Add(ConvertToCollection(part));
                            }
                        }
                    }
                }
            }
            else
            {
                var item = await GetAsync<TVDetail>(TmdbOptions.BaseUri + "tv/" + tmdb_id.ConfigureParameters(parameter));

                if (item != null)
                {
                    obj_return = new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.name,
                        original_title = item.original_name,
                        plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
                        release_date = item.first_air_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_average,
                        runtime = item.episode_run_time.FirstOrDefault(),
                        homepage = item.homepage,
                        Videos = item.videos?.results.Select(s => new Video { id = s.id, key = s.key, name = s.name }).ToList() ?? [],
                        Genres = item.genres.Select(s => s.name ?? "").ToList(),
                        MediaType = MediaType.tv
                    };

                    foreach (var season in item.seasons)
                    {
                        obj_return.Collection.Add(ConvertToCollection(season));
                    }
                }
            }

            return obj_return;
        }

        public static Collection ConvertToCollection(Part part)
        {
            return new Collection
            {
                id = part.id.ToString(),
                title = part.title,
                release_date = part.release_date.GetDate(),
                poster_small = string.IsNullOrEmpty(part.poster_path) ? null : TmdbOptions.SmallPosterPath + part.poster_path
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
                poster_small = string.IsNullOrEmpty(season.poster_path) ? null : TmdbOptions.SmallPosterPath + season.poster_path
            };
        }

        public async Task<TmdbCollection?> GetCollection(string? collectionId, Dictionary<string, string> parameters)
        {
            if (collectionId == null) return default;

            return await GetAsync<TmdbCollection>(TmdbOptions.BaseUri + "collection/" + collectionId.ConfigureParameters(parameters));
        }

        public async Task<TmdbSeason?> GetSeason(string? tmdbId, int? seasonNumber, Dictionary<string, string> parameters)
        {
            if (tmdbId == null) return default;
            if (seasonNumber == null) return default;

            return await GetAsync<TmdbSeason>(TmdbOptions.BaseUri + $"tv/{tmdbId}/season/{seasonNumber}".ConfigureParameters(parameters));
        }

        public async Task<MediaProviders?> GetWatchProvidersList(string? tmdb_id, MediaType? type)
        {
            ArgumentNullException.ThrowIfNull(tmdb_id);
            ArgumentNullException.ThrowIfNull(type);

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey }
            };

            if (type == MediaType.movie)
            {
                return await GetAsync<MediaProviders>(TmdbOptions.BaseUri + $"movie/{tmdb_id}/watch/providers".ConfigureParameters(parameter));
            }
            else //tv
            {
                return await GetAsync<MediaProviders>(TmdbOptions.BaseUri + $"tv/{tmdb_id}/watch/providers".ConfigureParameters(parameter));
            }
        }
    }

    internal sealed class Ordem
    {
        public MediaType type { get; set; }
        public int id { get; set; }
        public double Popularity { get; set; }
    }
}