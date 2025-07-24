using Nest;
using SampleApi.Elastic.Configurations;
using SampleApi.Elastic.Data.Models;
using System.Text.Json;



namespace SampleApi.Elastic.Data.MappingElastic
{
    public static class IndexMappingElastic
    {

        public static async Task MappignIndexElastic(this IServiceCollection services, IElasticClient _client,
            ElasticSettings elasticSettings)
        {
            await MappingIndexUsersElastic(_client, elasticSettings);
            await MappingIndexMovieElastic(_client);
        }


        private static async Task MappingIndexUsersElastic(IElasticClient _client,
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

        private static async Task MappingIndexMovieElastic(IElasticClient _client)
        {

            string indexName = "movie";

            if (_client.Indices.Exists(indexName).Exists)
                return;

            var createIndexResponse = await _client.Indices.CreateAsync(indexName, c => c
               .InitializeUsing(new IndexState { Settings = GetCustomAnalyzerMovie() })
               .Map<Movie>(m => m
                   .Properties(props => props
                       .Text(t => t.Name(n => n.Title).Analyzer("portuguese_analyzer"))
                       .Text(t => t.Name(n => n.Sinopse).Analyzer("portuguese_analyzer"))
                       )));

            // Exibe erro no console, se houver
            if (!createIndexResponse.IsValid)
            {
                Console.WriteLine("❌ Erro ao criar o índice:");
                Console.WriteLine(JsonSerializer.Serialize(createIndexResponse, new JsonSerializerOptions { WriteIndented = true }));
            }
            else
            {
                Console.WriteLine("✅ Índice 'movie' criado com sucesso!");
            }

        }

        private static IndexSettings GetCustomAnalyzerMovie()
        {
            return new IndexSettings
            {
                Analysis = new Analysis
                {
                    Analyzers = new Analyzers
                    {
                        {"portuguese_analyzer",
                            new CustomAnalyzer
                            {
                                Tokenizer = "standard",
                                Filter = new[] { "lowercase", "asciifolding", "portuguese_stop", "portuguese_stemmer" }
                            }
                        }
                    },
                    TokenFilters = new TokenFilters
                    {
                        { "portuguese_stop", new StopTokenFilter { StopWords = "_portuguese_" } },
                        { "portuguese_stemmer", new StemmerTokenFilter { Language = "light_portuguese" } }
                    }
                }
            };
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
