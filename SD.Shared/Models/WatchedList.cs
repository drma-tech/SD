namespace SD.Shared.Model
{
    public class WatchedList : DocumentBase
    {
        public WatchedList() : base(DocumentType.WatchedList, true)
        {
        }

        public List<string> Movies { get; set; } = new();
        public List<string> Shows { get; set; } = new();

        public override void SetIds(string? id)
        {
            SetValues(id);
        }
    }
}