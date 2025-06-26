using Microsoft.EntityFrameworkCore;
using Nest;
using SampleApi.Elastic.Configurations;
using SampleApi.Elastic.Data.Context;
using SampleApi.Elastic.Data.MappingElastic;
using SampleApi.Elastic.Data.Repository;
using SampleApi.Elastic.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var elasticSettings = builder.Configuration.GetSection("ElasticSettings").Get<ElasticSettings>();
var settings = new ConnectionSettings(new Uri(elasticSettings.Url ?? ""))
               .PrettyJson()
               .RequestTimeout(TimeSpan.FromMinutes(60));

builder.Services.AddSingleton<ElasticSettings>(elasticSettings);
var client = new ElasticClient(settings);

builder.Services.AddSingleton<IElasticClient>(client);

await builder.Services.MappignIndexUsersElastic(client, elasticSettings);

// Registrar o contexto com MySQL
builder.Services.AddDbContext<SampleElasticContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddScoped<IElasticSearchService, ElasticSearchService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
