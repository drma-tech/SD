namespace SD.Shared.Enums;

public enum AccountCycle
{
    [Custom(Name = "Weekly", Description = "Week", ResourceType = typeof(Resources.Enum.AccountCycle))]
    Weekly = 1,

    [Custom(Name = "Monthly", Description = "Month", ResourceType = typeof(Resources.Enum.AccountCycle))]
    Monthly = 2,

    [Custom(Name = "Yearly", Description = "Year", ResourceType = typeof(Resources.Enum.AccountCycle))]
    Yearly = 3
}