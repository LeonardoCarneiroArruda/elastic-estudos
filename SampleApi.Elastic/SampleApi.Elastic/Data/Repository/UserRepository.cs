using Microsoft.EntityFrameworkCore;
using SampleApi.Elastic.Data.Context;
using SampleApi.Elastic.Models;

namespace SampleApi.Elastic.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly SampleElasticContext _context;

        public UserRepository(SampleElasticContext context)
        {
            _context = context;
        }

        public async Task<List<User>> Search(string search)
        {
            return await _context.User
                        .Where(x => x.FirstName.Contains(search) || x.LastName.Contains(search) || x.Role.Contains(search))
                        .Take(100)
                        .ToListAsync();
        }
    }
}
