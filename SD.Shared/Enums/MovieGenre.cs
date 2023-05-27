namespace SD.Shared.Enums
{
    public enum MovieGenre
    {
        [Custom(Name = "Action", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Action = 28,

        [Custom(Name = "Adventure", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Adventure = 12,

        [Custom(Name = "Animation", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Animation = 16,

        [Custom(Name = "Comedy", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Comedy = 35,

        [Custom(Name = "Crime", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Crime = 80,

        [Custom(Name = "Documentary", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Documentary = 99,

        [Custom(Name = "Drama", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Drama = 18,

        [Custom(Name = "Family", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Family = 10751,

        [Custom(Name = "Fantasy", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Fantasy = 14,

        [Custom(Name = "History", ResourceType = typeof(Resources.Enum.MovieGenre))]
        History = 36,

        [Custom(Name = "Horror", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Horror = 27,

        [Custom(Name = "Music", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Music = 10402,

        [Custom(Name = "Mystery", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Mystery = 9648,

        [Custom(Name = "Romance", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Romance = 10749,

        [Custom(Name = "ScienceFiction", ResourceType = typeof(Resources.Enum.MovieGenre))]
        ScienceFiction = 878,

        [Custom(Name = "TVMovie", ResourceType = typeof(Resources.Enum.MovieGenre))]
        TVMovie = 10770,

        [Custom(Name = "Thriller", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Thriller = 53,

        [Custom(Name = "War", ResourceType = typeof(Resources.Enum.MovieGenre))]
        War = 10752,

        [Custom(Name = "Western", ResourceType = typeof(Resources.Enum.MovieGenre))]
        Western = 37,
    }
}