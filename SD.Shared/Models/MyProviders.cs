namespace SD.Shared.Model
{
    public class MyProviders : DocumentBase
    {
        public MyProviders() : base(DocumentType.MyProvider, true)
        {
        }

        public List<MyProvidersItem> Items { get; set; } = new();

        public override void SetIds(string? id)
        {
            SetValues(id);
        }
    }

    public class MyProvidersItem
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public string? logo { get; set; }
    }
}