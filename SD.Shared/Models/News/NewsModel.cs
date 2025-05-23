﻿namespace SD.Shared.Models.News;

public class NewsModel
{
    public List<Item> Items { get; set; } = [];
}

public class Item
{
    public Item()
    {
    }

    public Item(string? id, string? title, string? url_img, string? link)
    {
        this.id = id;
        this.title = title;
        this.url_img = url_img;
        this.link = link;
    }

    public string? id { get; set; }
    public string? title { get; set; }
    public string? url_img { get; set; }
    public string? link { get; set; }
}