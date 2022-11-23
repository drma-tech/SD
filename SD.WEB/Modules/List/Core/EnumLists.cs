using System.Xml.Linq;

namespace SD.WEB.Modules.List.Core
{
    public enum EnumLists
    {
        [Custom(Name = "Best Motion Picture")]
        OscarBestMotionPicture = 7103422,

        [Custom(Name = "Best Animated Feature Film")]
        OscarBestAnimatedFeatureFilm = 7103492,

        [Custom(Name = "Best International Feature Film")]
        OscarInternationalFeatureFilm = 7103493,

        [Custom(Name = "Best Documentary Feature")]
        OscarDocumentaryFeature = 7103488,

        [Custom(Name = "Best Motion Picture - Drama")]
        GoldenGlobesBestMotionPictureDrama = 7103543,

        [Custom(Name = "Best Motion Picture - Musical or Comedy")]
        GoldenGlobesBestMotionPictureMusicalComedy = 7103545,

        [Custom(Name = "Best Motion Picture - Animated")]
        GoldenGlobesBestMotionPictureAnimated = 7103548,

        [Custom(Name = "Outstanding Drama Series")]
        EmmysOutstandingDramaSeries = 7103525,

        [Custom(Name = "Outstanding Comedy Series")]
        EmmysOutstandingComedySeries = 7103519,

        [Custom(Name = "Outstanding Limited Series")]
        EmmysOutstandingLimitedSeries = 7103526,

        [Custom(Name = "Top 100 TV Shows")]
        HollywoodReporterTop100TVShows = 8222348,

        [Custom(Name = "Top 100 Movies")]
        HollywoodReporterTop100Movies = 8222343,

        [Custom(Name = "Top 250")]
        ImdbTop250Movies = 8223821,

        [Custom(Name = "Top 250")]
        ImdbTop250Shows = 8223826,

        [Custom(Name = "Best Film")]
        BaftaBestFilm = 8228253,

        [Custom(Name = "Best Animated Feature Film")]
        BaftaBestAnimatedFeatureFilm = 8228257,

        [Custom(Name = "Best Documentary")]
        BaftaBestDocumentary = 8228258,

        [Custom(Name = "Best Feature")]
        SpiritBestFeature = 8228261,

        [Custom(Name = "Best International Film")]
        SpiritBestInternationalFilm = 8228263,

        [Custom(Name = "Best Documentary")]
        SpiritBestDocumentary = 8228262,

        [Custom(Name = "Best Picture")]
        CriticsChoiceBestPicture = 8228330,

        [Custom(Name = "Best Limited Series")]
        CriticsChoiceBestLimitedSeries = 8228331,

        [Custom(Name = "Palme d'Or")]
        FestivalCannesPalme = 8228360,

        [Custom(Name = "Grand Prix")]
        FestivalCannesGrandPrix = 8228362,

        [Custom(Name = "Palme d'Or - Short Film")]
        FestivalCannesPalmeShortFilm = 8228361,

        [Custom(Name = "Movies - Tomato Meter")]
        RottenTomatoesMoviesTOMATOMETER = 8228415,

        [Custom(Name = "Movies - Audience Score")]
        RottenTomatoesMoviesAUDIENCESCORE = 8228416,

        [Custom(Name = "Best Movies of All Time")]
        MetacriticBestMoviesofAllTime = 8228420,

        [Custom(Name = "Best TV Shows of All Time")]
        MetacriticBestTVShowsofAllTime = 8228423,

        [Custom(Name = "The 100 Best Movies Of All Time")]
        EmpireThe100BestMoviesOfAllTime = 8228434,

        [Custom(Name = "The 100 Best TV Shows Of All Time")]
        EmpireThe100BestTVShowsOfAllTime = 8228435,

        [Custom(Name = "Best Movies of All Time")]
        RollingStoneBestMoviesofAllTime = 8228447,

        [Custom(Name = "Best TV Shows of All Time")]
        RollingStoneBestTVShowsofAllTime = 8228448
    }
}