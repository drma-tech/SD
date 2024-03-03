using SD.Shared.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SD.Shared.Models.Support
{
    public class UpdateModel : ProtectedMainDocument
    {
        public UpdateModel() : base(DocumentType.Update)
        {
            IsNew = Date > DateTime.Now.AddMonths(-3);
        }

        [Required]
        [Custom(Name = "Title", Prompt = "...")]
        public string? Title { get; set; }

        [Required]
        [Custom(Name = "Description", Prompt = "...")]
        public string? Description { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public bool IsNew { get; set; }

        public void Initialize()
        {
            var id = Guid.NewGuid().ToString();

            base.Initialize(id, id);
        }

        public override bool HasValidData()
        {
            return Title.NotEmpty() && Description.NotEmpty();
        }

        public override bool Equals(object? obj)
        {
            return obj is UpdateModel q && q.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }
}