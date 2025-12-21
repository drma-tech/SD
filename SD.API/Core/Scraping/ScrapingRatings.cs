using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using SD.Shared.Models.List;
using System.Globalization;

namespace SD.API.Core.Scraping;

public class ScrapingRatings(ILogger logger, RatingApiRoot? ratingApiRoot)
{
    private const string ImdbRatingUrl = "https://www.imdb.com/title/{0}/ratings";
    private const string ImdbUrl = "https://www.imdb.com/title/{0}";
    private const string MetacriticMovieUrl = "https://www.metacritic.com/movie/{0}";
    private const string MetacriticTvUrl = "https://www.metacritic.com/tv/{0}";
    //private const string TraktMovieUrl = "https://trakt.tv/movies/{0}-{1}";
    //private const string TraktShowUrl = "https://trakt.tv/shows/{0}";

    public Ratings GetMovieData(string? imdbId, string? tmdbRating, string? title, string? year)
    {
        var titleMeta = title?.RemoveSpecialCharacters().RemoveDiacritics().Replace(" ", "-").Replace("--", "-").ToLower();
        //var titleTrakt = title?.RemoveSpecialCharacters(null, '-').RemoveDiacritics().Replace(" ", "-").Replace("--", "-").Replace("--", "-").ToLower();

        var rottenTomatoesRating = ratingApiRoot?.result?.ratings?.RottenTomatoes?.audience?.rating;
        if (rottenTomatoesRating.HasValue) rottenTomatoesRating /= 10;

        var letterboxdRating = ratingApiRoot?.result?.ratings?.Letterboxd?.audience?.rating;
        if (letterboxdRating.HasValue) letterboxdRating *= 2;

        var data = new Ratings
        {
            imdbId = imdbId,
            type = MediaType.movie,
            tmdb = tmdbRating,
            tmdbLink = ratingApiRoot?.result?.links?.TMDB,
            metacritic = ratingApiRoot?.result?.ratings?.Metacritic?.audience?.rating.ToString(CultureInfo.InvariantCulture),
            metacriticLink = ratingApiRoot?.result?.links?.Metacritic,
            imdb = ratingApiRoot?.result?.ratings?.IMDb?.audience?.rating.ToString(CultureInfo.InvariantCulture),
            imdbLink = ratingApiRoot?.result?.links?.IMDb,
            trakt = null,
            traktLink = ratingApiRoot?.result?.links?.Trakt,
            rottenTomatoes = rottenTomatoesRating?.ToString(CultureInfo.InvariantCulture),
            rottenTomatoesLink = ratingApiRoot?.result?.links?.RottenTomatoes,
            filmAffinity = ratingApiRoot?.result?.ratings?.FilmAffinity?.audience?.rating.ToString(CultureInfo.InvariantCulture),
            filmAffinityLink = ratingApiRoot?.result?.links?.FilmAffinity,
            letterboxd = letterboxdRating?.ToString(CultureInfo.InvariantCulture),
            letterboxdLink = ratingApiRoot?.result?.links?.Letterboxd
        };

        //tmdb is not necessary
        if (data.metacritic.Empty()) ProcessMovieMetacritic(data, ratingApiRoot?.result?.links?.Metacritic ?? string.Format(MetacriticMovieUrl, titleMeta), year);
        if (imdbId.NotEmpty() && data.imdb.Empty()) ProcessMovieImdb(data, string.Format(ImdbRatingUrl, imdbId));
        //if (data.trakt.Empty()) ProcessTrack(data, ratingApiRoot?.result?.links?.Trakt ?? string.Format(TraktMovieUrl, titleTrakt, year), year); //its difficult to scrape (cloudflare)
        //rotten tomatoes -- its difficult to scrape
        //film affinity -- its difficult to scrape (cloudflare)
        //letterboxd -- always present if there is url

        return data;
    }

