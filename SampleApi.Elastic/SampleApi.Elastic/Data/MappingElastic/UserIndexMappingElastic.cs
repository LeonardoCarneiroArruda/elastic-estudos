using Nest;
using SampleApi.Elastic.Configurations;
using SampleApi.Elastic.Models;
using System.Text.Json;



namespace SampleApi.Elastic.Data.MappingElastic
{
    public static class UserIndexMappingElastic
    {

        public static async Task MappignIndexUsersElastic(this IServiceCollection services, IElasticClient _client,
            ElasticSettings elasticSettings)
        {

            var indexName = elasticSettings.DefaultIndex;

            if (_client.Indices.Exists(indexName).Exists)
                return;

            var createIndexResponse = await _client.Indices.CreateAsync(indexName, c => c
            .InitializeUsing(new IndexState() { Settings = GetCustomAnalyzer() })
            .Map<User>(m => m
                .Properties(props => props
                    .Text(t => t.Name(n => n.FirstName)
                        .Analyzer("edge_ngram_analyzer")
                        .SearchAnalyzer("standard")
                    )
                    .Text(t => t.Name(n => n.LastName)
                        .Analyzer("edge_ngram_analyzer")
                        .SearchAnalyzer("standard")
                    )
                    .Text(t => t.Name(n => n.Role)
                        .Analyzer("edge_ngram_analyzer")
                        .SearchAnalyzer("standard")
                    )
                )
            ));

            // Exibe erro no console, se houver
            if (!createIndexResponse.IsValid)
            {
                Console.WriteLine("❌ Erro ao criar o índice:");
                Console.WriteLine(JsonSerializer.Serialize(createIndexResponse, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine("✅ Índice 'users' criado com sucesso!");
            }
        }

        private static IndexSettings GetCustomAnalyzer()
        {
            return new IndexSettings
            {
                Analysis = new Analysis
                {
                    Tokenizers = new Tokenizers
                        {
                            {
                                "edge_ngram_tokenizer", new EdgeNGramTokenizer
                                {
                                    MinGram = 2,
                                    MaxGram = 20,
                                    TokenChars = new[] { TokenChar.Letter, TokenChar.Digit }
                                }
                            }
                        },
                    Analyzers = new Analyzers
                        {
                            {
                                "edge_ngram_analyzer", new CustomAnalyzer
                                {
                                    Tokenizer = "edge_ngram_tokenizer",
                                    Filter = new[] { "lowercase" }
                                }
                            }
                        }
                }
            };
        }

    }
}
