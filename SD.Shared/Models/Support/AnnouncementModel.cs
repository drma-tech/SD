using SD.Shared.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace SD.Shared.Models.Support
{
    public class AnnouncementModel : ProtectedMainDocument
    {
        public AnnouncementModel() : base(DocumentType.Announcement)
        {
        }

        public List<AnnouncementItem> Items { get; set; } = new();

        protected void Initialize()
        {
            var id = Guid.NewGuid().ToString();

            base.Initialize(id, id);
        }

        public override bool HasValidData()
        {
            return Items.Any();
        }

        public override bool Equals(object? obj)
        {
            return obj is AnnouncementModel q && q.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }

    public class AnnouncementItem
    {
        [Required]
        [Custom(Name = "Title", Prompt = "...")]
        public string? Title { get; set; }

        [Required]
        [Custom(Name = "Description", Prompt = "...")]
        public string? Description { get; set; }

        public DateTime Date { get; set; }
    }
}