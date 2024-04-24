using System.Text.Json.Serialization;
using Claims.Auditing;
using Claims.Proxy;
using Claims.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
);

builder.Services.AddTransient<ClaimsService>();
builder.Services.AddTransient<CoversService>();

builder.Services.AddSingleton(
    InitializeCosmosClientInstanceForCoversAsync(builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());

builder.Services.AddSingleton(
    InitializeCosmosClientInstanceAsync(builder.Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());

builder.Services.AddDbContext<AuditContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

static async Task<ClaimsProxy> InitializeCosmosClientInstanceAsync(IConfiguration configurationSection)
{
    var databaseName = configurationSection.GetSection("DatabaseName").Value;
    var containerName = configurationSection.GetSection("ContainerName").Value;
    var account = configurationSection.GetSection("Account").Value;
    var key = configurationSection.GetSection("Key").Value;

    var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
    var cosmosDbService = new ClaimsProxy(client, databaseName, containerName);
    var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);

    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return cosmosDbService;
}

static async Task<CoversProxy> InitializeCosmosClientInstanceForCoversAsync(IConfiguration configurationSection)
{
    var databaseName = configurationSection.GetSection("DatabaseName").Value;
    var containerName = configurationSection.GetSection("CoverContainerName").Value;
    var account = configurationSection.GetSection("Account").Value;
    var key = configurationSection.GetSection("Key").Value;

    var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
    var cosmosDbService = new CoversProxy(client);
    var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);

    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

    return cosmosDbService;
}

public partial class Program { }