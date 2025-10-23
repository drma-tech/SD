namespace SD.WEB.Modules.Subscription.Core
{
    public class PlanModel
    {
        public AccountProduct Product { get; set; }
        public AccountCycle Cycle { get; set; }
        public string? Price { get; set; }

        public string? ProductId { get; set; }
        public string? PriceId { get; set; }
    }
}