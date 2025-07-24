using SampleApi.Elastic.Data.Models;

namespace SampleApi.Elastic.Services.DTO.Response
{
    public class MovieResponse
    {
        public List<Movie> Data { get; set; }
        public int Total { get; set; }

    }
}
