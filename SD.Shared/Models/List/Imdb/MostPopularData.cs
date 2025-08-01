﻿namespace SD.Shared.Models.List.Imdb;

public class MostPopularData
{
    public List<MostPopularDataDetail> Items { get; set; } = [];

    public string? ErrorMessage { get; set; }
}

public class MostPopularDataDetail
{
    public string? Id { get; set; }
    public string? RankUpDown { get; set; }
    public string? Title { set; get; }
    public string? Year { set; get; }
    public string? Image { get; set; }
    public string? IMDbRating { get; set; }
}