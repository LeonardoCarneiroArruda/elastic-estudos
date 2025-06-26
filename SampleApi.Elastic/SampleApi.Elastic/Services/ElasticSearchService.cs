using CsvHelper;
using Elasticsearch.Net;
using Nest;
using SampleApi.Elastic.Configurations;
using SampleApi.Elastic.Data.Repository;
using SampleApi.Elastic.Models;
using SampleApi.Elastic.Services.DTO.Response;
using System.Globalization;

namespace SampleApi.Elastic.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly IElasticClient _client;
        private readonly ElasticSettings _elasticSettings;
        private readonly IUserRepository _userRepository;

        public ElasticSearchService(ElasticSettings settings,
            IElasticClient client,
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _client = client;
            _elasticSettings = settings;
        }

        public async Task<bool> AddOrUpdate(User user)
        {

            var response = await _client.IndexAsync(user, idx => idx
                .Index(_elasticSettings.DefaultIndex)
                .Id(user.Id)
                .OpType(OpType.Index)
                .Refresh(Refresh.WaitFor)
            );

            return response.IsValid;
        }

        public async Task CreateIndex(string indexName)
        {
            if (!_client.Indices.Exists(indexName).Exists)
            {
                await _client.Indices.CreateAsync(indexName);
            }
        }

        public async Task<User> Get(string key)
        {
            var response = await _client.GetAsync<User>(key, 
                            g => g.Index(_elasticSettings.DefaultIndex));

            return response.Source;
        }

        public async Task<List<User>> GetAll()
        {
            var response = await _client.SearchAsync<User>(g => 
                                    g.Index(_elasticSettings.DefaultIndex)
                                    .Size(1000));

            return response.IsValid ? response.Documents.ToList() : default;
        }

        public async Task<bool> Remove(string key)
        {
            var response = await _client.DeleteAsync<User>(key, 
                                  d => d.Index(_elasticSettings.DefaultIndex));

            return response.IsValid;
        }

        public async Task<UserResponse> SearchUsersAsync(string search)
        {

            var response = await _client.SearchAsync<User>(s => s
                               .Index(_elasticSettings.DefaultIndex)
                               .Query(q => q
                                   .MultiMatch(mm => mm
                                       .Fields(f => f
                                           .Field("firstName")
                                           .Field("lastName")
                                           .Field("role")
                                       )
                                       .Query(search)
                                       .Type(TextQueryType.BoolPrefix) // importante: usa edge_ngram
                                   )
                               )
                               .Size(100) // ou ajuste conforme necessário
                               .TrackTotalHits(false));

            if (!response.IsValid)
            {
                throw new Exception($"Erro ao buscar usuários");
            }


            var users = response.Documents.ToList();
            return new UserResponse() { Data = users, total = users.Count };
        }


        public async Task<UserResponse> SearchDbContextUsersAsync(string search)
        {
            var response = await _userRepository.Search(search);
            return new UserResponse() {Data = response, total = response.Count };
        }

        public async Task ImportCSVToElastic()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Properties", "MOCK_DATA.csv");
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<User>().ToList();

            var descriptor = new BulkDescriptor();

            descriptor.UpdateMany(records, (idx, obj) => idx.Index(_elasticSettings.DefaultIndex)
                                                            //.Id(GetIdValue(propertySearch, obj))
                                                            .Doc(obj)
                                                            .DocAsUpsert(true)
                                                            .RetriesOnConflict(3));

            var response = await _client.BulkAsync(descriptor);

            if (response.Errors)
                Console.WriteLine("Alguns erros ocorreram na importação.");
            else
                Console.WriteLine("Importação concluída com sucesso.");
        }
    }
}
