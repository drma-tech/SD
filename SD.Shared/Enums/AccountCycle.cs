namespace SD.Shared.Enums;

public enum AccountCycle
{
    [Custom(Name = "Monthly", Description = "Month", ResourceType = typeof(Resources.Enum.AccountCycle))]
    Monthly = 1,

    [Custom(Name = "Yearly", Description = "Year", ResourceType = typeof(Resources.Enum.AccountCycle))]
    Yearly = 2
}