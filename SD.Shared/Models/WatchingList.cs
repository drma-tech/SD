namespace SD.Shared.Models
{
    public class WatchingList : DocumentBase
    {
        public WatchingList() : base(DocumentType.WatchingList, true)
        {
        }

        public HashSet<WatchingListItem> Movies { get; init; } = new();
        public HashSet<WatchingListItem> Shows { get; init; } = new();

        public void AddCollection(MediaType? type, string? collectionId, string? collectionName, string? collectionLogo, HashSet<string> watched)
        {
            var item = new WatchingListItem(collectionId, collectionName, collectionLogo, watched);

            if (Contains(type, item))
            {
                foreach (var id in watched)
                {
                    GetCollection(type, collectionId)?.watched.Add(id);
                }
            }
            else
            {
                Items(type).Add(item);
            }
        }

        public bool Contains(MediaType? type, WatchingListItem? item)
        {
            if (item == null) return false;

            return Items(type).Contains(item);
        }

        public WatchingListItem? GetCollection(MediaType? type, string? id)
        {
            return Items(type).FirstOrDefault(f => f.id == id);
        }

        public HashSet<string> GetWatchedItems(MediaType? type, string? collectionId)
        {
            return Items(type).FirstOrDefault(f => f.id == collectionId)?.watched ?? new();
        }

        public void RemoveCollection(MediaType? type, string? collectionId, string? itemId)
        {
            if (collectionId == null) throw new ArgumentNullException(nameof(collectionId));

            var collection = Items(type).FirstOrDefault(f => f.id == collectionId);

            if (collection != null)
            {
                if (itemId == null)
                    collection.watched.Clear();
                else
                    collection.watched.Remove(itemId);

                if (!collection.watched.Any())
                {
                    Items(type).Remove(collection);
                }
            }
        }

        public void SetCollection(MediaType? type, HashSet<WatchingListItem> collection)
        {
            if (type == MediaType.movie)
            {
                Movies.Clear();
                foreach (var item in collection)
                {
                    Movies.Add(item);
                }
            }
            else
            {
                Shows.Clear();
                foreach (var item in collection)
                {
                    Shows.Add(item);
                }
            }
        }
        public override void SetIds(string? id)
        {
            SetValues(id);
        }

        private HashSet<WatchingListItem> Items(MediaType? type) => type == MediaType.movie ? Movies : Shows;
    }

    public sealed class WatchingListItem : IEquatable<WatchingListItem>
    {
        public WatchingListItem(string? id, string? name, string? logo, HashSet<string> watched)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(logo)) throw new ArgumentNullException(nameof(logo));
            if (watched.Count == 0) throw new ArgumentNullException(nameof(watched));

            this.id = id;
            this.name = name;
            this.logo = logo;
            this.watched = watched;
        }

        public string? id { get; init; }
        public string? logo { get; init; }
        public string? name { get; init; }
        public HashSet<string> watched { get; init; }

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