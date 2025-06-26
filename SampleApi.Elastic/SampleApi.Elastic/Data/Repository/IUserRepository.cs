using SampleApi.Elastic.Models;

namespace SampleApi.Elastic.Data.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> Search(string search);

    }
}
