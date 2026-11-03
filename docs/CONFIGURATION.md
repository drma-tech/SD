# Configuration

The backend configuration is defined by the `Configurations` class in `SD.API/Core/Configurations.cs`.

During local development, the API loads configuration from:

1. `local.settings.json`
2. .NET User Secrets

The User Secrets provider is added after `local.settings.json`, so matching values from User Secrets take precedence.

## User Secrets

The `SD.API` project already defines a `UserSecretsId`. .NET stores the development secrets in a file named `secrets.json` outside the repository, in the user profile directory.

To open or edit the secrets for this project, use the .NET CLI:

```bash
dotnet user-secrets list --project SD.API
dotnet user-secrets edit --project SD.API
```

The secrets file contains the configuration properties at its root. Do not wrap them in a `Configurations` object.

Example structure:

```json
{
  "CosmosDB": {
    "DatabaseId": "your-database-id",
    "ConnectionString": "your-cosmos-db-connection-string"
  },
  "Apple": {
    "Endpoint": "your-apple-endpoint",
    "SharedSecret": "your-apple-shared-secret",
    "BundleId": "your-app-bundle-id",
    "Premium": {
      "PriceMonth": "your-monthly-product-id",
      "PriceYear": "your-yearly-product-id"
    }
  },
  "Stripe": {
    "ApiKey": "your-stripe-api-key",
    "SigningSecret": "your-stripe-signing-secret",
    "Premium": {
      "PriceMonth": "your-monthly-price-id",
      "PriceYear": "your-yearly-price-id"
    }
  },
  "TMDB": {
    "ReadToken": "your-tmdb-read-token",
    "WriteToken": "your-tmdb-write-token"
  },
  "RapidAPI": {
    "Key": "your-rapidapi-key"
  },
  "ClerkAuth": {
    "SecretKey": "your-clerk-secret-key",
    "SigningSecret": "your-clerk-signing-secret"
  },
  "UserJot": {
    "ProjectId": "your-userjot-project-id",
    "Secret": "your-userjot-secret"
  },
  "ZeptoMail": {
    "ApiKey": "your-zeptomail-api-key"
  },
  "IsMaintenanceMode": false
}
```

Replace the placeholders with credentials and identifiers from the corresponding service accounts. Do not commit real credentials or share the generated `secrets.json` file.

You can also set individual values through the CLI. For example:

```bash
dotnet user-secrets set "CosmosDB:DatabaseId" "your-database-id" --project SD.API
dotnet user-secrets set "CosmosDB:ConnectionString" "your-connection-string" --project SD.API
```

## Configuration Reference

| Section | Property | Purpose |
|---|---|---|
| `CosmosDB` | `DatabaseId` | Azure Cosmos DB database name or identifier used by the application. |
| `CosmosDB` | `ConnectionString` | Azure Cosmos DB connection string. |
| `Apple` | `Endpoint` | Apple server API endpoint configured for the application. |
| `Apple` | `SharedSecret` | Apple subscription shared secret. Treat as confidential. |
| `Apple` | `BundleId` | Apple application bundle identifier. |
| `Apple:Premium` | `PriceMonth` | Monthly premium product identifier. |
| `Apple:Premium` | `PriceYear` | Yearly premium product identifier. |
| `Stripe` | `ApiKey` | Stripe API secret key. Treat as confidential. |
| `Stripe` | `SigningSecret` | Stripe webhook signing secret. Treat as confidential. |
| `Stripe:Premium` | `PriceMonth` | Monthly premium Stripe price identifier. |
| `Stripe:Premium` | `PriceYear` | Yearly premium Stripe price identifier. |
| `TMDB` | `ReadToken` | TMDB API read access token. |
| `TMDB` | `WriteToken` | TMDB API write access token, if required by the configured integration. |
| `RapidAPI` | `Key` | RapidAPI key. |
| `ClerkAuth` | `SecretKey` | Clerk backend secret key. Treat as confidential. |
| `ClerkAuth` | `SigningSecret` | Clerk webhook signing secret. Treat as confidential. |
| `UserJot` | `ProjectId` | UserJot project identifier. |
| `UserJot` | `Secret` | UserJot API secret. Treat as confidential. |
| `ZeptoMail` | `ApiKey` | ZeptoMail API key. Treat as confidential. |
| Root | `IsMaintenanceMode` | Enables or disables application maintenance mode. |

Not every integration is necessarily required to run every part of the application. The required settings depend on which features and API endpoints you intend to use.

## Azure Deployment

For deployed environments, provide the same configuration values through the Function App application settings or the configuration provider used by your deployment process. Use hierarchical keys with double underscores, for example:

```text
CosmosDB__DatabaseId
CosmosDB__ConnectionString
Stripe__ApiKey
ClerkAuth__SecretKey
```

Never deploy development User Secrets or commit credentials to source control.
