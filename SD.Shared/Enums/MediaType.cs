namespace SD.Shared.Enums;

public enum MediaType
{
    [FieldSettings("movieName", ResourceType = typeof(Translations.Enum.MediaType))]
    movie = 1,

    [FieldSettings("tvName", ResourceType = typeof(Translations.Enum.MediaType))]
    tv = 2,

    [FieldSettings("personName", ResourceType = typeof(Translations.Enum.MediaType))]
    person = 3
}