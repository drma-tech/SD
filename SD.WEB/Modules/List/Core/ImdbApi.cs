using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Model.List.Imdb;
using System.Globalization;

namespace SD.WEB.Modules.List.Core
{
    public class ImdbApi : ApiServices
    {
        public ImdbApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<HashSet<MediaDetail>> GetPopularList(MediaType type)
        {
            var parameter = new Dictionary<string, string>()
                {
                    { "apiKey", ImdbOptions.ApiKey }
                };

            var list_media = new HashSet<MediaDetail>();

            if (type == MediaType.movie)
            {
                var result = await GetAsync<MostPopularData>(ImdbOptions.BaseUri + "MostPopularMovies".ConfigureParameters(parameter), true); //bring 100 records

                foreach (var item in result?.Items ?? new List<MostPopularDataDetail>())
                {
                    if (item.IMDbRatingCount == "0") continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = new DateTime(int.Parse(item.Year ?? "0"), 1, 1),
                        poster_small = ImdbOptions.ResizeImage + item.Image,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.movie
                    });
                }
            }
            else if (type == MediaType.tv)
            {
                var result = await GetAsync<MostPopularData>(ImdbOptions.BaseUri + "MostPopularTVs".ConfigureParameters(parameter), true); //bring 100 records

                foreach (var item in result?.Items ?? new List<MostPopularDataDetail>())
                {
                    if (item.IMDbRatingCount == "0") continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = new DateTime(int.Parse(item.Year ?? "0"), 1, 1),
                        poster_small = ImdbOptions.ResizeImage + item.Image,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.tv
                    });
                }
            }

            return list_media;
        }

        public async Task<HashSet<MediaDetail>> GetTopRatedList(MediaType type)
        {
            var parameter = new Dictionary<string, string>()
                {
                    { "apiKey", ImdbOptions.ApiKey }
                };

            var list_media = new HashSet<MediaDetail>();

            if (type == MediaType.movie)
            {
                var result = await GetAsync<Top250Data>(ImdbOptions.BaseUri + "Top250Movies".ConfigureParameters(parameter), true); //bring 250 records

                foreach (var item in result?.Items ?? new List<Top250DataDetail>())
                {
                    //if (item.vote_count < 100) continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                    if (new DateTime(int.Parse(item.Year ?? "0"), 1, 1) < DateTime.Now.AddYears(-20)) continue;

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = new DateTime(int.Parse(item.Year ?? "0"), 1, 1),
                        poster_small = ImdbOptions.ResizeImage + item.Image,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.movie
                    });
                }
            }
            else if (type == MediaType.tv)
            {
                var result = await GetAsync<Top250Data>(ImdbOptions.BaseUri + "Top250TVs".ConfigureParameters(parameter), true); //bring 250 records

                foreach (var item in result?.Items ?? new List<Top250DataDetail>())
                {
                    //if (item.vote_count < 100) continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                    if (new DateTime(int.Parse(item.Year ?? "0"), 1, 1) < DateTime.Now.AddYears(-20)) continue;

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = new DateTime(int.Parse(item.Year ?? "0"), 1, 1),
                        poster_small = ImdbOptions.ResizeImage + item.Image,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.tv
                    });
                }
            }

            return list_media;
        }

        public async Task<HashSet<MediaDetail>> GetUpcomingList(MediaType type)
        {
            var parameter = new Dictionary<string, string>()
                {
                    { "apiKey", ImdbOptions.ApiKey }
                };

            var list_media = new HashSet<MediaDetail>();

            if (type == MediaType.movie)
            {
                var result = await GetAsync<NewMovieData>(ImdbOptions.BaseUri + "ComingSoon".ConfigureParameters(parameter), true); //undefined numeric record

                foreach (var item in result?.Items ?? new List<NewMovieDataDetail>())
                {
                    //if (item.vote_count < 100) continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = DateTime.Parse(item.ReleaseState ?? DateTime.MinValue.ToString(), CultureInfo.InvariantCulture),
                        //poster_path_small = ImdbOptions.ResizeImage + item.Image,
                        poster_small = item.Image,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.movie
                    });
                }
            }
            else if (type == MediaType.tv)
            {
                throw new NotImplementedException();
            }

            return list_media;
        }
    }
}