using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Suggestions.Interface;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class TmdbSearchApi : ApiServices, IMediaListApi
    {
        public TmdbSearchApi(IHttpClientFactory http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
                { "page", page.ToString() },
                { "include_adult", "false" }
            };

            if (stringParameters != null)
            {
                foreach (var item in stringParameters)
                {
                    parameter.TryAdd(item.Key, item.Value);
                }
            }

            var result = await GetByRequest<TmdbSearch>(TmdbOptions.BaseUri + $"search/multi".ConfigureParameters(parameter));

            if (result != null)
            {
                foreach (var item in result.results.OrderByDescending(o => o.popularity))
                {
                    var mediaType = (MediaType)Enum.Parse(typeof(MediaType), item.media_type ?? "");

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = mediaType == MediaType.movie ? item.title : item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = mediaType == MediaType.tv ? item.first_air_date?.GetDate() : item.release_date?.GetDate(),
                        poster_small = GetPoster(item, mediaType),
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average : 0,
                        MediaType = mediaType,
                        comments = GetComments(item, mediaType)
                    });
                }
            }

            return new(currentList, page >= result?.total_pages);
        }

        private string? GetPoster(TmdbResult? item, MediaType type)
        {
            if (item == null) return null;

            if (type == MediaType.person)
            {
                return string.IsNullOrEmpty(item.profile_path) ? null : TmdbOptions.SmallPosterPath + item.profile_path;
            }
            else
            {
                return string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path;
            }
        }

        private string? GetComments(TmdbResult? item, MediaType type)
        {
            if (item == null) return null;

            switch (type)
            {
                case MediaType.movie:
                    return MediaType.movie.GetName();
                case MediaType.tv:
                    return MediaType.tv.GetName();
                case MediaType.person:
                    return $"{MediaType.person.GetName()},{item.known_for_department}";
                default:
                    return null;
            }
        }
    }
}