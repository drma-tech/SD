using HtmlAgilityPack;
using SD.Shared.Models.News;

namespace SD.API.Core.Scraping;

public class ScrapingNews
{
    private readonly string news_url = "https://editorial.rottentomatoes.com/news";

    public NewsModel GetNews()
    {
        return ProcessHtml(news_url);
    }

    private static NewsModel ProcessHtml(string path)
    {
        var data = new NewsModel();

        var web = new HtmlWeb();
        var doc = web.Load(path);

        var div = doc.DocumentNode.SelectNodes("//div[starts-with(@class,'panel-body')]/div")?.FirstOrDefault();

        if (div != null)
        {
            var index = 0;
            foreach (var row in div.ChildNodes.Where(w => w.Name == "div"))
            foreach (var col in row.ChildNodes.Where(w => w.Name == "div"))
            {
                var item = new Item
                {
                    id = (++index).ToString(),
                    title = col.SelectNodes("a/div[2]/div/p[1]")?.FirstOrDefault()?.InnerHtml,
                    url_img = col.SelectNodes("a/div[1]/img")?.FirstOrDefault()?.GetAttributeValue("src", "url error"),
                    link = col.SelectNodes("a")?.FirstOrDefault()?.GetAttributeValue("href", "url error")
                };

                data.Items.Add(item);
            }
        }

        return data;
    }
}