using SampleApi.Elastic.Data.Models;

namespace SampleApi.Elastic.Data.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> Search(string search);

    }
}
