using SD.Shared.Core.Models;

namespace SD.Shared.Models
{
    public class MyProviders : PrivateMainDocument
    {
        public MyProviders() : base(DocumentType.MyProvider)
        {
        }

        public List<MyProvidersItem> Items { get; set; } = [];

        public override bool HasValidData()
        {
            return Items.Count != 0;
        }

        public void AddItem(HashSet<MyProvidersItem> items)
        {
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(MyProvidersItem item)
        {
            Items.Remove(item);
        }
    }

    public sealed class MyProvidersItem
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? logo { get; set; }
        public Region? region { get; set; }

        public bool Equals(MyProvidersItem? other)
        {
            if (other is null || other.id is null) return false;
            if (id is null) return false;

            return id.Equals(other.id);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as MyProvidersItem);
        }

        public override int GetHashCode()
        {
            return id?.GetHashCode() ?? 0;
        }
    }
}