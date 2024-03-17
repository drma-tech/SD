using HtmlAgilityPack;
using SD.Shared.Models.Reviews;

namespace SD.API.Core.Scraping
{
    public class ScrapingReview
    {
        private readonly string tv_url = "https://www.metacritic.com/tv/{0}/critic-reviews";

        public RootMetacritic GetTvReviews(string tv_name)
        {
            return ProcessHtml(string.Format(tv_url, tv_name));
        }

        private static RootMetacritic ProcessHtml(string path)
        {
            var data = new RootMetacritic
            {
                data = new()
                {
                    title = new()
                    {
                        metacritic = new()
                        {
                            reviews = new()
                        }
                    }
                }
            };

            var web = new HtmlWeb();
            var doc = web.Load(path);

            var div = doc.DocumentNode.SelectNodes("//div[starts-with(@class,'critic_reviews')]")?.FirstOrDefault();

            if (div != null)
            {
                foreach (var node in div.ChildNodes.Where(w => w.Name == "div"))
                {
                    _ = int.TryParse(node.SelectNodes("div[1]/div")?.FirstOrDefault()?.InnerText, out int score);
                    var site = node.SelectNodes("div[2]/div[1]/span[1]/a/img")?.FirstOrDefault()?.GetAttributeValue("alt", "site error");
                    if (string.IsNullOrEmpty(site)) //site title is not a image
                    {
                        site = node.SelectNodes("div[2]/div[1]/span[1]/a")?.FirstOrDefault()?.InnerText;
                    }
                    var reviewer = node.SelectNodes($"div[2]/div[1]/span[2]/a")?.FirstOrDefault()?.InnerText;
                    if (string.IsNullOrEmpty(reviewer)) //reviewer doesnt have a link
                    {
                        reviewer = node.SelectNodes($"div[2]/div[1]/span[2]")?.FirstOrDefault()?.InnerText;
                    }
                    var quote = node.SelectNodes("div[2]/div[2]/a[1]")?.FirstOrDefault()?.InnerText;
                    if (string.IsNullOrEmpty(quote)) //quote doesnt have a link
                    {
                        quote = node.SelectNodes("div[2]/div[2]")?.FirstOrDefault()?.InnerText;
                    }

                    if (score != 0)
                    {
                        var item = new Edge
                        {
                            node = new()
                            {
                                quote = new Quote { value = quote },
                                site = site,
                                url = node.SelectNodes($"div[2]/div[2]/a[2]")?.FirstOrDefault()?.GetAttributeValue("href", "url error"),
                                reviewer = reviewer,
                                score = score
                            }
                        };

                        data.data.title.metacritic.reviews.edges.Add(item);
                    }
                }
            }

            return data;
        }
    }
}