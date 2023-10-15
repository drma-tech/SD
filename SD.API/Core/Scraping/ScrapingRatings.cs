using HtmlAgilityPack;
using SD.Shared.Models.List;

namespace SD.API.Core.Scraping
{
    public class ScrapingRatings
    {
        private readonly string imdb_url = "https://www.imdb.com/title/{0}";
        private readonly string metacritic_url = "https://www.metacritic.com/tv/{0}";

        public Ratings GetMovieData(string imdb_id, string tmdb_rating)
        {
            var data = new Ratings() { imdbId = imdb_id, type = MediaType.movie, tmdb = tmdb_rating };
            ProcessMovieImdb(data, string.Format(imdb_url, imdb_id));
            return data;
        }

        public Ratings GetShowData(string imdb_id, string tmdb_rating, string title)
        {
            var data = new Ratings() { imdbId = imdb_id, type = MediaType.tv, tmdb = tmdb_rating };
            ProcessShowImdb(data, string.Format(imdb_url, imdb_id));
            ProcessShowMetacritic(data, string.Format(metacritic_url, title));
            return data;
        }

        private static void ProcessMovieImdb(Ratings data, string imdb_path)
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
            try
            {
                data.metacritic = doc.DocumentNode.SelectNodes("//*[@id=\"__next\"]/main/div/section[1]/section/div[3]/section/section/div[3]/div[2]/div[2]/ul/li[3]/a/span/span[1]/span").FirstOrDefault()?.InnerText;
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
    }
}