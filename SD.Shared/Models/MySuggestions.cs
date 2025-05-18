﻿using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SD.Shared.Models;

public class MySuggestions : PrivateMainDocument
{
    public MySuggestions() : base(DocumentType.MySuggestions)
    {
    }

    public DateTime? MovieSyncDate { get; set; }
    public DateTime? ShowSyncDate { get; set; }

    public HashSet<SuggestionListItem> Movies { get; init; } = [];
    public HashSet<SuggestionListItem> Shows { get; init; } = [];

    [Custom(Name = "Movie Genres")] public IReadOnlyList<MovieGenre> MovieGenres { get; set; } = [];

    [Custom(Name = "TV Genres")] public IReadOnlyList<TvGenre> TvGenres { get; set; } = [];

    [JsonIgnore]
    [NotMapped]
    public bool MovieCanSync => !MovieSyncDate.HasValue || MovieSyncDate.Value < DateTime.Now.AddDays(-4);

    [JsonIgnore]
    [NotMapped]
    public bool ShowCanSync => !ShowSyncDate.HasValue || ShowSyncDate.Value < DateTime.Now.AddDays(-4);

    public HashSet<SuggestionListItem> Items(MediaType? type)
    {
        return type == MediaType.movie ? Movies : Shows;
    }

    public SuggestionListItem? GetItem(MediaType? type, string? id)
    {
        return Items(type).FirstOrDefault(f => f.id == id);
    }

    public bool Contains(MediaType? type, SuggestionListItem? item)
    {
        if (item == null) return false;

        return Items(type).Contains(item);
    }

    public void AddItem(MediaType? type, SuggestionListItem item)
    {
        Items(type).Add(item);
    }

    public override bool HasValidData()
    {
        return Movies.Count != 0 || Shows.Count != 0;
    }
}

public sealed class SuggestionListItem : IEquatable<SuggestionListItem>
{
    public SuggestionListItem()
    {
    }

    public SuggestionListItem(string? id, string? name, string? logo, string[] providers)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrEmpty(logo)) throw new ArgumentNullException(nameof(logo));

        this.id = id;
        this.name = name;
        this.logo = logo;
        Providers = providers;
    }

    public string? id { get; init; }
    public string? logo { get; init; }
    public string? name { get; init; }
    public string[] Providers { get; set; } = [];

    public bool Equals(SuggestionListItem? other)
    {
        if (other is null || other.id is null) return false;
        if (id is null) return false;

        return id.Equals(other.id);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SuggestionListItem);
    }

    public override int GetHashCode()
    {
        return id?.GetHashCode() ?? 0;
    }
}