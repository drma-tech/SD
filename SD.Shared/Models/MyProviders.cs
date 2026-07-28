using SD.Shared.Core.Types;

namespace SD.Shared.Models;

public class MyProviders(string? id) : MainDocument(new MainIdentity(MainType.MyProvider, id))
{
    public HashSet<MyProvidersItem> Items { get; set; } = [];

    public void AddItem(HashSet<MyProvidersItem> items)
    {
        foreach (var item in items) Items.Add(item);
    }

    public void RemoveItem(MyProvidersItem item)
    {
        Items.Remove(item);
    }
}

public class MyProvidersItem : EqualityBase<MyProvidersItem>
{
    public string? id { get; set; }
    public string? name { get; set; }
    public string? logo { get; set; }
    public Country? region { get; set; }

    protected override object?[] EqualityValues => [id];
}