using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using SD.Shared.Models.List;

namespace SD.API.Core.Scraping
{
    public class ScrapingRatings(ILogger logger, RatingApiRoot? ratingApiRoot)
    {
        private readonly string imdb_url = "https://www.imdb.com/title/{0}";
        private readonly string imdb_rating_url = "https://www.imdb.com/title/{0}/ratings";
        private readonly string metacritic_movie_url = "https://www.metacritic.com/movie/{0}";
        private readonly string metacritic_tv_url = "https://www.metacritic.com/tv/{0}";
        private readonly string trakt_movie_url = "https://trakt.tv/movies/{0}-{1}";
        private readonly string trakt_show_url = "https://trakt.tv/shows/{0}";

        public Ratings GetMovieData(string? imdb_id, string? tmdb_rating, string? title, string? year)
        {
            var title_meta = title?.RemoveSpecialCharacters().RemoveDiacritics().Replace(" ", "-").Replace("--", "-").ToLower();
            var title_trakt = title?.RemoveSpecialCharacters(null, '-').RemoveDiacritics().Replace(" ", "-").Replace("--", "-").Replace("--", "-").ToLower();

            var data = new Ratings()
            {
                imdbId = imdb_id,
                type = MediaType.movie,
                tmdb = tmdb_rating,
                metacritic = ratingApiRoot?.result?.ratings?.Metacritic?.audience?.rating.ToString(),
                imdb = ratingApiRoot?.result?.ratings?.IMDb?.audience?.rating.ToString(),
                rottenTomatoes = ratingApiRoot?.result?.ratings?.RottenTomatoes?.audience?.rating.ToString(),
                filmAffinity = ratingApiRoot?.result?.ratings?.FilmAffinity?.audience?.rating.ToString()
            };

            if (imdb_id.NotEmpty() && data.imdb.Empty()) ProcessMovieImdb(data, string.Format(imdb_rating_url, imdb_id));
            if (data.metacritic.Empty()) ProcessMovieMetacritic(data, string.Format(metacritic_movie_url, title_meta), year);
            if (data.trakt.Empty()) ProcessTrack(data, string.Format(trakt_movie_url, title_trakt, year), year);

            return data;
        }

        public Ratings GetShowData(string? imdb_id, string? tmdb_rating, string? title, string? year)
        {
            var title_meta = title?.RemoveSpecialCharacters().RemoveDiacritics().Replace(" ", "-").Replace("--", "-").ToLower();
            var title_trakt = title?.RemoveSpecialCharacters(null, '-').RemoveDiacritics().Replace(" ", "-").Replace("--", "-").Replace("--", "-").ToLower();

            var data = new Ratings()
            {
                imdbId = imdb_id,
                type = MediaType.tv,
                tmdb = tmdb_rating,
                metacritic = ratingApiRoot?.result?.ratings?.Metacritic?.audience?.rating.ToString(),
                imdb = ratingApiRoot?.result?.ratings?.IMDb?.audience?.rating.ToString(),
                rottenTomatoes = ratingApiRoot?.result?.ratings?.RottenTomatoes?.audience?.rating.ToString(),
                filmAffinity = ratingApiRoot?.result?.ratings?.FilmAffinity?.audience?.rating.ToString()
            };

            if (imdb_id.NotEmpty() && data.imdb.Empty()) ProcessShowImdb(data, string.Format(imdb_url, imdb_id));
            if (data.metacritic.Empty()) ProcessShowMetacritic(data, string.Format(metacritic_tv_url, title_meta), year);
            if (data.trakt.Empty()) ProcessTrack(data, string.Format(trakt_show_url, title_trakt), year);

            return data;
        }

        private static string? GetNodeInnerText(HtmlDocument doc, string xPath)
        {
            return doc.DocumentNode.SelectNodes(xPath)?.FirstOrDefault()?.InnerText?.Trim();
        }

