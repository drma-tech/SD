namespace SD.Shared.Enums;

public enum PopularType
{
    [FieldSettings(nameof(Translations.Enum.PopularType.MovieName), ResourceType = typeof(Translations.Enum.PopularType))]
    Movie,

    [FieldSettings(nameof(Translations.Enum.PopularType.ShowName), ResourceType = typeof(Translations.Enum.PopularType))]
    Show
}