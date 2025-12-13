using Azure.Identity;
using Microsoft.Azure.Cosmos;
using ParodioczolkoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Parodioczolko API", Version = "v1" });
});

// Configure CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Azure Cosmos DB
builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var cosmosDbEndpoint = configuration["CosmosDb:Endpoint"];
    
    if (string.IsNullOrEmpty(cosmosDbEndpoint))
    {
        throw new InvalidOperationException("CosmosDb:Endpoint configuration is required");
    }

    // Use Managed Identity for authentication in Azure, local development key for local testing
    var cosmosClientOptions = new CosmosClientOptions
    {
        ApplicationName = "ParodioczolkoApi",
        ConnectionMode = ConnectionMode.Direct,
        ConsistencyLevel = ConsistencyLevel.Session
    };

    // In production, use Managed Identity. In development, use the primary key
    if (builder.Environment.IsProduction())
    {
        return new CosmosClient(cosmosDbEndpoint, new DefaultAzureCredential(), cosmosClientOptions);
    }
    else
    {
        var cosmosDbKey = configuration["CosmosDb:Key"];
        if (string.IsNullOrEmpty(cosmosDbKey))
        {
            throw new InvalidOperationException("CosmosDb:Key configuration is required for development");
        }
        return new CosmosClient(cosmosDbEndpoint, cosmosDbKey, cosmosClientOptions);
    }
});

// Register application services
builder.Services.AddScoped<ISongService, CosmosDbSongService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parodioczolko API v1");
        c.RoutePrefix = string.Empty; // Make Swagger UI the root page
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
