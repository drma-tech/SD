﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using SD.Shared.Models.Reviews.MetaCriticSite;
using System.Text.RegularExpressions;

namespace SD.API.Core.Scraping
{
    public class ScrapingReview
    {
        private readonly string tv_url = "https://www.metacritic.com/tv/{0}/critic-reviews/?sort-by=Recently%20Added";

        public RootMetacriticReview GetTvReviews(string? tv_name, int year)
        {
            var result = ProcessHtml(string.Format(tv_url, tv_name));

            if (result == null)
            {
                result = ProcessHtml(string.Format(tv_url, $"{tv_name}-{year}"));
            }

            return result ?? new();
        }

        private static RootMetacriticReview? ProcessHtml(string path)
        {
            var web = new HtmlWeb();
            var doc = web.Load(path);
            var htmlBody = doc.Text;

            int startIndex = htmlBody.IndexOf("j.components=") + "j.components=".Length;
            int endIndex = htmlBody.IndexOf(";j.footer=", startIndex);

            if (startIndex < 0 || endIndex < 0)
            {
                startIndex = htmlBody.IndexOf("k.components=") + "k.components=".Length;
                endIndex = htmlBody.IndexOf(";k.footer=", startIndex);

                if (startIndex < 0 || endIndex < 0)
                {
                    startIndex = htmlBody.IndexOf("l.components=") + "l.components=".Length;
                    endIndex = htmlBody.IndexOf(";l.footer=", startIndex);

                    if (startIndex < 0 || endIndex < 0)
                    {
                        return null;
                    }
                }
            }

            string jsonContent = htmlBody.Substring(startIndex, endIndex - startIndex).Trim();

            jsonContent = Regex.Replace(jsonContent, @",[b-o],", ",\"\",");
            jsonContent = Regex.Replace(jsonContent, @":[b-o]", ":\"\"");
            jsonContent = Regex.Replace(jsonContent, @":\[[b-o]\]", ":[\"\"]");

            var result = JsonConvert.DeserializeObject<List<RootMetacriticReview>>(jsonContent)?[2];

            if (result != null) result.items = result.items.TakeLast(10).OrderByDescending(x => x.score).ToList();

            return result;
        }
    }
}