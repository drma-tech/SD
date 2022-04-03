namespace SD.API.Core
{
    public class TmdbOptions
    {
        public const string Section = "TMDB";

        public string BaseUri { get; set; }
        public string BaseUriNew { get; set; }
        public string ApiKey { get; set; }
        public string SmallPosterPath { get; set; }
        public string LargePosterPath { get; set; }
    }

    public class ImdbOptions
    {
        public const string Section = "IMDB";

        public string BaseUri { get; set; }
        public string ApiKey { get; set; }
    }
}