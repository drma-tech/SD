namespace SD.Shared.Models.Subscription;

public class PaddleConfigurations
{
    public string? CustomerPortalEndpoint { get; set; }
    public string? Token { get; set; }
    public string? ProductStandard { get; set; }
    public string? ProductPremium { get; set; }
    public string? PriceStandardMonth { get; set; }
    public string? PriceStandardYear { get; set; }
    public string? PricePremiumMonth { get; set; }
    public string? PricePremiumYear { get; set; }

    public AccountProduct GetAccountProduct(string? productId)
    {
        if (productId == ProductStandard)
            return AccountProduct.Standard;
        if (productId == ProductPremium)
            return AccountProduct.Premium;
        return AccountProduct.Basic;
    }
}