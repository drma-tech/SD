namespace SD.Shared.Enums;

public enum AccountCycle
{
    [FieldSettings("Weekly", Description = "Week", ResourceType = typeof(Translations.Enum.AccountCycle))]
    Weekly = 1,

    [FieldSettings("Monthly", Description = "Month", ResourceType = typeof(Translations.Enum.AccountCycle))]
    Monthly = 2,

    [FieldSettings("Yearly", Description = "Year", ResourceType = typeof(Translations.Enum.AccountCycle))]
    Yearly = 3
}