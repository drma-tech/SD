namespace SD.Shared.Enums;

public enum AccountCycle
{
    [FieldSettings(nameof(Translations.Enum.AccountCycle.Monthly), Description = nameof(Translations.Enum.AccountCycle.Month), ResourceType = typeof(Translations.Enum.AccountCycle))]
    Monthly = 2,

    [FieldSettings(nameof(Translations.Enum.AccountCycle.Yearly), Description = nameof(Translations.Enum.AccountCycle.Year), ResourceType = typeof(Translations.Enum.AccountCycle))]
    Yearly = 3
}
