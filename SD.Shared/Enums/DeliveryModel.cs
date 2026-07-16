namespace SD.Shared.Enums;

public enum DeliveryModel
{
    /// <summary>
    /// Free
    /// </summary>
    [FieldSettings(nameof(Translations.Enum.DeliveryModel.FREEName), Description = nameof(Translations.Enum.DeliveryModel.FREEDescription), ResourceType = typeof(Translations.Enum.DeliveryModel))]
    FREE = 0,

    /// <summary>
    /// Advertising
    /// </summary>
    [FieldSettings(nameof(Translations.Enum.DeliveryModel.AVODName), Description = nameof(Translations.Enum.DeliveryModel.AVODDescription), ResourceType = typeof(Translations.Enum.DeliveryModel))]
    AVOD = 1,

    /// <summary>
    /// Subscription
    /// </summary>
    [FieldSettings(nameof(Translations.Enum.DeliveryModel.SVODName), Description = nameof(Translations.Enum.DeliveryModel.SVODDescription), ResourceType = typeof(Translations.Enum.DeliveryModel))]
    SVOD = 2,

    /// <summary>
    /// Transaction
    /// </summary>
    [FieldSettings(nameof(Translations.Enum.DeliveryModel.TVODName), Description = nameof(Translations.Enum.DeliveryModel.TVODDescription), ResourceType = typeof(Translations.Enum.DeliveryModel))]
    TVOD = 3,

    ///// <summary>
    ///// Premium
    ///// </summary>
    //[FieldSettings(nameof(Translations.Enum.DeliveryModel.PVODName), Description = nameof(Translations.Enum.DeliveryModel.PVODDescription), ResourceType = typeof(Translations.Enum.DeliveryModel))]
    //PVOD = 4
}