using SD.Shared.Core.Models;

namespace SD.Shared.Models
{
    public class WishList : PrivateMainDocument
    {
        public WishList() : base(DocumentType.WishList)
        {
        }

        public HashSet<WishListItem> Movies { get; init; } = [];

        public HashSet<WishListItem> Shows { get; init; } = [];

        public WishListItem? GetItem(MediaType? type, string? id)
        {
            return Items(type).FirstOrDefault(f => f.id == id);
        }

        public bool Contains(MediaType? type, string? id)
        {
            if (id == null) return false;

            return Items(type).Contains(new WishListItem(id, null, null, null));
        }

        public void AddItem(MediaType? type, WishListItem item)
        {
            Items(type).Add(item);
        }

        public void RemoveItem(MediaType? type, string? id)
        {
            if (id == null) return;

            var item = GetItem(type, id);
            if (item != null) Items(type).Remove(item);
        }

        private HashSet<WishListItem> Items(MediaType? type) => type == MediaType.movie ? Movies : Shows;

        public override bool HasValidData()
        {
            return Movies.Count != 0 || Shows.Count != 0;
        }
    }

    public class WishListItem : IEquatable<WishListItem>
    {
        public WishListItem()
        {
        }

        public WishListItem(string? id, string? name, string? logo, int? runtime)
        {
            this.id = id;
            this.name = name;
            this.logo = logo;
            this.runtime = runtime;
        }

        public string? id { get; init; }
        public string? name { get; init; }
        public string? logo { get; init; }
        public int? runtime { get; init; }

        public bool Equals(WishListItem? other)
        {
            if (other is null || other.id is null) return false;
            if (id is null) return false;

            return id.Equals(other.id);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as WishListItem);
        }

        public override int GetHashCode()
        {
            return id?.GetHashCode() ?? 0;
        }
    }
}