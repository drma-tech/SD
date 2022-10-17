using SD.Shared.Core;

namespace SD.Shared.Model
{
    public class WatchedList : CosmosBase
    {
        public WatchedList() : base(CosmosType.WatchedList)
        {
        }

        public List<string> Movies { get; set; } = new();
        public List<string> Shows { get; set; } = new();

        public override void SetIds(string? IdLoggedUser)
        {
            if (IdLoggedUser == null) return;

            SetId(IdLoggedUser);
            SetPartitionKey(IdLoggedUser);
        }
    }
}