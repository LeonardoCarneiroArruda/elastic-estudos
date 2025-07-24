using SampleApi.Elastic.Data.Models;
using SampleApi.Elastic.Services.DTO.Response;

namespace SampleApi.Elastic.Services
{
    public interface IElasticSearchService
    {
        // create index
        Task CreateIndex(string indexName);

        // add or update user
        Task<bool> AddOrUpdate(User user);

        Task<User> Get(string key);

        Task<List<User>> GetAll();

        Task<bool> Remove(string key);

        Task<UserResponse> SearchUsersAsync(string search);
        Task<UserResponse> SearchDbContextUsersAsync(string search);

        Task ImportUserCSVToElastic();
        Task ImportMovieCSVToElastic();
        Task<List<Movie>> GetAllMovie();
        Task<MovieResponse> SearchMoviesAsync(string search);
    }
}
