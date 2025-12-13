using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ParodioczolkoApi.Models;
using ParodioczolkoApi.Services;
using System.Net;

namespace ParodioczolkoApi.Tests.Services;

public class CosmosDbSongServiceTests
{
    private readonly Mock<CosmosClient> _mockCosmosClient;
    private readonly Mock<Container> _mockContainer;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<CosmosDbSongService>> _mockLogger;
    private readonly CosmosDbSongService _service;

    public CosmosDbSongServiceTests()
    {
        _mockCosmosClient = new Mock<CosmosClient>();
        _mockContainer = new Mock<Container>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<CosmosDbSongService>>();

        // Setup configuration
        _mockConfiguration.Setup(c => c["CosmosDb:DatabaseName"]).Returns("ParodioczolkoDb");
        _mockConfiguration.Setup(c => c["CosmosDb:ContainerName"]).Returns("Songs");

        // Setup CosmosClient to return our mock container
        _mockCosmosClient.Setup(c => c.GetContainer("ParodioczolkoDb", "Songs"))
                        .Returns(_mockContainer.Object);

        _service = new CosmosDbSongService(_mockCosmosClient.Object, _mockConfiguration.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetRandomSongAsync_WhenSongsExist_ReturnsSong()
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

        var mockCountIterator = new Mock<FeedIterator<int>>();
        mockCountIterator.Setup(i => i.HasMoreResults).Returns(true);
        mockCountIterator.Setup(i => i.ReadNextAsync(default))
                        .ReturnsAsync(CreateMockFeedResponse(new[] { 5 })); // 5 songs total

        var mockSongIterator = new Mock<FeedIterator<Song>>();
        mockSongIterator.Setup(i => i.ReadNextAsync(default))
                       .ReturnsAsync(CreateMockFeedResponse(new[] { expectedSong }));

        _mockContainer.Setup(c => c.GetItemQueryIterator<int>(
                            It.Is<QueryDefinition>(q => q.QueryText.Contains("COUNT")),
                            null, null))
                     .Returns(mockCountIterator.Object);

        _mockContainer.Setup(c => c.GetItemQueryIterator<Song>(
                            It.Is<QueryDefinition>(q => q.QueryText.Contains("ORDER BY") && q.QueryText.Contains("OFFSET")),
                            null, null))
                     .Returns(mockSongIterator.Object);

        // Act
        var result = await _service.GetRandomSongAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSong.Id, result.Id);
        Assert.Equal(expectedSong.Artist, result.Artist);
        Assert.Equal(expectedSong.Name, result.Name);
        Assert.Equal(expectedSong.Year, result.Year);
    }

