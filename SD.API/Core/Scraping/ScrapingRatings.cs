using HtmlAgilityPack;
using SD.Shared.Models.List;

namespace SD.API.Core.Scraping
{
    public class ScrapingRatings
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

            var data = new Ratings() { imdbId = imdb_id, type = MediaType.movie, tmdb = tmdb_rating };
            ProcessMovieImdb(data, string.Format(imdb_rating_url, imdb_id));
            ProcessMovieMetacritic(data, string.Format(metacritic_movie_url, title_meta), year);
            ProcessTrack(data, string.Format(trakt_movie_url, title_trakt, year), year);
            return data;
        }

        public Ratings GetShowData(string? imdb_id, string? tmdb_rating, string? title, string? year)
        {
            var title_meta = title?.RemoveSpecialCharacters().RemoveDiacritics().Replace(" ", "-").Replace("--", "-").ToLower();
            var title_trakt = title?.RemoveSpecialCharacters(null, '-').RemoveDiacritics().Replace(" ", "-").Replace("--", "-").Replace("--", "-").ToLower();

            var data = new Ratings() { imdbId = imdb_id, type = MediaType.tv, tmdb = tmdb_rating };
            ProcessShowImdb(data, string.Format(imdb_url, imdb_id));
            ProcessShowMetacritic(data, string.Format(metacritic_tv_url, title_meta), year);
            ProcessTrack(data, string.Format(trakt_show_url, title_trakt), year);
            return data;
        }

        //private static void ProcessMovieMetacrict(Ratings data, string? imdb_path)
        //{
        //    var web = new HtmlWeb();
        //    var doc = web.Load(imdb_path);

        //    try
        //    {
        //        //here is getting metascore, not user score
        //        data.metacritic = doc.DocumentNode.SelectNodes("//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[3]/div[2]/div[2]/ul/li[3]/a/span/span[1]/span").FirstOrDefault()?.InnerText;
        //    }
        //    catch
        //    {
        //        //do nothing
        //    }
        //}

        private static void ProcessMovieMetacritic(Ratings data, string metacritic_path, string? year)
        {
            var web = new HtmlWeb();
            var doc = web.Load(metacritic_path);

            try
            {
                var page_year = doc.DocumentNode.SelectNodes("/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[1]/div/div[1]/div[2]/div/ul/li[1]/span").FirstOrDefault()?.InnerText;

                if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic"))
                {
                    doc = web.Load($"{metacritic_path}-{year}");

                    if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic"))
                    {
                        return;
                    }
                }

                //getting user score
                data.metacritic = doc.DocumentNode.SelectNodes("html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[2]/div[2]/div[1]/div[2]/div/div/span").FirstOrDefault()?.InnerText;

                if (data.metacritic == "tbd")
                {
                    doc = web.Load($"{metacritic_path}-{year}");
                    data.metacritic = doc.DocumentNode.SelectNodes("html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[2]/div[2]/div[1]/div[2]/div/div/span").FirstOrDefault()?.InnerText;
                }

                if (year != null && page_year != null && year != page_year)
                {
                    //probably wrong title, then try to search by other url
                    doc = web.Load($"{metacritic_path}-{year}");
                    data.metacritic = doc.DocumentNode.SelectNodes("html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[2]/div[2]/div[1]/div[2]/div/div/span").FirstOrDefault()?.InnerText;
                }

                if (data.metacritic == "tbd") data.metacritic = null;
            }
            catch
            {
                //do nothing
            }
        }

        /// <summary>
        /// some titles have year on path, some dont
        /// </summary>
        /// <param name="data"></param>
        /// <param name="metacritic_path"></param>
        private static void ProcessShowMetacritic(Ratings data, string metacritic_path, string? year)
        {
            var web = new HtmlWeb();
            var doc = web.Load(metacritic_path);

            try
            {
                var page_year = doc.DocumentNode.SelectNodes("/html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[1]/div/div[1]/div[2]/div/ul/li[1]/span").FirstOrDefault()?.InnerText;

                if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic"))
                {
                    doc = web.Load($"{metacritic_path}-{year}");

                    if (doc.DocumentNode.InnerText.Contains("Page Not Found - Metacritic"))
                    {
                        return;
                    }
                }

                //getting user score
                data.metacritic = doc.DocumentNode.SelectNodes("html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[4]/div[2]/div[1]/div[2]/div/div/span").FirstOrDefault()?.InnerText;

                if (data.metacritic == "tbd")
                {
                    doc = web.Load($"{metacritic_path}-{year}");
                    data.metacritic = doc.DocumentNode.SelectNodes("html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[4]/div[2]/div[1]/div[2]/div/div/span").FirstOrDefault()?.InnerText;
                }

                if (year != null && page_year != null && year != page_year)
                {
                    //probably wrong title, then try to search by other url
                    doc = web.Load($"{metacritic_path}-{year}");
                    data.metacritic = doc.DocumentNode.SelectNodes("html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[4]/div[2]/div[1]/div[2]/div/div/span").FirstOrDefault()?.InnerText;
                }

                if (data.metacritic == "tbd") data.metacritic = null;
            }
            catch
            {
                //do nothing
            }
        }

        private static void ProcessMovieImdb(Ratings data, string? imdb_path)
        {
            var web = new HtmlWeb();
            var doc = web.Load(imdb_path);

            try
            {
                data.imdb = doc.DocumentNode.SelectNodes("//*[@id=\"__next\"]/main/div/section/div/section/div/div[1]/section[1]/div[2]/div[2]/div[1]/div[2]/div[2]/div[1]/span[1]").FirstOrDefault()?.InnerText;
            }
            catch
            {
                //do nothing
            }
        }

        private static void ProcessShowImdb(Ratings data, string imdb_path)
        {
            var web = new HtmlWeb();
            var doc = web.Load(imdb_path);

            try
            {
                data.imdb = doc.DocumentNode.SelectNodes("//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[2]/div[2]/div/div[1]/a/span/div/div[2]/div[1]/span[1]").FirstOrDefault()?.InnerText;
            }
            catch
            {
                //do nothing
            }
        }

        private static void ProcessTrack(Ratings data, string trakt_url, string? year)
        {
            var web = new HtmlWeb();
            var doc = web.Load(trakt_url);

            try
            {
                //Disney 100: A Century of Dreams - A Special Edition of 20/20

                if (doc.DocumentNode.InnerText.Contains("Page Not Found (404) - Trakt")) return;

                var page_year = doc.DocumentNode.SelectNodes("html/body/div[2]/section[1]/div[4]/div/div/div[2]/h1/span").FirstOrDefault()?.InnerText;

                if (year != null && page_year != null && year != page_year)
                {
                    //probably wrong title, then try to search by other url
                    doc = web.Load($"{trakt_url}-{year}");
                    data.trakt = doc.DocumentNode.SelectNodes("//*[@id=\"summary-ratings-wrapper\"]/div/div/div/ul[1]/li[1]/a/div[2]/div[1]").FirstOrDefault()?.InnerText.Replace("%", "");
                }
                else
                {
                    data.trakt = doc.DocumentNode.SelectNodes("//*[@id=\"summary-ratings-wrapper\"]/div/div/div/ul[1]/li[1]/a/div[2]/div[1]").FirstOrDefault()?.InnerText.Replace("%", "");
                }
            }
            catch
            {
                //do nothing
            }
        }
    }
}