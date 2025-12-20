namespace SD.Shared.Models.Subscription;

public class PaymentConfigurations
{
    public string? CustomerPortalEndpoint { get; set; }
    public string? Token { get; set; }
    public string? PricePremiumWeek { get; set; }
    public string? PricePremiumMonth { get; set; }
    public string? PricePremiumYear { get; set; }

    public string? GetPriceId(AccountProduct product, AccountCycle cycle)
    {
        return (product, cycle) switch
        {
            (AccountProduct.Premium, AccountCycle.Weekly) => PricePremiumWeek,
            (AccountProduct.Premium, AccountCycle.Monthly) => PricePremiumMonth,
            (AccountProduct.Premium, AccountCycle.Yearly) => PricePremiumYear,
            _ => null,
        };
    }
}