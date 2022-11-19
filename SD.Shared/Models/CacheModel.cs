using SD.Shared.Models.News;

namespace SD.Shared.Models
{
    public class CacheModel
    {
        public CacheModel(string key, Flixster? value)
        {
            Id = key;
            Key = key;
            Value = value;
        }

        public string? Id { get; set; }
        public string? Key { get; set; }
        public Flixster? Value { get; set; }
    }
}