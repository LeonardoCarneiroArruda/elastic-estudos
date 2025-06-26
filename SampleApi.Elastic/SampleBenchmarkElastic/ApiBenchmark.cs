using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nest;
using SampleApi.Elastic.Configurations;
using SampleApi.Elastic.Controllers;
using SampleApi.Elastic.Data.Context;
using SampleApi.Elastic.Data.Repository;
using SampleApi.Elastic.Services;

namespace SampleBenchmarkElastic
{
    [MemoryDiagnoser]
    public class ApiBenchmark
    {
        private readonly UsersController _controller;
        private readonly string connection = "Server=localhost;Port=3306;Database=elastic_estudos;User=root;Password=rootpass;";

        public ApiBenchmark()
        {
            var options = new DbContextOptionsBuilder<SampleElasticContext>()
                         .UseMySql(connection, ServerVersion.AutoDetect(connection))
                         .Options;
            SampleElasticContext context = new SampleElasticContext(options);
            IUserRepository _userRepository = new UserRepository(context);

            var elasticSettings = new ElasticSettings()
            {
                Url = "http://localhost:9200",
                DefaultIndex = "users",
                Username = "",
                Password = ""
            };

            IElasticClient client = new ElasticClient(new ConnectionSettings(
                                     new Uri(elasticSettings.Url ?? ""))
                                    .PrettyJson()
                                    .RequestTimeout(TimeSpan.FromMinutes(60)));

            IElasticSearchService _elasticService = new ElasticSearchService(elasticSettings, 
                                                        client, _userRepository);

            _controller = new UsersController(_elasticService);
        }

        [Benchmark]
        [Arguments("admin")]
        [Arguments("editor")]
        public async Task<IActionResult> SearchElastic(string search)
        {
            return await _controller.SearchElastic(search);
        }

        [Benchmark]
        [Arguments("admin")]
        [Arguments("editor")]
        public async Task<IActionResult> SearchDbContext(string search)
        {
            return await _controller.SearchDbContext(search);
        }

    }
}
