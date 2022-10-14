using SD.Shared.Core;
using SD.Shared.Modal.Enum;

namespace SD.Shared.Model
{
    public class WishList : CosmosBase
    {
        public WishList() : base(CosmosType.WishList)
        {
        }

        public List<WishListItem> Items { get; set; } = new();

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
        public string? logo_path { get; set; }
        public MediaType MediaType { get; set; }
    }
}