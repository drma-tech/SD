﻿namespace SD.Shared.Models;

public class Configurations
{
    public CosmosDB? CosmosDB { get; set; }
    public Paddle? Paddle { get; set; }
    public Sendgrid? Sendgrid { get; set; }
    public Google? Google { get; set; }
    public TMDB? TMDB { get; set; }
    public RapidAPI? RapidAPI { get; set; }
    public Settings? Settings { get; set; }
}

public class CosmosDB
{
    public string? DatabaseId { get; set; }
    public string? ConnectionString { get; set; }
}

public class Paddle
{
    public string? Endpoint { get; set; } = string.Empty;
    public string? Token { get; set; } = string.Empty;
    public string? Key { get; set; } = string.Empty;
    public string? Signature { get; set; } = string.Empty;
    public PaddleProductSettings? Standard { get; set; } = new();
    public PaddleProductSettings? Premium { get; set; } = new();
}

public class PaddleProductSettings
{
    public string? Product { get; set; }
    public string? PriceMonth { get; set; }
    public string? PriceYear { get; set; }
}

public class Sendgrid
{
    public string? Key { get; set; }
}

public class Google
{
    public string? Captcha { get; set; }
}

public class TMDB
{
    public string? WriteToken { get; set; }
}

public class RapidAPI
{
    public string? Key { get; set; }
}

public class Settings
{
    public bool ShowAdSense { get; set; }
}