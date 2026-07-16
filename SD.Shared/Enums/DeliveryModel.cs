namespace SD.Shared.Enums;

public enum DeliveryModel
{
    /// <summary>
    /// Free
    /// </summary>
    [FieldSettings("FREEName", Description = "FREEDescription", ResourceType = typeof(Translations.Enum.DeliveryModel))]
    FREE = 0,

    /// <summary>
    /// Advertising
    /// </summary>
    [FieldSettings("AVODName", Description = "AVODDescription", ResourceType = typeof(Translations.Enum.DeliveryModel))]
    AVOD = 1,

    /// <summary>
    /// Subscription
    /// </summary>
    [FieldSettings("SVODName", Description = "SVODDescription", ResourceType = typeof(Translations.Enum.DeliveryModel))]
    SVOD = 2,

    /// <summary>
    /// Transaction
    /// </summary>
    [FieldSettings("TVODName", Description = "TVODDescription", ResourceType = typeof(Translations.Enum.DeliveryModel))]
    TVOD = 3,

    ///// <summary>
    ///// Premium
    ///// </summary>
    //[FieldSettings("PVODName", Description = "PVODDescription", ResourceType = typeof(Translations.Enum.DeliveryModel))]
    //PVOD = 4
}