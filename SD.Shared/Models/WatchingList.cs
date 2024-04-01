namespace SD.Shared.Models
{
    public class WatchingList : PrivateMainDocument
    {
        public WatchingList() : base(DocumentType.WatchingList)
        {
        }

        public DateTime? MovieSyncDate { get; set; }
        public DateTime? ShowSyncDate { get; set; }

        public HashSet<WatchingListItem> Items(MediaType? type) => type == MediaType.movie ? Movies : Shows;

        public HashSet<WatchingListItem> Movies { get; init; } = [];
        public HashSet<WatchingListItem> Shows { get; init; } = [];

        public HashSet<string> DeletedMovies { get; init; } = [];
        public HashSet<string> DeletedShows { get; init; } = [];

        public bool CanSync(MediaType? type) => type == MediaType.movie ? MovieCanSync : ShowCanSync;
        public bool MovieCanSync => !MovieSyncDate.HasValue || MovieSyncDate.Value < DateTime.Now.AddDays(-14);
        public bool ShowCanSync => !ShowSyncDate.HasValue || ShowSyncDate.Value < DateTime.Now.AddDays(-14);

        public WatchingListItem? GetItem(MediaType? type, string? id)
        {
            return Items(type).FirstOrDefault(f => f.id == id);
        }

        public HashSet<string> GetWatchingItems(MediaType? type, string? collectionId)
        {
            return Items(type).FirstOrDefault(f => f.id == collectionId)?.watched ?? [];
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

                item.watched.Clear();

                item.maxItems = newItem.maxItems;
                foreach (var id in newItem.watched)
                {
                    item.watched.Add(id);
                }
            }
            else
            {
                Items(type).Add(newItem);
            }
        }

        public void RemoveItem(MediaType? type, string? collectionId, string? itemId)
        {
            ArgumentNullException.ThrowIfNull(collectionId);

            var collection = Items(type).FirstOrDefault(f => f.id == collectionId);

            if (collection != null)
            {
                if (itemId == null)
                    collection.watched.Clear();
                else
                    collection.watched.Remove(itemId);

                if (collection.watched.Count == 0)
                {
                    Items(type).Remove(collection);

                    if (type == MediaType.movie)
                        DeletedMovies.Add(collectionId);
                    else
                        DeletedShows.Add(collectionId);
                }
            }
        }

        public override bool HasValidData()
        {
            return Movies.Count != 0 || Shows.Count != 0;
        }
    }

    public sealed class WatchingListItem : IEquatable<WatchingListItem>
    {
        public WatchingListItem()
        {
        }

        public WatchingListItem(string? id, string? name, string? logo, int maxItems, HashSet<string> watched)
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
        public HashSet<string> watched { get; init; } = [];

        public bool Equals(WatchingListItem? other)
        {
            if (other is null || other.id is null) return false;
            if (id is null) return false;

            return id.Equals(other.id);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as WatchingListItem);
        }

        public override int GetHashCode()
        {
            return id?.GetHashCode() ?? 0;
        }
    }
}