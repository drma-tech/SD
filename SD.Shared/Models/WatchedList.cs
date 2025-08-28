namespace SD.Shared.Models;

public class WatchedList() : PrivateMainDocument(DocumentType.WatchedList)
{
    public HashSet<string> Movies { get; init; } = [];
    public HashSet<string> Shows { get; init; } = [];

    public HashSet<string> GetItems(MediaType? type)
    {
        return Items(type);
    }

    public bool Contains(MediaType? type, string? item)
    {
        return item != null && Items(type).Contains(item);
    }

    public void AddItem(MediaType? type, HashSet<string> items)
    {
        foreach (var item in items) Items(type).Add(item);
    }

    public void RemoveItem(MediaType? type, string item)
    {
        Items(type).Remove(item);
    }

    public HashSet<string> Items(MediaType? type)
    {
        return type == MediaType.movie ? Movies : Shows;
    }
}