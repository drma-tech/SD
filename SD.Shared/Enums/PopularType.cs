namespace SD.Shared.Enums
{
    public enum PopularType
    {
        [Custom(Name = "StreamingName", ResourceType = typeof(Resources.Enum.PopularType))]
        Streaming,

        [Custom(Name = "RentName", ResourceType = typeof(Resources.Enum.PopularType))]
        Rent,

        [Custom(Name = "CinemaName", ResourceType = typeof(Resources.Enum.PopularType))]
        Cinema
    }
}