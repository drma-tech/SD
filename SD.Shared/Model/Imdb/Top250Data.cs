using System.Collections.Generic;

namespace SD.Shared.Modal.Imdb
{
    public class Top250Data
    {
        public List<Top250DataDetail> Items { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class Top250DataDetail
    {
        public string Id { get; set; }
        public string Rank { get; set; }
        public string Title { set; get; }
        public string FullTitle { set; get; }
        public string Year { set; get; }
        public string Image { get; set; }
        public string Crew { get; set; }
        public string IMDbRating { get; set; }
        public string IMDbRatingCount { get; set; }
    }
}