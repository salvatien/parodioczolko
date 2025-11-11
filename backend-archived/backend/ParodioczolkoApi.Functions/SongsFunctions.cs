using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ParodioczolkoApi.Functions.Services;
using System.Net;
using System.Text.Json;

namespace ParodioczolkoApi.Functions;

public class SongsFunctions
{
    private readonly ILogger _logger;
    private readonly ISongService _songService;

    public SongsFunctions(ILoggerFactory loggerFactory, ISongService songService)
    {
        _logger = loggerFactory.CreateLogger<SongsFunctions>();
        _songService = songService;
    }

    [Function("GetAllSongs")]
    public async Task<HttpResponseData> GetAllSongs(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "songs")] HttpRequestData req)
    {
        _logger.LogInformation("Getting all songs");

        try
        {
            var songs = await _songService.GetAllSongsAsync();
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            
            // Add CORS headers
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

            await response.WriteStringAsync(JsonSerializer.Serialize(songs, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all songs");
            
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            errorResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
            errorResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            
            await errorResponse.WriteStringAsync(JsonSerializer.Serialize(new { error = "Internal server error" }));
            return errorResponse;
        }
    }

    [Function("GetRandomSong")]
    public async Task<HttpResponseData> GetRandomSong(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "songs/random")] HttpRequestData req)
    {
        _logger.LogInformation("Getting random song");

        try
        {
            var song = await _songService.GetRandomSongAsync();
            
            if (song == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                notFoundResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
                notFoundResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                
                await notFoundResponse.WriteStringAsync(JsonSerializer.Serialize(new { error = "No songs found" }));
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

            await response.WriteStringAsync(JsonSerializer.Serialize(song, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting random song");
            
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            errorResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
            errorResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            
            await errorResponse.WriteStringAsync(JsonSerializer.Serialize(new { error = "Internal server error" }));
            return errorResponse;
        }
    }

    [Function("GetSongById")]
    public async Task<HttpResponseData> GetSongById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "songs/{id}")] HttpRequestData req,
        string id)
    {
        _logger.LogInformation("Getting song by ID: {SongId}", id);

        try
        {
            var song = await _songService.GetSongByIdAsync(id);
            
            if (song == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                notFoundResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
                notFoundResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                
                await notFoundResponse.WriteStringAsync(JsonSerializer.Serialize(new { error = "Song not found" }));
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

            await response.WriteStringAsync(JsonSerializer.Serialize(song, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting song by ID: {SongId}", id);
            
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            errorResponse.Headers.Add("Content-Type", "application/json; charset=utf-8");
            errorResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            
            await errorResponse.WriteStringAsync(JsonSerializer.Serialize(new { error = "Internal server error" }));
            return errorResponse;
        }
    }

    [Function("OptionsHandler")]
    public HttpResponseData OptionsHandler(
        [HttpTrigger(AuthorizationLevel.Anonymous, "options", Route = "{*path}")] HttpRequestData req)
    {
        _logger.LogInformation("Handling OPTIONS request for CORS");

        var response = req.CreateResponse(HttpStatusCode.OK);
        
        // CORS headers for preflight requests
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, x-requested-with");
        response.Headers.Add("Access-Control-Max-Age", "86400");

        return response;
    }
}