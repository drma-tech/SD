﻿using HtmlAgilityPack;
using SD.Shared.Models.List.Imdb;
using System.Text.RegularExpressions;

namespace SD.API.Core.Scraping
{
    public partial class ScrapingPopular
    {
        private readonly string movie_url = "https://www.imdb.com/chart/moviemeter";
        private readonly string tv_url = "https://www.imdb.com/chart/tvmeter";

        public MostPopularData GetMovieData()
        {
            return ProcessHtml(movie_url);
        }

        public MostPopularData GetTvData()
        {
            return ProcessHtml(tv_url);
        }

        private static MostPopularData ProcessHtml(string path)
        {
            var data = new MostPopularData();

            try
            {
                var web = new HtmlWeb();
                var doc = web.Load(path);

                var ul = doc.DocumentNode.SelectNodes("//ul[starts-with(@class,'ipc-metadata-list')]")?.FirstOrDefault();

                if (ul != null)
                {
                    foreach (var node in ul.ChildNodes.Take(40))
                    {
                        var id = node.SelectNodes($"div/div/div/div/div[2]/div[2]/a")?.FirstOrDefault()?.ChildAttributes("href").FirstOrDefault()?.Value;
                        var idRegex = ImdbId().Match(id ?? "");
                        var year = node.SelectNodes($"div/div/div/div/div[2]/div[3]/span[1]")?.FirstOrDefault()?.InnerText.Trim().Split("–")[0];
                        _ = int.TryParse(year, out int year_fix);
                        var rating = node.SelectNodes($"div/div/div/div/div[2]/span/div/span/span[1]/text()")?.FirstOrDefault()?.InnerText;

                        var item = new MostPopularDataDetail
                        {
                            Id = $"tt{idRegex.Value}",
                            RankUpDown = GetRankUpDown(node),
                            Title = node.SelectNodes($"div/div/div/div/div[2]/div[2]/a/h3/text()")?.FirstOrDefault()?.InnerText,
                            Year = year_fix == 0 ? "" : year_fix.ToString(),
                            Image = node.SelectNodes($"div/div/div/div/div[1]/div/div[2]/img")?.FirstOrDefault()?.ChildAttributes("src").FirstOrDefault()?.Value,
                            IMDbRating = rating
                        };

                        data.Items.Add(item);
                    }
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

            var rank = node.SelectNodes($"div/div/div/div/div[2]/div[1]")?.FirstOrDefault()?.InnerText;

            if (string.IsNullOrEmpty(rank)) return null;

            var rankRegex = RankValue().Match(rank);

            var imageRank = node.SelectNodes($"div/div/div/div/div[2]/div[1]/span/svg")?.FirstOrDefault()?.ChildAttributes("class").FirstOrDefault()?.Value;

            if (string.IsNullOrEmpty(imageRank)) return null;

            if (imageRank.Contains("base flat")) return "0";
            else if (imageRank.Contains("base up")) return $"+{rankRegex.Value}";
            else if (imageRank.Contains("base down")) return $"-{rankRegex.Value}";
            else return null;
        }

        [GeneratedRegex("(?<=\\()([0-9]+)(?=\\))")]
        private static partial Regex RankValue();

        [GeneratedRegex("(?<=\\/tt)(\\w*)(?=\\/)")]
        private static partial Regex ImdbId();
    }
}