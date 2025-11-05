using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ParodioczolkoApi.Controllers;
using ParodioczolkoApi.Models;
using ParodioczolkoApi.Services;

namespace ParodioczolkoApi.Tests.Controllers;

public class SongsControllerTests
{
    private readonly Mock<ISongService> _mockSongService;
    private readonly Mock<ILogger<SongsController>> _mockLogger;
    private readonly SongsController _controller;

    public SongsControllerTests()
    {
        _mockSongService = new Mock<ISongService>();
        _mockLogger = new Mock<ILogger<SongsController>>();
        _controller = new SongsController(_mockSongService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetRandomSong_WhenSongExists_ReturnsOkWithSong()
    {
        // Arrange
        var expectedSong = new Song
        {
            Id = "song-1",
            PartitionKey = "song",
            Artist = "Test Artist",
            Name = "Test Song",
            Year = 2023
        };

        _mockSongService.Setup(s => s.GetRandomSongAsync())
                       .ReturnsAsync(expectedSong);

        // Act
        var result = await _controller.GetRandomSong();

        // Assert
        var actionResult = Assert.IsType<ActionResult<Song>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedSong = Assert.IsType<Song>(okResult.Value);
        
        Assert.Equal(expectedSong.Id, returnedSong.Id);
        Assert.Equal(expectedSong.Artist, returnedSong.Artist);
        Assert.Equal(expectedSong.Name, returnedSong.Name);
        Assert.Equal(expectedSong.Year, returnedSong.Year);
    }

    [Fact]
    public async Task GetRandomSong_WhenNoSongExists_ReturnsNotFound()
    {
        // Arrange
        _mockSongService.Setup(s => s.GetRandomSongAsync())
                       .ReturnsAsync((Song?)null);

        // Act
        var result = await _controller.GetRandomSong();

        // Assert
        var actionResult = Assert.IsType<ActionResult<Song>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal("No songs available", notFoundResult.Value);
    }

    [Fact]
    public async Task GetRandomSong_WhenExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        _mockSongService.Setup(s => s.GetRandomSongAsync())
                       .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetRandomSong();

        // Assert
        var actionResult = Assert.IsType<ActionResult<Song>>(result);
        var statusCodeResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error", statusCodeResult.Value);

        // Verify that error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error getting random song")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllSongs_WhenSongsExist_ReturnsOkWithSongs()
    {
        // Arrange
        var expectedSongs = new List<Song>
        {
            new() { Id = "song-1", Artist = "Artist 1", Name = "Song 1", Year = 2020 },
            new() { Id = "song-2", Artist = "Artist 2", Name = "Song 2", Year = 2021 },
            new() { Id = "song-3", Artist = "Artist 3", Name = "Song 3", Year = 2022 }
        };

        _mockSongService.Setup(s => s.GetAllSongsAsync())
                       .ReturnsAsync(expectedSongs);

        // Act
        var result = await _controller.GetAllSongs();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Song>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedSongs = Assert.IsAssignableFrom<IEnumerable<Song>>(okResult.Value);
        
        var songsList = returnedSongs.ToList();
        Assert.Equal(3, songsList.Count);
        Assert.Equal("song-1", songsList[0].Id);
        Assert.Equal("song-2", songsList[1].Id);
        Assert.Equal("song-3", songsList[2].Id);
    }

    [Fact]
    public async Task GetAllSongs_WhenNoSongsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockSongService.Setup(s => s.GetAllSongsAsync())
                       .ReturnsAsync(new List<Song>());

        // Act
        var result = await _controller.GetAllSongs();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Song>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedSongs = Assert.IsAssignableFrom<IEnumerable<Song>>(okResult.Value);
        Assert.Empty(returnedSongs);
    }

    [Fact]
    public async Task GetAllSongs_WhenExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        _mockSongService.Setup(s => s.GetAllSongsAsync())
                       .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllSongs();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Song>>>(result);
        var statusCodeResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error", statusCodeResult.Value);

        // Verify that error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error getting all songs")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetSong_WhenSongExists_ReturnsOkWithSong()
    {
        // Arrange
        const string songId = "song-1";
        var expectedSong = new Song
        {
            Id = songId,
            PartitionKey = "song",
            Artist = "Test Artist",
            Name = "Test Song",
            Year = 2023
        };

        _mockSongService.Setup(s => s.GetSongByIdAsync(songId))
                       .ReturnsAsync(expectedSong);

        // Act
        var result = await _controller.GetSong(songId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Song>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedSong = Assert.IsType<Song>(okResult.Value);
        
        Assert.Equal(expectedSong.Id, returnedSong.Id);
        Assert.Equal(expectedSong.Artist, returnedSong.Artist);
        Assert.Equal(expectedSong.Name, returnedSong.Name);
        Assert.Equal(expectedSong.Year, returnedSong.Year);
    }

    [Fact]
    public async Task GetSong_WhenSongNotFound_ReturnsNotFound()
    {
        // Arrange
        const string songId = "nonexistent";
        _mockSongService.Setup(s => s.GetSongByIdAsync(songId))
                       .ReturnsAsync((Song?)null);

        // Act
        var result = await _controller.GetSong(songId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Song>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal($"Song with ID {songId} not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetSong_WhenExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        const string songId = "song-1";
        _mockSongService.Setup(s => s.GetSongByIdAsync(songId))
                       .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetSong(songId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Song>>(result);
        var statusCodeResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("Internal server error", statusCodeResult.Value);

        // Verify that error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Error getting song with ID: {songId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetSong_WhenIdIsEmptyOrWhitespace_StillCallsService(string songId)
    {
        // Arrange
        _mockSongService.Setup(s => s.GetSongByIdAsync(songId))
                       .ReturnsAsync((Song?)null);

        // Act
        var result = await _controller.GetSong(songId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Song>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal($"Song with ID {songId} not found", notFoundResult.Value);
        
        // Verify service was called
        _mockSongService.Verify(s => s.GetSongByIdAsync(songId), Times.Once);
    }
}