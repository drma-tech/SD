namespace SD.Shared.Enums;

public enum DeliveryModel
{
    [FieldSettings("FREEName", Description = "FREEDescription", ResourceType = typeof(Translations.Enum.DeliveryModel))]
    FREE = 0,

    [FieldSettings("AVODName", Description = "AVODDescription", ResourceType = typeof(Translations.Enum.DeliveryModel))]
    AVOD = 1,

    [FieldSettings("SVODName", Description = "SVODDescription", ResourceType = typeof(Translations.Enum.DeliveryModel))]
    SVOD = 2,

    [FieldSettings("TVODName", Description = "TVODDescription", ResourceType = typeof(Translations.Enum.DeliveryModel))]
    TVOD = 3,

    [FieldSettings("PVODName", Description = "PVODDescription", ResourceType = typeof(Translations.Enum.DeliveryModel))]
    PVOD = 4
}