    public Ratings GetShowData(string? imdbId, string? tmdbRating, string? title, string? year)
    {
        var titleMeta = title?.RemoveSpecialCharacters().RemoveDiacritics().Replace(" ", "-").Replace("--", "-").ToLower();
        //var titleTrakt = title?.RemoveSpecialCharacters(null, '-').RemoveDiacritics().Replace(" ", "-").Replace("--", "-").Replace("--", "-").ToLower();

        var rottenTomatoesRating = ratingApiRoot?.result?.ratings?.RottenTomatoes?.audience?.rating;
        if (rottenTomatoesRating.HasValue) rottenTomatoesRating /= 10;

        var letterboxdRating = ratingApiRoot?.result?.ratings?.Letterboxd?.audience?.rating;
        if (letterboxdRating.HasValue) letterboxdRating *= 2;

        var data = new Ratings
        {
            imdbId = imdbId,
            type = MediaType.tv,
            tmdb = tmdbRating,
            tmdbLink = ratingApiRoot?.result?.links?.TMDB,
            metacritic = ratingApiRoot?.result?.ratings?.Metacritic?.audience?.rating.ToString(CultureInfo.InvariantCulture),
            metacriticLink = ratingApiRoot?.result?.links?.Metacritic,
            imdb = ratingApiRoot?.result?.ratings?.IMDb?.audience?.rating.ToString(CultureInfo.InvariantCulture),
            imdbLink = ratingApiRoot?.result?.links?.IMDb,
            trakt = null,
            traktLink = ratingApiRoot?.result?.links?.Trakt,
            rottenTomatoes = rottenTomatoesRating?.ToString(CultureInfo.InvariantCulture),
            rottenTomatoesLink = ratingApiRoot?.result?.links?.RottenTomatoes,
            filmAffinity = ratingApiRoot?.result?.ratings?.FilmAffinity?.audience?.rating.ToString(CultureInfo.InvariantCulture),
            filmAffinityLink = ratingApiRoot?.result?.links?.FilmAffinity,
            letterboxd = letterboxdRating?.ToString(CultureInfo.InvariantCulture),
            letterboxdLink = ratingApiRoot?.result?.links?.Letterboxd
        };

        if (imdbId.NotEmpty() && data.imdb.Empty()) ProcessShowImdb(data, string.Format(ImdbUrl, imdbId));
        if (data.metacritic.Empty()) ProcessShowMetacritic(data, string.Format(MetacriticTvUrl, titleMeta), year);
        //if (data.trakt.Empty()) ProcessTrack(data, string.Format(TraktShowUrl, titleTrakt), year);

        return data;
    }

    private static string? GetNodeInnerText(HtmlDocument doc, string xPath)
    {
        return doc.DocumentNode.SelectNodes(xPath)?.FirstOrDefault()?.InnerText.Trim();
    }

    private void ProcessMovieMetacritic(Ratings data, string metacriticPath, string? year)
    {
        try
        {
            if (metacriticPath.Empty()) return;

            var web = new HtmlWeb();
            var doc = web.Load(metacriticPath);

            if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic"))
            {
                doc = web.Load($"{metacriticPath}-{year}");

                if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic")) return;
            }

            //getting user score
            data.metacritic = GetNodeInnerText(doc, "/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[2]/div[2]/div[1]/div[2]/div/div/span");

            if (data.metacritic == "tbd")
            {
                doc = web.Load($"{metacriticPath}-{year}");
                data.metacritic = GetNodeInnerText(doc, "/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[2]/div[2]/div[1]/div[2]/div/div/span");
            }

            var pageYear = GetNodeInnerText(doc, "/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[1]/div/div[1]/div[2]/div/ul/li[1]/span");

            if (year != null && pageYear != null && year != pageYear)
            {
                //probably wrong title, then try to search by other url
                doc = web.Load($"{metacriticPath}-{year}");
                data.metacritic = GetNodeInnerText(doc, "/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[2]/div[2]/div[1]/div[2]/div/div/span");
            }

            if (data.metacritic == "tbd") data.metacritic = null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Path: {MetacriticPath}, Year {Year}", metacriticPath, year);
        }
    }

