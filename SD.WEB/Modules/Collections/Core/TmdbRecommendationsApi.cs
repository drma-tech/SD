using SD.Shared.Models.List.Tmdb;

namespace SD.WEB.Modules.Collections.Core;

public class TmdbRecommendationsApi(IHttpClientFactory factory) : ApiExternal(factory)
{
    public async Task<HashSet<MediaDetail>> GetList(MediaType? type, string? tmdbId, ComponentActions<HashSet<MediaDetail>>? actions, CancellationToken cancellationToken)
    {
        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", (await AppStateStatic.GetContentLanguage(cancellationToken: cancellationToken)).GetName(false) ?? "en-US" }
        };

        if (actions != null) await actions.StartLoading.Invoke(null);

        if (type == MediaType.movie)
        {
            var result = await GetAsync<MoviePopular>(TmdbOptions.BaseUri + $"movie/{tmdbId}/recommendations".ConfigureParameters(parameter), false, null, cancellationToken);

            var currentList = new HashSet<MediaDetail>();

            foreach (var item in result?.results ?? [])
            {
                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = item.title,
                    plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                    release_date = item.release_date?.GetDate(),
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_count > 10 ? item.vote_average : 0,
                    MediaType = MediaType.movie
                });
            }

            if (actions != null) await actions.FinishLoading.Invoke(currentList);

            return currentList;
        }
        else //if (type == MediaType.tv)
        {
            var result = await GetAsync<TVPopular>(TmdbOptions.BaseUri + $"tv/{tmdbId}/recommendations".ConfigureParameters(parameter), false, null, cancellationToken);

            var currentList = new HashSet<MediaDetail>();

            foreach (var item in result?.results ?? [])
            {
                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = item.name,
                    plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                    release_date = item.first_air_date?.GetDate(),
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_count > 10 ? item.vote_average : 0,
                    MediaType = MediaType.tv
                });
            }

            if (actions != null) await actions.FinishLoading.Invoke(currentList);

            return currentList;
        }
    }
}