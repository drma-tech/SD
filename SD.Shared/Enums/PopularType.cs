namespace SD.Shared.Enums;

public enum PopularType
{
    [Custom(Name = "MovieName", ResourceType = typeof(Resources.Enum.PopularType))]
    Movie,

    [Custom(Name = "ShowName", ResourceType = typeof(Resources.Enum.PopularType))]
    Show
}