    [Fact]
    public async Task GetRandomSongAsync_WhenNoSongsExist_ReturnsNull()
    {
        // Arrange
        var mockCountIterator = new Mock<FeedIterator<int>>();
        mockCountIterator.Setup(i => i.HasMoreResults).Returns(true);
        mockCountIterator.Setup(i => i.ReadNextAsync(default))
                        .ReturnsAsync(CreateMockFeedResponse(new[] { 0 })); // 0 songs

        var mockSongIterator = new Mock<FeedIterator<Song>>();
        mockSongIterator.Setup(i => i.ReadNextAsync(default))
                       .ReturnsAsync(CreateMockFeedResponse(Array.Empty<Song>()));

        _mockContainer.Setup(c => c.GetItemQueryIterator<int>(
                            It.Is<QueryDefinition>(q => q.QueryText.Contains("COUNT")),
                            null, null))
                     .Returns(mockCountIterator.Object);

        _mockContainer.Setup(c => c.GetItemQueryIterator<Song>(
                            It.Is<QueryDefinition>(q => q.QueryText.Contains("ORDER BY") && q.QueryText.Contains("OFFSET")),
                            null, null))
                     .Returns(mockSongIterator.Object);

        // Act
        var result = await _service.GetRandomSongAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetRandomSongAsync_WhenExceptionOccurs_ReturnsNull()
    {
        // Arrange
        _mockContainer.Setup(c => c.GetItemQueryIterator<int>(
                            It.IsAny<QueryDefinition>(),
                            null, null))
                     .Throws(new CosmosException("Test exception", HttpStatusCode.InternalServerError, 0, "", 0));

        // Act
        var result = await _service.GetRandomSongAsync();

        // Assert
        Assert.Null(result);
        
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
    public async Task GetAllSongsAsync_WhenSongsExist_ReturnsAllSongs()
    {
        // Arrange
        var expectedSongs = new[]
        {
            new Song { Id = "song-1", Artist = "Artist 1", Name = "Song 1", Year = 2020 },
            new Song { Id = "song-2", Artist = "Artist 2", Name = "Song 2", Year = 2021 },
            new Song { Id = "song-3", Artist = "Artist 3", Name = "Song 3", Year = 2022 }
        };

        var mockIterator = new Mock<FeedIterator<Song>>();
        mockIterator.SetupSequence(i => i.HasMoreResults)
                   .Returns(true)
                   .Returns(false);
        
        mockIterator.Setup(i => i.ReadNextAsync(default))
                   .ReturnsAsync(CreateMockFeedResponse(expectedSongs));

        _mockContainer.Setup(c => c.GetItemQueryIterator<Song>(
                            It.Is<QueryDefinition>(q => q.QueryText == "SELECT * FROM c"),
                            null, null))
                     .Returns(mockIterator.Object);

        // Act
        var result = await _service.GetAllSongsAsync();

        // Assert
        Assert.NotNull(result);
        var songsList = result.ToList();
        Assert.Equal(3, songsList.Count);
        Assert.Equal("song-1", songsList[0].Id);
        Assert.Equal("song-2", songsList[1].Id);
        Assert.Equal("song-3", songsList[2].Id);
    }

    [Fact]
    public async Task GetAllSongsAsync_WhenExceptionOccurs_ReturnsEmptyCollection()
    {
        // Arrange
        _mockContainer.Setup(c => c.GetItemQueryIterator<Song>(
                            It.IsAny<QueryDefinition>(),
                            null, null))
                     .Throws(new CosmosException("Test exception", HttpStatusCode.InternalServerError, 0, "", 0));

        // Act
        var result = await _service.GetAllSongsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        
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
    public async Task GetSongByIdAsync_WhenSongExists_ReturnsSong()
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

        var mockResponse = new Mock<ItemResponse<Song>>();
        mockResponse.Setup(r => r.Resource).Returns(expectedSong);

        _mockContainer.Setup(c => c.ReadItemAsync<Song>("song-1", new PartitionKey("song"), null, default))
                     .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await _service.GetSongByIdAsync("song-1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSong.Id, result.Id);
        Assert.Equal(expectedSong.Artist, result.Artist);
        Assert.Equal(expectedSong.Name, result.Name);
        Assert.Equal(expectedSong.Year, result.Year);
    }

    [Fact]
    public async Task GetSongByIdAsync_WhenSongNotFound_ReturnsNull()
    {
        // Arrange
        _mockContainer.Setup(c => c.ReadItemAsync<Song>("nonexistent", new PartitionKey("song"), null, default))
                     .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, "", 0));

        // Act
        var result = await _service.GetSongByIdAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetSongByIdAsync_WhenOtherExceptionOccurs_ReturnsNull()
    {
        // Arrange
        _mockContainer.Setup(c => c.ReadItemAsync<Song>("song-1", new PartitionKey("song"), null, default))
                     .ThrowsAsync(new CosmosException("Server error", HttpStatusCode.InternalServerError, 0, "", 0));

        // Act
        var result = await _service.GetSongByIdAsync("song-1");

        // Assert
        Assert.Null(result);
        
        // Verify that error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error getting song by id: song-1")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetSongByIdAsync_WhenIdIsNullOrEmpty_ReturnsNull(string? id)
    {
        // Arrange
        if (id == null)
        {
            // Setup mock to throw ArgumentNullException when null ID is passed
            _mockContainer.Setup(c => c.ReadItemAsync<Song>(null!, new PartitionKey("song"), null, default))
                         .ThrowsAsync(new ArgumentNullException("id"));
        }
        else
        {
            // Setup mock to throw ArgumentException for empty/whitespace strings
            _mockContainer.Setup(c => c.ReadItemAsync<Song>(id, new PartitionKey("song"), null, default))
                         .ThrowsAsync(new ArgumentException("Invalid id"));
        }

        // Act
        var result = await _service.GetSongByIdAsync(id!);

        // Assert
        Assert.Null(result);
        
        // Verify that error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Error getting song by id: {id}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static FeedResponse<T> CreateMockFeedResponse<T>(IEnumerable<T> items)
    {
        var mockResponse = new Mock<FeedResponse<T>>();
        mockResponse.Setup(r => r.GetEnumerator()).Returns(items.GetEnumerator());
        mockResponse.As<IEnumerable<T>>().Setup(r => r.GetEnumerator()).Returns(items.GetEnumerator());
        return mockResponse.Object;
    }
}