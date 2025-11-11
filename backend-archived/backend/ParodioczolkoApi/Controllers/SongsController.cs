using Microsoft.AspNetCore.Mvc;
using ParodioczolkoApi.Models;
using ParodioczolkoApi.Services;

namespace ParodioczolkoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongsController : ControllerBase
{
    private readonly ISongService _songService;
    private readonly ILogger<SongsController> _logger;

    public SongsController(ISongService songService, ILogger<SongsController> logger)
    {
        _songService = songService;
        _logger = logger;
    }

    /// <summary>
    /// Get a random song for the game
    /// </summary>
    /// <returns>A random song</returns>
    [HttpGet("random")]
    public async Task<ActionResult<Song>> GetRandomSong()
    {
        try
        {
            var song = await _songService.GetRandomSongAsync();
            
            if (song == null)
            {
                return NotFound("No songs available");
            }

            return Ok(song);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting random song");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get all songs
    /// </summary>
    /// <returns>List of all songs</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Song>>> GetAllSongs()
    {
        try
        {
            var songs = await _songService.GetAllSongsAsync();
            return Ok(songs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all songs");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get a specific song by ID
    /// </summary>
    /// <param name="id">Song ID</param>
    /// <returns>The requested song</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Song>> GetSong(string id)
    {
        try
        {
            var song = await _songService.GetSongByIdAsync(id);
            
            if (song == null)
            {
                return NotFound($"Song with ID {id} not found");
            }

            return Ok(song);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting song with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}