namespace SD.Shared.Enums;

public enum PopularType
{
    [FieldSettings("MovieName", ResourceType = typeof(Translations.Enum.PopularType))]
    Movie,

    [FieldSettings("ShowName", ResourceType = typeof(Translations.Enum.PopularType))]
    Show
}