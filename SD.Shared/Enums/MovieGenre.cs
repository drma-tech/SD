namespace SD.Shared.Enums;

public enum MovieGenre
{
    [FieldSettings(nameof(Translations.Enum.MovieGenre.Action), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Action = 28,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Adventure), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Adventure = 12,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Animation), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Animation = 16,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Comedy), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Comedy = 35,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Crime), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Crime = 80,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Documentary), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Documentary = 99,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Drama), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Drama = 18,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Family), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Family = 10751,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Fantasy), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Fantasy = 14,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.History), ResourceType = typeof(Translations.Enum.MovieGenre))]
    History = 36,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Horror), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Horror = 27,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Music), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Music = 10402,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Mystery), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Mystery = 9648,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Romance), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Romance = 10749,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.ScienceFiction), ResourceType = typeof(Translations.Enum.MovieGenre))]
    ScienceFiction = 878,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.TVMovie), ResourceType = typeof(Translations.Enum.MovieGenre))]
    TVMovie = 10770,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Thriller), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Thriller = 53,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.War), ResourceType = typeof(Translations.Enum.MovieGenre))]
    War = 10752,

    [FieldSettings(nameof(Translations.Enum.MovieGenre.Western), ResourceType = typeof(Translations.Enum.MovieGenre))]
    Western = 37
}