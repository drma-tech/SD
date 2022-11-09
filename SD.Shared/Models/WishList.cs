namespace SD.Shared.Model
{
    public class WishList : DocumentBase
    {
        public WishList() : base(DocumentType.WishList, true)
        {
        }

        public List<WishListItem> Movies { get; set; } = new();
        public List<WishListItem> Shows { get; set; } = new();

        public override void SetIds(string? id)
        {
            SetValues(id);
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