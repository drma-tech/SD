﻿namespace SD.Shared.Models.List.Tmdb;

public class BelongsToCollection
{
    public int id { get; set; }
    public string? name { get; set; }
    public string? poster_path { get; set; }
    public string? backdrop_path { get; set; }
}

public class Genre
{
    public int id { get; set; }
    public string? name { get; set; }
}

public class ProductionCompany
{
    public int id { get; set; }
    public string? logo_path { get; set; }
    public string? name { get; set; }
    public string? origin_country { get; set; }
}

public class ProductionCountry
{
    public string? iso_3166_1 { get; set; }
    public string? name { get; set; }
}

public class SpokenLanguage
{
    public string? english_name { get; set; }
    public string? iso_639_1 { get; set; }
    public string? name { get; set; }
}

public class Result
{
    public string? id { get; set; }
    public string? iso_639_1 { get; set; }
    public string? iso_3166_1 { get; set; }
    public string? key { get; set; }
    public string? name { get; set; }
    public string? site { get; set; }
    public int size { get; set; }
    public string? type { get; set; }
}

public class Videos
{
    public List<Result> results { get; set; } = [];
}

public class MovieDetail
{
    public bool adult { get; set; }
    public string? backdrop_path { get; set; }
    public BelongsToCollection? belongs_to_collection { get; set; }
    public int budget { get; set; }
    public List<Genre> genres { get; set; } = [];
    public string? homepage { get; set; }
    public int id { get; set; }
    public string? imdb_id { get; set; }
    public string? original_language { get; set; }
    public string? original_title { get; set; }
    public string? overview { get; set; }
    public double popularity { get; set; }
    public string? poster_path { get; set; }
    public List<ProductionCompany> production_companies { get; set; } = [];
    public List<ProductionCountry> production_countries { get; set; } = [];
    public string? release_date { get; set; }
    public long revenue { get; set; }
    public int? runtime { get; set; }
    public List<SpokenLanguage> spoken_languages { get; set; } = [];
    public string? status { get; set; }
    public string? tagline { get; set; }
    public string? title { get; set; }
    public bool video { get; set; }
    public double vote_average { get; set; }
    public int vote_count { get; set; }
    public Videos? videos { get; set; }
}