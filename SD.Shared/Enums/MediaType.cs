namespace SD.Shared.Enums;

public enum MediaType
{
    [FieldSettings(nameof(Translations.Enum.MediaType.movieName), ResourceType = typeof(Translations.Enum.MediaType))]
    movie = 1,

    [FieldSettings(nameof(Translations.Enum.MediaType.tvName), ResourceType = typeof(Translations.Enum.MediaType))]
    tv = 2,

    [FieldSettings(nameof(Translations.Enum.MediaType.personName), ResourceType = typeof(Translations.Enum.MediaType))]
    person = 3
}