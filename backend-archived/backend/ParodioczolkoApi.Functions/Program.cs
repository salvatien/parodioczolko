using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ParodioczolkoApi.Functions.Services;
using Microsoft.Azure.Cosmos;
using Azure.Identity;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Configure Cosmos DB
        var cosmosEndpoint = configuration["CosmosDb__AccountEndpoint"];
        var databaseName = configuration["CosmosDb__DatabaseName"] ?? "ParodioczolkoDb";
        
        services.AddSingleton<CosmosClient>(serviceProvider =>
        {
            // Use connection string first (for both emulator and Azure)
            var connectionString = configuration["CosmosDb__ConnectionString"];
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                return new CosmosClient(connectionString);
            }
            else if (!string.IsNullOrEmpty(cosmosEndpoint))
            {
                // Use Managed Identity for Azure Cosmos DB
                return new CosmosClient(cosmosEndpoint, new DefaultAzureCredential());
            }
            else
            {
                // Fallback to emulator for local development
                return new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            }
        });

        services.AddScoped<ISongService>(serviceProvider =>
        {
            var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
            var logger = serviceProvider.GetRequiredService<ILogger<CosmosDbSongService>>();
            
            return new CosmosDbSongService(cosmosClient, configuration, logger);
        });
    })
    .Build();

host.Run();