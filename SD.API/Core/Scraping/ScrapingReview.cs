using HtmlAgilityPack;
using Newtonsoft.Json;
using SD.Shared.Models.Reviews.MetaCriticSite;
using System.Text.RegularExpressions;

namespace SD.API.Core.Scraping;

public class ScrapingReview
{
    private const string TvUrl = "https://www.metacritic.com/tv/{0}/critic-reviews/?sort-by=Recently%20Added";

    public RootMetacriticReview GetTvReviews(string? tvName, int year)
    {
        if (tvName == null) return new RootMetacriticReview();

        var result = ProcessHtml(string.Format(TvUrl, tvName));

        result ??= ProcessHtml(string.Format(TvUrl, $"{tvName}-{year}"));
        result ??= ProcessHtml(string.Format(TvUrl, CleanTitle(tvName)));
        result ??= ProcessHtml(string.Format(TvUrl, $"{CleanTitle(tvName)}-{year}"));

        return result ?? new RootMetacriticReview();
    }

    private static string CleanTitle(string tvName)
    {
        var wordsToRemove = new[] { "and", "the", "of", "a", "an", "or", "in", "on", "at", "for", "with" };
        var cleanedName = string.Join("-", tvName.Split('-').Where(w => !wordsToRemove.Contains(w.ToLower()) && !string.IsNullOrWhiteSpace(w)));
        cleanedName = Regex.Replace(cleanedName, "-+", "-").TrimEnd('.');
        return cleanedName;
    }

    private static RootMetacriticReview? ProcessHtml(string path)
    {
        var web = new HtmlWeb();
        var doc = web.Load(path);
        var htmlBody = doc.Text;

        var startIndex = htmlBody.IndexOf("j.components=", StringComparison.Ordinal) + "j.components=".Length;
        var endIndex = htmlBody.IndexOf(";j.footer=", startIndex, StringComparison.Ordinal);

        if (startIndex < 0 || endIndex < 0)
        {
            startIndex = htmlBody.IndexOf("k.components=", StringComparison.Ordinal) + "k.components=".Length;
            endIndex = htmlBody.IndexOf(";k.footer=", startIndex, StringComparison.Ordinal);

            if (startIndex < 0 || endIndex < 0)
            {
                startIndex = htmlBody.IndexOf("l.components=", StringComparison.Ordinal) + "l.components=".Length;
                endIndex = htmlBody.IndexOf(";l.footer=", startIndex, StringComparison.Ordinal);

                if (startIndex < 0 || endIndex < 0) return null;
            }
        }

        var jsonContent = htmlBody.Substring(startIndex, endIndex - startIndex).Trim();

        jsonContent = Regex.Replace(jsonContent, @",[b-t],", ",\"\",");
        jsonContent = Regex.Replace(jsonContent, @":[b-t]", ":\"\"");
        jsonContent = Regex.Replace(jsonContent, @":\[[b-t]\]", ":[\"\"]");

        var result = JsonConvert.DeserializeObject<List<RootMetacriticReview>>(jsonContent)?[2];

        if (result != null) result.items = result.items.TakeLast(10).OrderByDescending(x => x.score).ToList();

        return result;
    }
}