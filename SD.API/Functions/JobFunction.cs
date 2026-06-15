using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.Shared.Models.List.Tmdb;
using System.Globalization;

namespace SD.API.Functions;

public class JobFunction(CosmosJobRepository repo, IHttpClientFactory factory)
{
    [Function("ClearExpectedMovies")]
    public async Task ClearExpectedMovies([HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "job/clear-expected-movies")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var cacheKey = $"{TmdbOptions.BaseUriNew}list/{(int)EnumLists.ExpectedMovieOf2026}";
        var tmdbWriteToken = ApiStartup.Configurations.TMDB?.WriteToken;
        var client = factory.CreateClient("tmdb");

        var result = await client.GetdTmdbList<CustomListNew>(cacheKey, tmdbWriteToken, cancellationToken);

        foreach (var item in result?.results ?? [])
        {
            var date = item.release_date.NotEmpty() ? DateTime.ParseExact(item.release_date, "yyyy-MM-dd", CultureInfo.CurrentCulture) : (DateTime?)null;

            if (date < DateTime.UtcNow.AddDays(-14)) //delete items that are released for more than 2 weeks
            {
                await client.RemoveTmdbListItem((int)EnumLists.ExpectedMovieOf2026, item.id, Enum.Parse<MediaType>(item.media_type!), tmdbWriteToken, cancellationToken);
            }
        }
    }
}