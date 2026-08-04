using System.Text.Json.Serialization;

namespace SD.Shared.Models;

public class AllProviders
{
    public IReadOnlyCollection<ProviderModel> Items { get; set; } = [];
}

public class ProviderModel
{
    public string? id { get; set; }
    public string? name { get; set; }
    public int priority { get; set; }
    public string? description { get; set; }
    public string? link { get; set; }
    public string? logo_path { get; set; }
    public ICollection<Country> regions { get; set; } = [];
    public ICollection<MediaType> types { get; set; } = [];
    public ICollection<DeliveryModel> models { get; set; } = [];

    [JsonIgnore]
    public string? regions_str => string.Join(", ", regions.Select(r => r.ToString())).Truncate(10);
}