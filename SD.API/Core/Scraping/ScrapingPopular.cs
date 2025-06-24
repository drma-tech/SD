using System.Text.RegularExpressions;
using HtmlAgilityPack;
using SD.Shared.Models.List.Imdb;

namespace SD.API.Core.Scraping;

public partial class ScrapingPopular
{
    private const string MovieUrl = "https://www.imdb.com/chart/moviemeter";
    private const string TvUrl = "https://www.imdb.com/chart/tvmeter";

    public static MostPopularData GetMovieData()
    {
        return ProcessHtml(MovieUrl);
    }

    public static MostPopularData GetTvData()
    {
        return ProcessHtml(TvUrl);
    }

    private static MostPopularData ProcessHtml(string path)
    {
        var data = new MostPopularData();

        try
        {
            var web = new HtmlWeb();
            var doc = web.Load(path);

            var ul = doc.DocumentNode.SelectNodes("//ul[starts-with(@class,'ipc-metadata-list')]")?.FirstOrDefault();

            if (ul == null) return data;

            foreach (var node in ul.ChildNodes.Take(40))
            {
                var id = node.SelectNodes("div/div/div/div/div[2]/div[2]/a")?.FirstOrDefault()?.ChildAttributes("href").FirstOrDefault()?.Value;
                var idRegex = ImdbId().Match(id ?? "");
                var year = node.SelectNodes("div/div/div/div/div[2]/div[3]/span[1]")?.FirstOrDefault()?.InnerText.Trim().Split("–")[0];
                _ = int.TryParse(year, out var yearFix);
                var rating = node.SelectNodes("div/div/div/div/div[2]/span/div/span/span[1]/text()")?.FirstOrDefault()?.InnerText;

                var item = new MostPopularDataDetail
                {
                    Id = $"tt{idRegex.Value}",
                    RankUpDown = GetRankUpDown(node),
                    Title = node.SelectNodes("div/div/div/div/div[2]/div[2]/a/h3/text()")?.FirstOrDefault()?.InnerText,
                    Year = yearFix == 0 ? "" : yearFix.ToString(),
                    Image = node.SelectNodes("div/div/div/div/div[1]/div/div[1]/img")?.FirstOrDefault()?.ChildAttributes("src").FirstOrDefault()?.Value,
                    IMDbRating = rating
                };

                if (item.Id is null or "tt") continue; //filter movie with bug on website

                data.Items.Add(item);
            }

            return data;
        }
        catch (Exception)
        {
            return data;
        }
    }

    private static string? GetRankUpDown(HtmlNode? node)
    {
        if (node == null) return null;

        var rank = node.SelectNodes("div/div/div/div/div[2]/div[1]/span")?.FirstOrDefault()?.InnerText;

        if (string.IsNullOrEmpty(rank)) return null;

        var imageRank = node.SelectNodes("div/div/div/div/div[2]/div[1]/span/svg")?.FirstOrDefault()?.ChildAttributes("class").FirstOrDefault()?.Value;

        if (string.IsNullOrEmpty(imageRank)) return null;

        if (imageRank.Contains("base flat")) return "0";
        if (imageRank.Contains("base up")) return $"+{rank}";
        if (imageRank.Contains("base down")) return $"-{rank}";
        return null;
    }

    [GeneratedRegex("(?<=\\/tt)(\\w*)(?=\\/)")]
    private static partial Regex ImdbId();
}