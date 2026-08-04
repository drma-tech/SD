using SD.Shared.Core.Types;

namespace SD.Shared.Models;

public class WishList(string? id) : MainDocument(new MainIdentity(MainType.WishList, id))
{
    public ISet<WishListItem> Movies { get; init; } = new HashSet<WishListItem>();

    public ISet<WishListItem> Shows { get; init; } = new HashSet<WishListItem>();

    public WishListItem? GetItem(MediaType? type, string? id)
    {
        return Items(type).FirstOrDefault(f => string.Equals(f.id, id, StringComparison.Ordinal));
    }

    public bool Contains(MediaType? type, string? id)
    {
        return id != null && Items(type).Contains(new WishListItem(id, name: null, logo: null, runtime: null));
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

    public ISet<WishListItem> Items(MediaType? type)
    {
        return type == MediaType.movie ? Movies : Shows;
    }

    protected override object?[] EqualityValues => [Id];
}

public class WishListItem : EqualityBase<WishListItem>
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

    protected override object?[] EqualityValues => [id];
}