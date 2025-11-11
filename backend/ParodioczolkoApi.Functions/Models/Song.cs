using Newtonsoft.Json;

namespace ParodioczolkoApi.Functions.Models;

public class Song
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("partitionKey")]
    public string PartitionKey { get; set; } = "song";

    [JsonProperty("artist")]
    public string Artist { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("year")]
    public int Year { get; set; }

    [JsonProperty("_etag")]
    public string? ETag { get; set; }
}