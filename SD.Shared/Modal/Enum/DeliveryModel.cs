using SD.Shared.Core;

namespace SD.Shared.Modal.Enum
{
    public enum DeliveryModel
    {
        [Custom(Name = "FREEName", Description = "FREEDescription", ResourceType = typeof(Resources.Enum.DeliveryModel))]
        FREE = 0,

        [Custom(Name = "AVODName", Description = "AVODDescription", ResourceType = typeof(Resources.Enum.DeliveryModel))]
        AVOD = 1,

        [Custom(Name = "SVODName", Description = "SVODDescription", ResourceType = typeof(Resources.Enum.DeliveryModel))]
        SVOD = 2,

        [Custom(Name = "TVODName", Description = "TVODDescription", ResourceType = typeof(Resources.Enum.DeliveryModel))]
        TVOD = 3,

        [Custom(Name = "PVODName", Description = "PVODDescription", ResourceType = typeof(Resources.Enum.DeliveryModel))]
        PVOD = 4,
    }
}