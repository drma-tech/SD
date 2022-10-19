using SD.Shared.Core;

namespace SD.Shared.Model
{
    public class WishList : CosmosBase
    {
        public WishList() : base(CosmosType.WishList)
        {
        }

        public List<WishListItem> Movies { get; set; } = new();
        public List<WishListItem> Shows { get; set; } = new();

        public override void SetIds(string? IdLoggedUser)
        {
            if (IdLoggedUser == null) return;

            SetId(IdLoggedUser);
            SetPartitionKey(IdLoggedUser);
        }
    }

    public class WishListItem
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? logo { get; set; }
        public int? runtime { get; set; }
    }
}