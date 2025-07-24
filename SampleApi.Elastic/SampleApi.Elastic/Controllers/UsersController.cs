using Microsoft.AspNetCore.Mvc;
using SampleApi.Elastic.Data.Models;
using SampleApi.Elastic.Services;

namespace SampleApi.Elastic.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IElasticSearchService _elasticsearchService;

        public UsersController(IElasticSearchService elasticService)
        {
            _elasticsearchService = elasticService;
        }

        [HttpPost("create-index")]
        public async Task<IActionResult> CreateIndex([FromBody] string indexName)
        {
            await _elasticsearchService.CreateIndex(indexName);
            return Ok($"Index {indexName} criado ou já existe");
        }

        [HttpPost("add-or-update-user")]
        public async Task<IActionResult> AddOrUpdateUser([FromBody] User user)
        {
            var result = await _elasticsearchService.AddOrUpdate(user);
            return result ? Ok($"Usuário adicionado ou atualizado com sucesso") : 
                BadRequest("Erro ao adicionar ou atualizar usuário");
        }

        [HttpGet("get-user/{key}")]
        public async Task<IActionResult> GetUser(string key)
        {
            var user = await _elasticsearchService.Get(key);
            return user != null ? Ok(user) : NotFound();
        }

        [HttpGet("get-all-user")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _elasticsearchService.GetAll();
            return users != null ? Ok(users) : NotFound();
        }

        [HttpDelete("delete-user/{key}")]
        public async Task<IActionResult> DeleteUser(string key)
        {
            var result = await _elasticsearchService.Remove(key);
            return result ? Ok("Deletado com sucesso") : BadRequest("Erro deleção de usuário");
        }

        [HttpGet("search-elastic")]
        public async Task<IActionResult> SearchElastic(string search) 
        {
            var result = await _elasticsearchService.SearchUsersAsync(search);
            return Ok(result);
        }

        [HttpGet("search-db")]
        public async Task<IActionResult> SearchDbContext(string search)
        {
            var result = await _elasticsearchService.SearchDbContextUsersAsync(search);
            return Ok(result);
        }

        [HttpPost("importcsv")]
        public async Task<IActionResult> ImportUserCSVToElastic()
        {
            await _elasticsearchService.ImportUserCSVToElastic();
            return Ok();
        }

    }
}
