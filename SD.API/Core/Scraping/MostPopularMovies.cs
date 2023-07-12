using HtmlAgilityPack;
using SD.Shared.Models.List.Imdb;
using System.Text.RegularExpressions;

namespace SD.API.Core.Scraping
{
    public class MostPopularMovies
    {
        private readonly string? movie_url = "https://www.imdb.com/chart/moviemeter";
        private readonly string? tv_url = "https://www.imdb.com/chart/tvmeter";

        public async Task<MostPopularData> GetMovieData()
        {
            using var client = new HttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, movie_url);
            var response = await client.SendAsync(requestMessage);
            using HttpContent content = response.Content;
            var sourcedata = await content.ReadAsStringAsync();

            return ProcessHtml(sourcedata);
        }

        public async Task<MostPopularData> GetTvData()
        {
            using var client = new HttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, tv_url);
            var response = await client.SendAsync(requestMessage);
            using HttpContent content = response.Content;
            var sourcedata = await content.ReadAsStringAsync();

            return ProcessHtml(sourcedata);
        }

        private static MostPopularData ProcessHtml(string html)
        {
            var doc = new HtmlDocument();
            var data = new MostPopularData();

            doc.LoadHtml(html);

            var ul = doc.DocumentNode.SelectNodes("//ul[starts-with(@class,'ipc-metadata-list')]")?.FirstOrDefault();

            if (ul != null)
            {
                foreach (var node in ul.ChildNodes.Take(40))
                {
                    var rank = node.SelectNodes($"div[2]/div/div/div[1]").FirstOrDefault()?.InnerText;
                    var imageRank = node.SelectNodes($"div[2]/div/div/div[1]/span/svg").FirstOrDefault()?.ChildAttributes("id").First().Value;
                    var rankRegex = Regex.Match(rank, "(?<=\\()([0-9]+)(?=\\))");
                    var id = node.SelectSingleNode($"div[1]/div/a").ChildAttributes("href").FirstOrDefault()?.Value;
                    var idRegex = Regex.Match(id, "(?<=\\/tt)(\\w*)(?=\\/)");
                    var year = node.SelectNodes($"div[2]/div/div/div[3]/span[1]").FirstOrDefault()?.InnerText.Trim().Split("–")[0];
                    _ = int.TryParse(year, out int year_fix);

                    var item = new MostPopularDataDetail
                    {
                        Id = $"tt{idRegex.Value}",
                        RankUpDown = imageRank == "iconContext-dash" ? "0" : imageRank.Contains("-up") ? $"+{rankRegex.Value}" : $"-{rankRegex.Value}",
                        Title = node.SelectNodes($"div[2]/div/div/div[2]/a/h3/text()").FirstOrDefault()?.InnerText,
                        Year = year_fix == 0 ? "" : year_fix.ToString(),
                        Image = node.SelectSingleNode($"div[1]/div/div[2]/img").ChildAttributes("src").First().Value,
                        IMDbRating = node.SelectNodes($"div[2]/div/div/span/div/span").FirstOrDefault()?.InnerText
                    };

                    data.Items.Add(item);
                }
            }

            return data;
        }
    }
}