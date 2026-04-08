namespace SD.WEB.Modules.Collections.Core;

public enum EnumLists
{
    [Custom(Name = "ExpectedMovieOfYear", Description = "Discover the most anticipated movies of 2026, featuring upcoming blockbusters, fan favorites, and highly awaited releases hitting theaters and streaming.", ResourceType =typeof(Resources.Translations))]
    ExpectedMovieOf2026 = 8544544,

    [Custom(Name = "AwardsOfTheYear", Description = "Explore films and series that won the year’s top awards. Discover main categories, winners, and celebrated titles recognized by critics and audiences.", ResourceType = typeof(Resources.Translations))]
    AwardsOfTheYear = 8498534,

    [Custom(Name = "EditorsChoiceTitle", Description = "Discover our Editor’s Choice: films and series consistently rated highly across IMDb, TMDb, Metacritic, Rotten Tomatoes, Trakt, FilmAffinity, and Letterboxd.", ResourceType = typeof(Resources.Translations))]
    CertifiedStreamingDiscoveryMovies = 8498673,

    [Custom(Name = "EditorsChoiceTitle", Description = "Discover our Editor’s Choice: films and series consistently rated highly across IMDb, TMDb, Metacritic, Rotten Tomatoes, Trakt, FilmAffinity, and Letterboxd.", ResourceType = typeof(Resources.Translations))]
    CertifiedStreamingDiscoveryShows = 8498675,
}