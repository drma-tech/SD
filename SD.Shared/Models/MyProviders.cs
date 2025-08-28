namespace SD.Shared.Models;

public class MyProviders() : PrivateMainDocument(DocumentType.MyProvider)
{
    public List<MyProvidersItem> Items { get; set; } = [];

    public void AddItem(HashSet<MyProvidersItem> items)
    {
        foreach (var item in items) Items.Add(item);
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
        if (other?.id is null) return false;
        return id is not null && id.Equals(other.id);
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