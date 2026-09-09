using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Functions.Admin;
using SD.Shared.Core.Types;
using SD.Shared.Models.Auth;
using SD.Shared.Models.List.Tmdb;
using System.Globalization;

namespace SD.API.Functions.Public;

public class JobFunction(IHttpClientFactory factory, CosmosMainRepository repo)
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

    [Function("ProcessFollowingUpdates")]
    public async Task ProcessFollowingUpdates([HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "job/process-following-updates")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var docs = await repo.Query<WatchingList>(MainType.WatchingList,
            x => !x.SyncDate.IsDefined() || x.SyncDate == null || x.SyncDate < DateTime.UtcNow.AddDays(-14),
            x => x.Take(100),
            cancellationToken: cancellationToken);

        var client = factory.CreateClient("tmdb");
        var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", "en-US" },
            { "append_to_response", "videos" },
        };

        foreach (var doc in docs)
        {
            var newMovies = new List<string>();
            var newSeasons = new List<string>();

            foreach (var movie in doc.Movies)
            {
                var uri = TmdbOptions.BaseUri + "collection/" + movie.id!.ConfigureParameters(parameters);
                try
                {
                    var collection = await client.GetJsonFromApi<TmdbCollection>(uri, cancellationToken);

                    if (collection!.parts.Count > movie.maxItems) //there is new movie in the franchise, update it
                    {
                        movie.maxItems = collection.parts.Count;
                        newMovies.Add(collection.name!);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("could not be found", StringComparison.OrdinalIgnoreCase))
                    {
                        doc.Movies.Remove(movie);
                    }
                }
            }

            foreach (var show in doc.Shows)
            {
                var uri = TmdbOptions.BaseUri + "tv/" + show.id!.ConfigureParameters(parameters);
                var media = await client.GetJsonFromApi<TVDetail>(uri, cancellationToken);

                if (media!.seasons.Count > show.maxItems) //there is new season in the show, update it
                {
                    show.maxItems = media.seasons.Count;
                    newSeasons.Add(media.name!);
                }
            }

            if (doc.Movies.Empty() && doc.Shows.Empty()) //if empty, delete the document
            {
                await repo.DeleteItemAsync<WatchingList>(new MainIdentity(MainType.WatchingList, doc.Identity.RawId));
            }
            else
            {
                doc.SyncDate = DateTime.UtcNow;
                await repo.UpsertItemAsync(doc);

                if (newMovies.Count != 0 || newSeasons.Count != 0)
                {
                    var userId = doc.Identity.RawId;
                    var principal = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken) ?? throw new UnhandledException("Client null");

                    var zepto = new ZeptoMailClient(factory, ApiStartup.Configurations.ZeptoMail!.ApiKey!);
                    if (principal.Email.NotEmpty())
                    {
                        _ = zepto.SendFollowingTemplate(userId!, principal.Email, principal.DisplayName, franchises: newMovies.Count != 0 ? string.Join(", ", newMovies) : "No updates", series: newSeasons.Count != 0 ? string.Join(", ", newSeasons) : "No updates", cancellationToken);
                    }
                }
            }
        }
    }

    [Function("NotifyInactiveUsers")]
    public async Task NotifyInactiveUsers([HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "job/notify-inactive-users")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var docs = await repo.Query<AuthLogin>(MainType.Login,
            p => (!p.Notified.IsDefined() || !p.Notified) && (!p.Accesses.IsDefined() || !p.Accesses.Any() || p.Accesses.Max(a => a.Date) < DateTimeOffset.UtcNow.AddMonths(-3)),
            p => p.Take(100), cancellationToken);

        foreach (var doc in docs)
        {
            var userId = doc.Identity.RawId;
            var principal = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken) ?? throw new UnhandledException("Client null");

            if (principal.GetActiveSubscription() != null) continue; //ignore premium users

            doc.Notified = true;
            await repo.UpsertItemAsync(doc);

            var zepto = new ZeptoMailClient(factory, ApiStartup.Configurations.ZeptoMail!.ApiKey!);
            if (principal.Email.NotEmpty())
            {
                _ = zepto.SendInactiveTemplate(userId!, principal.Email, principal.DisplayName, cancellationToken);
            }
        }
    }

    [Function("DeleteInactiveUsers")]
    public async Task DeleteInactiveUsers([HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "job/delete-inactive-users")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var docs = await repo.Query<AuthLogin>(MainType.Login,
            p => !p.Accesses.IsDefined() || !p.Accesses.Any() || p.Accesses.Max(a => a.Date) < DateTimeOffset.UtcNow.AddMonths(-6),
            p => p.Take(100), cancellationToken);

        foreach (var doc in docs)
        {
            var userId = doc.Identity.RawId;
            var principal = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken) ?? throw new UnhandledException("Client null");

            if (principal.GetActiveSubscription() != null) continue; //ignore premium users
            if (!doc.Notified)
            {
                throw new UnhandledException($"User {userId} has not been notified before deletion.");
            }

            await Auth.PrincipalFunction.DeleteUser(repo, userId);
        }
    }
}