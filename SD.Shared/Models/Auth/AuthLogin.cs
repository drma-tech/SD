using SD.Shared.Core.Types;

namespace SD.Shared.Models.Auth;

public class AuthLogin(string id) : MainDocument(new MainIdentity(MainType.Login, id))
{
    public string? UserId { get; set; } = id;

    public ISet<Access> Accesses { get; set; } = new HashSet<Access>();

    protected override object?[] EqualityValues => [Id];
}

public class Access : EqualityBase<Access>
{
    public DateTimeOffset Date { get; set; }
    public string? Platform { get; set; }
    public string? Ip { get; set; }
    public string? Country { get; set; }

    protected override object?[] EqualityValues => [Date];
}