        private void ProcessMovieMetacritic(Ratings data, string metacriticPath, string? year)
        {
            var web = new HtmlWeb();
            var doc = web.Load(metacriticPath);

            try
            {
                if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic"))
                {
                    doc = web.Load($"{metacriticPath}-{year}");

                    if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic"))
                    {
                        return;
                    }
                }

                //getting user score
                data.metacritic = GetNodeInnerText(doc, "/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[2]/div[2]/div[1]/div[2]/div/div/span");

                if (data.metacritic == "tbd")
                {
                    doc = web.Load($"{metacriticPath}-{year}");
                    data.metacritic = GetNodeInnerText(doc, "/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[2]/div[2]/div[1]/div[2]/div/div/span");
                }

                var page_year = GetNodeInnerText(doc, "/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[1]/div/div[1]/div[2]/div/ul/li[1]/span");

                if (year != null && page_year != null && year != page_year)
                {
                    //probably wrong title, then try to search by other url
                    doc = web.Load($"{metacriticPath}-{year}");
                    data.metacritic = GetNodeInnerText(doc, "/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[2]/div[2]/div[1]/div[2]/div/div/span");
                }

                if (data.metacritic == "tbd") data.metacritic = null;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Path: {MetacriticPath}, Year {Year}", [metacriticPath, year]);
            }
        }

        /// <summary>
        /// some titles have year on path, some dont
        /// </summary>
        /// <param name="data"></param>
        /// <param name="metacriticPath"></param>
        private void ProcessShowMetacritic(Ratings data, string metacriticPath, string? year)
        {
            var web = new HtmlWeb();
            var doc = web.Load(metacriticPath);

            try
            {
                //Fullmetal Alchemist: Brotherhood - doesnt exist
                //Rick and Morty - changed 'and' by '&'
                //Attack on Titan - doesnt exist

                if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic"))
                {
                    doc = web.Load($"{metacriticPath}-{year}");

                    if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic"))
                    {
                        return;
                    }
                }

                //getting user score
                data.metacritic = GetNodeInnerText(doc, "html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[4]/div[2]/div[1]/div[2]/div/div/span");

                if (data.metacritic == "tbd")
                {
                    doc = web.Load($"{metacriticPath}-{year}");
                    data.metacritic = GetNodeInnerText(doc, "html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[4]/div[2]/div[1]/div[2]/div/div/span");
                }

                var page_year = GetNodeInnerText(doc, "/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[1]/div/div[1]/div[2]/div/ul/li[1]/span");

                if (year != null && page_year != null && year != page_year)
                {
                    //probably wrong title, then try to search by other url
                    doc = web.Load($"{metacriticPath}-{year}");
                    data.metacritic = GetNodeInnerText(doc, "html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[4]/div[2]/div[1]/div[2]/div/div/span");
                }

                if (data.metacritic == "tbd") data.metacritic = null;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Path: {MetacriticPath}, Year {Year}", [metacriticPath, year]);
            }
        }

        private void ProcessMovieImdb(Ratings data, string? imdbPath)
        {
            var web = new HtmlWeb();
            var doc = web.Load(imdbPath);

            try
            {
                data.imdb = GetNodeInnerText(doc, "//*[@id=\"__next\"]/main/div/section/div/section/div/div[1]/section[1]/div[2]/div[2]/div[1]/div[2]/div[2]/div[1]/span[1]");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ImdbPath {ImdbPath}", [imdbPath]);
            }
        }

        private void ProcessShowImdb(Ratings data, string imdbPath)
        {
            var web = new HtmlWeb();
            var doc = web.Load(imdbPath);

            try
            {
                data.imdb = GetNodeInnerText(doc, "//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[2]/div[2]/div/div[1]/a/span/div/div[2]/div[1]/span[1]");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ImdbPath {ImdbPath}", [imdbPath]);
            }
        }

        private void ProcessTrack(Ratings data, string traktUrl, string? year)
        {
            traktUrl = traktUrl.Replace("--", "-");

            var web = new HtmlWeb();
            var doc = web.Load(traktUrl);

            try
            {
                if (doc.DocumentNode.InnerText.Contains("Page Not Found (404) - Trakt")) return;

                var page_year = GetNodeInnerText(doc, "html/body/div[2]/section[1]/div[4]/div/div/div[2]/h1/span");

                if (year != null && page_year != null && year != page_year)
                {
                    //probably wrong title, then try to search by other url
                    doc = web.Load($"{traktUrl}-{year}");
                    data.trakt = GetNodeInnerText(doc, "//*[@id=\"summary-ratings-wrapper\"]/div/div/div/ul[1]/li[1]/a/div[2]/div[1]")?.Replace("%", "");
                }
                else
                {
                    data.trakt = GetNodeInnerText(doc, "//*[@id=\"summary-ratings-wrapper\"]/div/div/div/ul[1]/li[1]/a/div[2]/div[1]")?.Replace("%", "");
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "TraktUrl {TraktUrl}, Year {Year}", [traktUrl, year]);
            }
        }
    }
}