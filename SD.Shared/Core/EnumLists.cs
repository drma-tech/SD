namespace SD.Shared.Core;

public enum EnumLists
{
    [FieldSettings("ExpectedMovieOfYear", Description = "ExpectedMovieOfYearDesc", ResourceType = typeof(Resources.Translations))]
    ExpectedMovieOf2026 = 8544544,

    [FieldSettings("AwardsOfTheYear", Description = "AwardsOfTheYearDesc", ResourceType = typeof(Resources.Translations))]
    AwardsOfTheYear = 8498534,

    [FieldSettings("EditorsChoiceTitle", Description = "Discover our Editor’s Choice: films and series consistently rated highly across IMDb, TMDb, Metacritic, Rotten Tomatoes, Trakt, FilmAffinity, and Letterboxd.", ResourceType = typeof(Resources.Translations))]
    CertifiedStreamingDiscoveryMovies = 8498673,

    [FieldSettings("EditorsChoiceTitle", Description = "Discover our Editor’s Choice: films and series consistently rated highly across IMDb, TMDb, Metacritic, Rotten Tomatoes, Trakt, FilmAffinity, and Letterboxd.", ResourceType = typeof(Resources.Translations))]
    CertifiedStreamingDiscoveryShows = 8498675,
}