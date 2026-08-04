using Newtonsoft.Json;
using SD.Shared.Core.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace SD.Shared.Models;

public class WatchingList(string? id) : MainDocument(new MainIdentity(MainType.WatchingList, id))
{
    public DateTime? MovieSyncDate { get; set; }
    public DateTime? ShowSyncDate { get; set; }

    public ISet<WatchingListItem> Movies { get; init; } = new HashSet<WatchingListItem>();
    public ISet<WatchingListItem> Shows { get; init; } = new HashSet<WatchingListItem>();

    [JsonIgnore]
    [NotMapped]
    public bool MovieCanSync => !MovieSyncDate.HasValue || MovieSyncDate.Value < DateTime.Now.AddDays(-14);

    [JsonIgnore]
    [NotMapped]
    public bool ShowCanSync => !ShowSyncDate.HasValue || ShowSyncDate.Value < DateTime.Now.AddDays(-14);

    public ISet<WatchingListItem> Items(MediaType? type)
    {
        return type == MediaType.movie ? Movies : Shows;
    }

    public bool CanSync(MediaType? type)
    {
        return type == MediaType.movie ? MovieCanSync : ShowCanSync;
    }

    public WatchingListItem? GetItem(MediaType? type, string? id)
    {
        return Items(type).FirstOrDefault(f => string.Equals(f.id, id, StringComparison.Ordinal));
    }

    public ISet<string> GetWatchingItems(MediaType? type, string? collectionId)
    {
        return Items(type).FirstOrDefault(f => string.Equals(f.id, collectionId, StringComparison.Ordinal))?.watched ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public bool Contains(MediaType? type, WatchingListItem? item)
    {
        if (item == null) return false;

        return Items(type).Contains(item);
    }

    public void AddItem(MediaType? type, WatchingListItem newItem)
    {
        if (Contains(type, newItem))
        {
            var item = GetItem(type, newItem.id)!;

            item.maxItems = newItem.maxItems;
            foreach (var _id in newItem.watched) item.watched.Add(_id);
        }
        else
        {
            Items(type).Add(newItem);
        }
    }

    public void RemoveItem(MediaType? type, string? collectionId, string? itemId)
    {
        ArgumentNullException.ThrowIfNull(collectionId);

        var collection = Items(type).FirstOrDefault(f => string.Equals(f.id, collectionId, StringComparison.Ordinal));

        if (collection != null)
        {
            if (itemId == null)
                collection.watched.Clear();
            else
                collection.watched.Remove(itemId);

            if (collection.watched.Count == 0)
            {
                Items(type).Remove(collection);
            }
        }
    }

    protected override object?[] EqualityValues => [Id];
}

public sealed class WatchingListItem : EqualityBase<WatchingListItem>
{
    public WatchingListItem()
    {
    }

    public WatchingListItem(string? id, string? name, string? logo, int maxItems, ISet<string> watched)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        //if (string.IsNullOrEmpty(logo)) throw new ArgumentNullException(nameof(logo)); //some collection has no logo
        if (maxItems == 0) throw new ArgumentNullException(nameof(maxItems));
        if (watched.Count == 0) throw new ArgumentNullException(nameof(watched));

        this.id = id;
        this.name = name;
        this.logo = logo;
        this.maxItems = maxItems;
        this.watched = watched;
    }

    public string? id { get; init; }
    public string? logo { get; init; }
    public string? name { get; init; }
    public int maxItems { get; set; }
    public ISet<string> watched { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    protected override object?[] EqualityValues => [id];
}