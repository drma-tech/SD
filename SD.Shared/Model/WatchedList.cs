using SD.Shared.Core;
using SD.Shared.Modal.Enum;

namespace SD.Shared.Model
{
    public class WatchedList : CosmosBase
    {
        public WatchedList() : base(CosmosType.WatchedList)
        {
        }

        public List<WatchedListItem> Items { get; set; } = new();

        public override void SetIds(string? IdLoggedUser)
        {
            if (IdLoggedUser == null) return;

            SetId(IdLoggedUser);
            SetPartitionKey(IdLoggedUser);
        }
    }

    public class WatchedListItem
    {
        public string? id { get; set; }
        //public string? name { get; set; }
        //public string? logo { get; set; }
        public MediaType type { get; set; }
    }
}