    /// <summary>
    ///     some titles have year on path, some don't
    /// </summary>
    /// <param name="data"></param>
    /// <param name="metacriticPath"></param>
    /// <param name="year"></param>
    private void ProcessShowMetacritic(Ratings data, string metacriticPath, string? year)
    {
        try
        {
            if (metacriticPath.Empty()) return;

            var web = new HtmlWeb();
            var doc = web.Load(metacriticPath);

            if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic"))
            {
                doc = web.Load($"{metacriticPath}-{year}");

                if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic")) return;
            }

            //getting user score
            data.metacritic = GetNodeInnerText(doc, "html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[4]/div[2]/div[1]/div[2]/div/div/span");

            if (data.metacritic == "tbd")
            {
                doc = web.Load($"{metacriticPath}-{year}");
                data.metacritic = GetNodeInnerText(doc, "html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[4]/div[2]/div[1]/div[2]/div/div/span");
            }

            var pageYear = GetNodeInnerText(doc, "/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[1]/div/div[1]/div[2]/div/ul/li[1]/span");

            if (year != null && pageYear != null && year != pageYear)
            {
                //probably wrong title, then try to search by other url
                doc = web.Load($"{metacriticPath}-{year}");
                data.metacritic = GetNodeInnerText(doc, "html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[4]/div[2]/div[1]/div[2]/div/div/span");
            }

            if (data.metacritic == "tbd") data.metacritic = null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Path: {MetacriticPath}, Year {Year}", metacriticPath, year);
        }
    }

    private void ProcessMovieImdb(Ratings data, string imdbPath)
    {
        try
        {
            if (imdbPath.Empty()) return;

            var web = new HtmlWeb();
            var doc = web.Load(imdbPath);

            data.imdb = GetNodeInnerText(doc, "//*[@id=\"__next\"]/main/div/section/div/section/div/div[1]/section[1]/div[2]/div[2]/div[1]/div[2]/div[2]/div[1]/span[1]");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ImdbPath {ImdbPath}", imdbPath);
        }
    }

    private void ProcessShowImdb(Ratings data, string imdbPath)
    {
        try
        {
            if (imdbPath.Empty()) return;

            var web = new HtmlWeb();
            var doc = web.Load(imdbPath);

            data.imdb = GetNodeInnerText(doc, "//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[2]/div[2]/div/div[1]/a/span/div/div[2]/div[1]/span[1]");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ImdbPath {ImdbPath}", imdbPath);
        }
    }

    //private void ProcessTrack(Ratings data, string traktUrl, string? year)
    //{
    //    if (traktUrl.Empty()) return;

    //    traktUrl = traktUrl.Replace("--", "-");

    //    try
    //    {
    //        var web = new HtmlWeb();
    //        var doc = web.Load(traktUrl);

    //        if (doc.DocumentNode.InnerText.Contains("Page Not Found (404) - Trakt")) return;

    //        var pageYear = GetNodeInnerText(doc, "html/body/div[1]/div[3]/div/div/trakt-render-for[1]/div[1]/div[2]/div/div[2]/div/div[2]/div[2]/div/p");

    //        if (year != null && pageYear != null && year != pageYear)
    //        {
    //            //probably wrong title, then try to search by other url
    //            doc = web.Load($"{traktUrl}-{year}");
    //            data.trakt = GetNodeInnerText(doc, "html/body/div[1]/div[3]/div/div/trakt-render-for[1]/div[1]/div[2]/div/div[2]/div/div[1]/rating[1]/a/div/div/div/div/p")?.Replace("%", "");
    //            data.traktLink = traktUrl;
    //        }
    //        else
    //        {
    //            data.trakt = GetNodeInnerText(doc, "html/body/div[1]/div[3]/div/div/trakt-render-for[1]/div[1]/div[2]/div/div[2]/div/div[1]/rating[1]/a/div/div/div/div/p")?.Replace("%", "");
    //            data.traktLink = traktUrl;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, "TraktUrl {TraktUrl}, Year {Year}", traktUrl, year);
    //    }
    //}
}