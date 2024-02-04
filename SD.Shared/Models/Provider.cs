namespace SD.Shared.Models
{
    public class AllProviders
    {
        public List<ProviderModel> Items { get; set; } = [];
    }

    public class ProviderModel
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public int priority { get; set; }
        public string? description { get; set; }
        public string? link { get; set; }
        public string? logo_path { get; set; }
        public List<Region> regions { get; set; } = [];
        public List<MediaType> types { get; set; } = [];
        public List<DeliveryModel> models { get; set; } = [];
        public List<Plan> plans { get; set; } = [];
    }

    public class Plan
    {
        public string? name { get; set; }
        public decimal price { get; set; }
    }
}