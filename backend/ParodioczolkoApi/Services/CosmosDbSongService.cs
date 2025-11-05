using Azure.Identity;
using Microsoft.Azure.Cosmos;
using ParodioczolkoApi.Models;

namespace ParodioczolkoApi.Services;

public interface ISongService
{
    Task<Song?> GetRandomSongAsync();
    Task<IEnumerable<Song>> GetAllSongsAsync();
    Task<Song?> GetSongByIdAsync(string id);
}

public class CosmosDbSongService : ISongService
{
    private readonly Container _container;
    private readonly ILogger<CosmosDbSongService> _logger;

    public CosmosDbSongService(CosmosClient cosmosClient, IConfiguration configuration, ILogger<CosmosDbSongService> logger)
    {
        var databaseName = configuration["CosmosDb:DatabaseName"] ?? "ParodioczolkoDb";
        var containerName = configuration["CosmosDb:ContainerName"] ?? "Songs";
        
        _container = cosmosClient.GetContainer(databaseName, containerName);
        _logger = logger;
    }

    public async Task<Song?> GetRandomSongAsync()
    {
        try
        {
            var query = new QueryDefinition("SELECT * FROM c ORDER BY c._ts OFFSET @offset LIMIT 1")
                .WithParameter("@offset", Random.Shared.Next(0, await GetSongCountAsync()));

            var iterator = _container.GetItemQueryIterator<Song>(query);
            var response = await iterator.ReadNextAsync();

            return response.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting random song");
            return null;
        }
    }

    public async Task<IEnumerable<Song>> GetAllSongsAsync()
    {
        try
        {
            var query = new QueryDefinition("SELECT * FROM c");
            var iterator = _container.GetItemQueryIterator<Song>(query);
            var songs = new List<Song>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                songs.AddRange(response);
            }

            return songs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all songs");
            return Enumerable.Empty<Song>();
        }
    }

    public async Task<Song?> GetSongByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Song>(id, new PartitionKey("song"));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting song by id: {Id}", id);
            return null;
        }
    }

    private async Task<int> GetSongCountAsync()
    {
        try
        {
            var query = new QueryDefinition("SELECT VALUE COUNT(1) FROM c");
            var iterator = _container.GetItemQueryIterator<int>(query);
            var response = await iterator.ReadNextAsync();
            return response.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting song count");
            return 0;
        }
    }
}