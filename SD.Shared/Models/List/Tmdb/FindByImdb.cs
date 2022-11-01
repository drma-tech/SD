namespace SD.Shared.Model.List.Tmdb
{
    public class TvResultFindByImdb
    {
        public string? backdrop_path { get; set; }
        public List<int> genre_ids { get; set; } = new();
        public string? original_language { get; set; }
        public string? poster_path { get; set; }
        public int vote_count { get; set; }
        public double vote_average { get; set; }
        public string? name { get; set; }
        public string? first_air_date { get; set; }
        public string? overview { get; set; }
        public string? original_name { get; set; }
        public List<string> origin_country { get; set; } = new();
        public int id { get; set; }
        public double popularity { get; set; }
    }

    public class FindByImdb
    {
        public List<object>? movie_results { get; set; }
        public List<object>? person_results { get; set; }
        public List<TvResultFindByImdb> tv_results { get; set; } = new();
        public List<object>? tv_episode_results { get; set; }
        public List<object>? tv_season_results { get; set; }
    }
}