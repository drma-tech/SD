namespace SD.Shared.Models;

public class Configurations
{
    public AzureAd? AzureAd { get; set; }
    public CosmosDB? CosmosDB { get; set; }
    public Firebase? Firebase { get; set; }
    public Paddle? Paddle { get; set; }
    public Apple? Apple { get; set; }
    public Sendgrid? Sendgrid { get; set; }
    public Google? Google { get; set; }
    public TMDB? TMDB { get; set; }
    public RapidAPI? RapidAPI { get; set; }
    public Settings? Settings { get; set; }
}

public class AzureAd
{
    public string? ClientId { get; set; }
    public string? Issuer { get; set; }
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

public class Paddle
{
    public string? CustomerPortalEndpoint { get; set; } = string.Empty;
    public string? Endpoint { get; set; } = string.Empty;
    public string? Token { get; set; } = string.Empty;
    public string? Key { get; set; } = string.Empty; //do not share with users
    public string? Signature { get; set; } = string.Empty; //do not share with users
    public ProductSettings? Standard { get; set; } = new();
    public ProductSettings? Premium { get; set; } = new();
}

public class Apple
{
    public string? Endpoint { get; set; } = string.Empty;
    public string? SharedSecret { get; set; } = string.Empty; //do not share with users
    public string? BundleId { get; set; }
    public ProductSettings? Standard { get; set; } = new();
    public ProductSettings? Premium { get; set; } = new();
}

public class ProductSettings
{
    public string? PriceMonth { get; set; }
    public string? PriceYear { get; set; }
}

public class Sendgrid
{
    public string? Key { get; set; }
}

public class Google
{
    public string? ApiKey { get; set; }
    public string? Captcha { get; set; }
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
public class Settings
{
    public bool ShowAdSense { get; set; } = true;
}