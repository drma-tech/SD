using System.ComponentModel.DataAnnotations;

namespace SD.Shared.Models.Support
{
    public class AnnouncementModel : DocumentBase
    {
        public AnnouncementModel() : base(DocumentType.Announcement, true)
        {
        }

        [Required]
        [Custom(Name = "Título", Prompt = "Uma frase que resume seu feedback")]
        public string? Title { get; set; }

        [Required]
        [Custom(Name = "Descrição", Prompt = "Descreva o mais detalhado possível para que possamos entender melhor a situação")]
        public string? Description { get; set; }

        public override void SetIds(string id)
        {
            SetValues(Guid.NewGuid().ToString());
            //IdUserOwner = id.ToString();
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
}