using Microsoft.AspNetCore.Mvc;
using SampleApi.Elastic.Services;

namespace SampleApi.Elastic.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IElasticSearchService _elasticsearchService;

        public MovieController(IElasticSearchService elasticService)
        {
            _elasticsearchService = elasticService;
        }

        [HttpGet()]
        public async Task<IActionResult> SearchMovieBySinopse()
        {
            var users = await _elasticsearchService.GetAllMovie();
            return users != null ? Ok(users) : NotFound();
        }

        [HttpPost("importcsv")]
        public async Task<IActionResult> ImportMovieCSVToElastic()
        {
            await _elasticsearchService.ImportMovieCSVToElastic();
            return Ok();
        }

        [HttpGet("search-elastic")]
        public async Task<IActionResult> SearchElastic(string search)
        {
            var result = await _elasticsearchService.SearchMoviesAsync(search);
            return Ok(result);
        }
    }
}
