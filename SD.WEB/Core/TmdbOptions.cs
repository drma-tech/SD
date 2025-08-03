namespace SD.WEB.Core;

/// <summary>
///     this API is free, so it's not a big problem to expose this data
/// </summary>
public static class TmdbOptions
{
    public const string Section = "TMDB";

    public const string BaseUri = "https://api.themoviedb.org/3/";
    public const string BaseUriNew = "https://api.themoviedb.org/4/";
    public const string ApiKey = "3c3f4a904d959f4dbb0c8adda1be9433";
    public const string SmallPosterPath = "https://image.tmdb.org/t/p/w185/";
    public const string LargePosterPath = "https://image.tmdb.org/t/p/w300/";
    public const string OriginalPosterPath = "https://image.tmdb.org/t/p/original/";
    public const string FacePath = "https://image.tmdb.org/t/p/w66_and_h66_face/";
}