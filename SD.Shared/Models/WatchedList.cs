namespace SD.Shared.Models
{
    public class WatchedList : DocumentBase
    {
        public WatchedList() : base(DocumentType.WatchedList, true)
        {
        }

        public HashSet<string> Movies { get; init; } = new();
        public HashSet<string> Shows { get; init; } = new();

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

        public override void SetIds(string? id)
        {
            SetValues(id);
        }

        private HashSet<string> Items(MediaType? type) => type == MediaType.movie ? Movies : Shows;
    }
}