namespace SD.Shared.Models
{
    public class AllProviders
    {
        public List<ProviderModel> Items { get; set; } = new();
    }

    public class ProviderModel
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public int priority { get; set; }
        public string? description { get; set; }
        public string? link { get; set; }
        public string? logo_path { get; set; }
        public Language? head_language { get; set; }
        public List<Region> regions { get; set; } = new();
        public List<MediaType> types { get; set; } = new();
        public List<DeliveryModel> models { get; set; } = new();
        public List<Plan> plans { get; set; } = new();
    }

    public class Plan
    {
        public string? name { get; set; }
        public decimal price { get; set; }
    }
}