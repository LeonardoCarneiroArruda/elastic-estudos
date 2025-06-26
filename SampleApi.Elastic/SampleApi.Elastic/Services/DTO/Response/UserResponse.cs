using SampleApi.Elastic.Models;

namespace SampleApi.Elastic.Services.DTO.Response
{
    public class UserResponse
    {
        public List<User> Data { get; set; }
        public int total { get; set; }

    }
}
