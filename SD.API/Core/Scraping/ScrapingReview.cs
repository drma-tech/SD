using HtmlAgilityPack;
using SD.Shared.Models.Reviews.MetaCriticSite;
using System.Text.RegularExpressions;

namespace SD.API.Core.Scraping;

public static class ScrapingReview
{
    private const string TvUrl = "https://www.metacritic.com/tv/{0}/critic-reviews/?sort-by=Recently%20Added";

    public static List<RootMetacriticReviewNew> GetTvReviews(string? tvName, int year)
    {
        if (tvName == null) return [];

        var result = ProcessHtml(string.Format(TvUrl, tvName));

        result ??= ProcessHtml(string.Format(TvUrl, $"{tvName}-{year}"));
        result ??= ProcessHtml(string.Format(TvUrl, CleanTitle(tvName)));
        result ??= ProcessHtml(string.Format(TvUrl, $"{CleanTitle(tvName)}-{year}"));

        return result ?? [];
    }

    private static string CleanTitle(string tvName)
    {
        var wordsToRemove = new[] { "and", "the", "of", "a", "an", "or", "in", "on", "at", "for", "with" };
        var cleanedName = string.Join("-", tvName.Split('-').Where(w => !wordsToRemove.Contains(w.ToLower()) && !string.IsNullOrWhiteSpace(w)));
        cleanedName = Regex.Replace(cleanedName, "-+", "-", RegexOptions.None, TimeSpan.FromMilliseconds(100)).TrimEnd('.');
        return cleanedName;
    }

    private static List<RootMetacriticReviewNew> ProcessHtml(string path)
    {
        var web = new HtmlWeb();
        var doc = web.Load(path);

        if (doc.DocumentNode.InnerText.Contains("Page Not Found")) return [];

        var divs = doc.DocumentNode.SelectNodes("//*[@id=\"__nuxt\"]/div[2]/main/div/div/div/div/section/div[3]/div[2]/div");
        if (divs == null) return [];

        List<RootMetacriticReviewNew> result = [];

        foreach (var div in divs)
        {
            var site = div.SelectSingleNode("div[1]/a/text()")?.InnerText;
            var url = div.SelectSingleNode("div[2]/a[2]")?.GetAttributeValue("href", "");
            var reviewer = div.SelectSingleNode("div[2]/a[1]")?.InnerText;
            var score = int.Parse(div.SelectSingleNode("div[1]/a/div/div/span")?.InnerText ?? "0");
            var quote = div.SelectSingleNode("div[1]/div[2]/div")?.InnerText;
            quote ??= div.SelectSingleNode("div[1]/div/div")?.InnerText;

            var reviewerLink = div.SelectSingleNode("div[2]/div") == null;

            result.Add(new RootMetacriticReviewNew
            {
                Site = site?.Trim(),
                Url = reviewerLink ? url?.Trim() : div.SelectSingleNode("div[2]/a[1]")?.GetAttributeValue("href", "")?.Trim(),
                Reviewer = reviewerLink ? reviewer?.Trim() : div.SelectSingleNode("div[2]/div")?.InnerText?.Trim(),
                Score = score,
                Quote = quote?.Trim()
            });
        }

        result = result.TakeLast(10).OrderByDescending(x => x.Score).ToList();

        return result;
    }
}