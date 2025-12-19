namespace SD.Shared.Models;

public class Configurations
{
    public CosmosDB? CosmosDB { get; set; }
    public Firebase? Firebase { get; set; }
    public Apple? Apple { get; set; }
    public Stripe? Stripe { get; set; }
    public TMDB? TMDB { get; set; }
    public RapidAPI? RapidAPI { get; set; }
}

public class CosmosDB
{
    public string? DatabaseId { get; set; }
    public string? ConnectionString { get; set; }
}

public class Firebase
{
    public string? ClientId { get; set; }
    public string? PrivateKeyId { get; set; }
    public string? PrivateKey { get; set; }
    public string? ClientEmail { get; set; }
    public string? CertUrl { get; set; }
}

public class Apple
{
    public string? Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// do not share with users
    /// </summary>
    public string? SharedSecret { get; set; } = string.Empty;

    public string? BundleId { get; set; }
    public ProductSettings? Standard { get; set; } = new();
    public ProductSettings? Premium { get; set; } = new();
}

public class Stripe
{
    /// <summary>
    /// do not share with users
    /// </summary>
    public string? ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// do not share with users
    /// </summary>
    public string? SigningSecret { get; set; }

    public ProductSettings? Standard { get; set; } = new();
    public ProductSettings? Premium { get; set; } = new();
}

public class ProductSettings
{
    public string? PriceMonth { get; set; }
    public string? PriceYear { get; set; }
}

public class TMDB
{
    public string? ReadToken { get; set; }
    public string? WriteToken { get; set; }
}

public class RapidAPI
{
    public string? Key { get; set; }
}