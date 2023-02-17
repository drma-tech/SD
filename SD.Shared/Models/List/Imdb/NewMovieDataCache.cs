using SD.Shared.Core.Models;
using SD.Shared.Model.List.Imdb;

namespace SD.Shared.Models.List.Imdb
{
    public class NewMovieDataCache : CacheDocument<NewMovieData>
    {
        public NewMovieDataCache()
        { }

        public NewMovieDataCache(NewMovieData data, string key) : base(key, data, ttlCache.one_day)
        { }
    }
}