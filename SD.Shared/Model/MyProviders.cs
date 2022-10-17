using SD.Shared.Core;

namespace SD.Shared.Model
{
    public class MyProviders : CosmosBase
    {
        public MyProviders() : base(CosmosType.MyProvider)
        {
        }

        public List<MyProvidersItem> Items { get; set; } = new();

        public override void SetIds(string? IdLoggedUser)
        {
            if (IdLoggedUser == null) return;

            SetId(IdLoggedUser);
            SetPartitionKey(IdLoggedUser);
        }
    }

    public class MyProvidersItem
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? logo { get; set; }
    }
}