using HtmlAgilityPack;
using SD.Shared.Models.List.Imdb;
using System.Net;
using System.Text.RegularExpressions;

namespace SD.API.Core.Scraping;

public partial class ScrapingPopular
{
    private const string MovieUrl = "https://www.imdb.com/chart/moviemeter";
    private const string TvUrl = "https://www.imdb.com/chart/tvmeter";
    private const string PeopleUrl = "https://www.imdb.com/chart/starmeter";

    public static MostPopularData GetMovieData()
    {
        return ProcessHtml(MovieUrl);
    }

    public static MostPopularData GetTvData()
    {
        return ProcessHtml(TvUrl);
    }

    public static MostPopularData GetStarData()
    {
        return ProcessHtmlStar(PeopleUrl);
    }

    private static MostPopularData ProcessHtml(string path)
    {
        var data = new MostPopularData();

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
            var srcset = node.SelectNodes("div/div/div/div/div[1]/div/div[1]/img")?.FirstOrDefault()?.ChildAttributes("srcset").FirstOrDefault()?.Value;
            string? image = null;
            if (srcset != null)
            {
                var matches = ImageSrcSet().Matches(srcset);

                if (matches.Count > 1)
                {
                    image = matches[1].Groups[1].Value;
                }
            }

            var item = new MostPopularDataDetail
            {
                Id = $"tt{idRegex.Value}",
                Title = WebUtility.HtmlDecode(node.SelectNodes("div/div/div/div/div[2]/div[2]/a/h3/text()")?.FirstOrDefault()?.InnerText),
                Year = yearFix == 0 ? "" : yearFix.ToString(),
                Image = image,
                IMDbRating = rating
            };

            if (item.Id is null or "tt") continue; //filter movie with bug on website

            data.Items.Add(item);
        }

        return data;
    }

    private static MostPopularData ProcessHtmlStar(string path)
    {
        var data = new MostPopularData();

        var web = new HtmlWeb();
        var doc = web.Load(path);

        var ul = doc.DocumentNode.SelectNodes("//ul[starts-with(@class,'ipc-metadata-list')]")?.FirstOrDefault();

        if (ul == null) return data;

        foreach (var node in ul.ChildNodes.Take(40))
        {
            var id = node.SelectNodes("div/div/div/div/div[2]/div[2]/a")?.FirstOrDefault()?.ChildAttributes("href").FirstOrDefault()?.Value;
            var idRegex = ImdbStarId().Match(id ?? "");
            //var year = node.SelectNodes("div/div/div/div/div[2]/div[3]/span[1]")?.FirstOrDefault()?.InnerText.Trim().Split("–")[0];
            //_ = int.TryParse(year, out var yearFix);
            //var rating = node.SelectNodes("div/div/div/div/div[2]/span/div/span/span[1]/text()")?.FirstOrDefault()?.InnerText;
            var srcset = node.SelectNodes("div/div/div/div/div[1]/div/div[1]/img")?.FirstOrDefault()?.ChildAttributes("srcset").FirstOrDefault()?.Value;
            string? image = null;
            if (srcset != null)
            {
                var matches = ImageSrcSet().Matches(srcset);

                if (matches.Count > 1)
                {
                    image = matches[2].Groups[1].Value;
                }
            }

            var item = new MostPopularDataDetail
            {
                Id = $"nm{idRegex.Value}",
                Title = WebUtility.HtmlDecode(node.SelectNodes("div/div/div/div/div[2]/div[2]/a/h3/text()")?.FirstOrDefault()?.InnerText),
                //Year = yearFix == 0 ? "" : yearFix.ToString(),
                Image = image,
                //IMDbRating = rating
            };

            if (item.Id is null or "nm") continue; //filter movie with bug on website

            data.Items.Add(item);
        }

        return data;
    }

    [GeneratedRegex("(?<=\\/tt)(\\w*)(?=\\/)")]
    private static partial Regex ImdbId();

    [GeneratedRegex("(?<=\\/nm)(\\w*)(?=\\/)")]
    private static partial Regex ImdbStarId();

    [GeneratedRegex(@"(https:[^\s]+)\s\d+w")]
    private static partial Regex ImageSrcSet();
}