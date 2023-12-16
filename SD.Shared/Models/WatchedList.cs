using SD.Shared.Core.Models;

namespace SD.Shared.Models
{
    public class WatchedList : PrivateMainDocument
    {
        public WatchedList() : base(DocumentType.WatchedList)
        {
        }

        public HashSet<string> Movies { get; init; } = [];
        public HashSet<string> Shows { get; init; } = [];

        public HashSet<string> GetItems(MediaType? type)
        {
            return Items(type);
        }

        public bool Contains(MediaType? type, string? item)
        {
            if (item == null) return false;

            return Items(type).Contains(item);
        }

        public void AddItem(MediaType? type, HashSet<string> items)
        {
            foreach (var item in items)
            {
                Items(type).Add(item);
            }
        }

        public void RemoveItem(MediaType? type, string item)
        {
            Items(type).Remove(item);
        }

        private HashSet<string> Items(MediaType? type) => type == MediaType.movie ? Movies : Shows;

        public override bool HasValidData()
        {
            return Movies.Count != 0 || Shows.Count != 0;
        }
    }
}