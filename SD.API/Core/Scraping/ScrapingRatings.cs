using HtmlAgilityPack;
using SD.Shared.Models.List;

namespace SD.API.Core.Scraping
{
    public class ScrapingRatings
    {
        private readonly string imdb_url = "https://www.imdb.com/title/{0}";
        private readonly string imdb_rating_url = "https://www.imdb.com/title/{0}/ratings";
        private readonly string metacritic_tv_url = "https://www.metacritic.com/tv/{0}";
        private readonly string trakt_movie_url = "https://trakt.tv/movies/{0}-{1}";
        private readonly string trakt_show_url = "https://trakt.tv/shows/{0}";

        public Ratings GetMovieData(string? imdb_id, string? tmdb_rating, string? title, string? year)
        {
            var data = new Ratings() { imdbId = imdb_id, type = MediaType.movie, tmdb = tmdb_rating };
            ProcessMovieImdb(data, string.Format(imdb_url, imdb_id));
            ProcessMovieRatingImdb(data, string.Format(imdb_rating_url, imdb_id));
            ProcessTrack(data, string.Format(trakt_movie_url, title, year));
            return data;
        }

        public Ratings GetShowData(string? imdb_id, string? tmdb_rating, string? title, string? year)
        {
            var data = new Ratings() { imdbId = imdb_id, type = MediaType.tv, tmdb = tmdb_rating };
            ProcessShowImdb(data, string.Format(imdb_url, imdb_id));
            ProcessShowMetacritic(data, string.Format(metacritic_tv_url, title));
            ProcessTrack(data, string.Format(trakt_show_url, title));
            return data;
        }

        private static void ProcessMovieImdb(Ratings data, string? imdb_path)
        {
            var web = new HtmlWeb();
            var doc = web.Load(imdb_path);

            try
            {
                data.metacritic = doc.DocumentNode.SelectNodes("//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[3]/div[2]/div[2]/ul/li[3]/a/span/span[1]/span").FirstOrDefault()?.InnerText;
            }
            catch
            {
                //do nothing
            }
        }

        private static void ProcessMovieRatingImdb(Ratings data, string? imdb_path)
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

        private static void ProcessShowMetacritic(Ratings data, string metacritic_path)
        {
            var web = new HtmlWeb();
            var doc = web.Load(metacritic_path);

            try
            {
                data.metacritic = doc.DocumentNode.SelectNodes("html/body/div[1]/div/div/div[2]/div[1]/div[1]/div/div/div[2]/div[3]/div[4]/div[1]/div/div[1]/div[2]/div/div/span").FirstOrDefault()?.InnerText;
            }
            catch
            {
                //do nothing
            }
        }

        private static void ProcessTrack(Ratings data, string trakt_url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(trakt_url);

            try
            {
                data.trakt = doc.DocumentNode.SelectNodes("//*[@id=\"summary-ratings-wrapper\"]/div/div/div/ul[1]/li[1]/a/div[2]/div[1]").FirstOrDefault()?.InnerText.Replace("%", "");
            }
            catch
            {
                //do nothing
            }
        }
    }
}