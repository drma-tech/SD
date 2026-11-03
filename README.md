# Streaming Discovery

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fdrma-tech%2FSD.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fdrma-tech%2FSD?ref=badge_shield)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fdrma-tech%2FSD.svg?type=shield&issueType=security)](https://app.fossa.com/projects/git%2Bgithub.com%2Fdrma-tech%2FSD?ref=badge_shield&issueType=security)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=drma-tech_SD&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=drma-tech_SD)

**Discover what to watch. Explore streaming catalogs. Keep track of what matters to you.**

[Streaming Discovery](https://streamingdiscovery.com/en) is a movie and TV series discovery platform designed to help users explore content from different sources and streaming services.

Its primary focus is content discovery and research. Users can explore movies and series, discover recommendations, check streaming availability, and access detailed information. Personal features complement this experience by helping users keep track of their watchlists and viewing activity.

## Features

### Content Discovery

- Explore movies and TV series from different countries and streaming catalogs.
- Discover popular, trending, upcoming, and highly rated content.
- Explore curated collections and recommendations from major award events.
- Find new titles through detailed search and discovery filters.

### Streaming Availability

- Explore where movies and series are available to stream.
- Discover content across different streaming platforms and regions.

### Content Information

- Access detailed information about movies, TV series, seasons, and episodes.
- Explore cast, crew, genres, ratings, trailers, and related content.
- Follow news and other information related to movies and television.

### Personal Watch Management

- Maintain personal watchlists.
- Track content you want to watch, are watching, or have already watched.
- Keep your personal viewing information in one place.

## Technology Stack

| Area | Technology |
|---|---|
| Frontend | Blazor WebAssembly |
| Backend | Azure Functions, .NET 10 |
| Language | C# |
| UI | MudBlazor |
| Database | Azure Cosmos DB |
| Authentication | Clerk |
| Payments | Stripe and Apple |
| Monitoring | Sentry |
| Analytics | Microsoft Clarity |

## Architecture

The solution is organized into three main projects:

| Project | Description |
|---|---|
| `SD.WEB` | Blazor WebAssembly frontend |
| `SD.API` | Azure Functions backend |
| `SD.Shared` | Shared models and contracts |

The backend integrates with external services to retrieve content information, streaming availability, and other related data.

## External Services

Streaming Discovery uses several external services:

- **TMDB:** Primary source for movie and TV series metadata.
- **RapidAPI:** Provides access to additional content-related data sources.
- **Clerk:** Authentication and user identity management.
- **Stripe:** Subscription and payment processing.
- **Apple:** App Store subscription integration.
- **UserJot:** User feedback and feature requests.
- **ZeptoMail:** Transactional email delivery.
- **Sentry:** Error monitoring and diagnostics.
- **Microsoft Clarity:** Usage analytics.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local)
- An Azure Cosmos DB account
- API credentials for the external services required by the backend

### Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/drma-tech/SD.git
   ```

2. Open `SD.sln` in Visual Studio or your preferred .NET IDE.

3. Configure the backend credentials using .NET User Secrets. See the [Configuration Guide](docs/CONFIGURATION.md) for the required settings.

4. Configure and run the `SD.API` project using Azure Functions Core Tools.

5. Run the `SD.WEB` project to start the frontend.

Some features depend on external services and valid API credentials. A complete local environment requires the corresponding service accounts and configuration.

## Documentation

- [Configuration Guide](docs/CONFIGURATION.md)
- [Roadmap](ROADMAP.md)

## License

This project is licensed under the [Mozilla Public License 2.0](LICENSE.md).
