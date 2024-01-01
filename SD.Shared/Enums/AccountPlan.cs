namespace SD.Shared.Enums
{
    public enum AccountPlan
    {
        [Custom(Name = "Basic")]
        Basic = 1,

        [Custom(Name = "Standard")]
        Standard = 2,

        [Custom(Name = "Premium")]
        Premium = 3,
    }
}