﻿namespace SD.Shared.Models.List.Tmdb;

public class ResultMovieTopRated
{
    public string? poster_path { get; set; }
    public bool adult { get; set; }
    public string? overview { get; set; }
    public string? release_date { get; set; }
    public List<int> genre_ids { get; set; } = [];
    public int id { get; set; }
    public string? original_title { get; set; }
    public string? original_language { get; set; }
    public string? title { get; set; }
    public string? backdrop_path { get; set; }
    public double popularity { get; set; }
    public int vote_count { get; set; }
    public bool video { get; set; }
    public double vote_average { get; set; }
}

public class MovieTopRated
{
    public int page { get; set; }
    public List<ResultMovieTopRated> results { get; set; } = [];
    public int total_results { get; set; }
    public int total_pages { get; set; }
}