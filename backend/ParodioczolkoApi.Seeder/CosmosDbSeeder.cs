using Microsoft.Azure.Cosmos;
using Azure.Identity;
using ParodioczolkoApi.Models;

namespace ParodioczolkoApi.Seeder;

public class CosmosDbSeeder : IDisposable
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;
    private readonly string _containerName;

    public CosmosDbSeeder(string endpoint, string key, string databaseName, string containerName, bool useEmulator = false)
    {
        _databaseName = databaseName;
        _containerName = containerName;

        var cosmosClientOptions = new CosmosClientOptions
        {
            ApplicationName = "ParodioczolkoDataSeeder",
            ConnectionMode = ConnectionMode.Direct,
            ConsistencyLevel = ConsistencyLevel.Session
        };

        if (useEmulator || !string.IsNullOrEmpty(key))
        {
            // Use key-based authentication (emulator or production with key)
            _cosmosClient = new CosmosClient(endpoint, key, cosmosClientOptions);
        }
        else
        {
            // Use managed identity for production
            _cosmosClient = new CosmosClient(endpoint, new DefaultAzureCredential(), cosmosClientOptions);
        }
    }

    public async Task SeedDataAsync()
    {
        try
        {
            Console.WriteLine("🎵 Starting Parodioczolko song data seeding...");

            // Create database if it doesn't exist
            Console.WriteLine($"📊 Creating database '{_databaseName}' if it doesn't exist...");
            var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);
            Console.WriteLine($"✅ Database ready: {database.Database.Id}");

            // Create container if it doesn't exist
            Console.WriteLine($"📦 Creating container '{_containerName}' if it doesn't exist...");
            var containerProperties = new ContainerProperties(_containerName, "/partitionKey");
            var container = await database.Database.CreateContainerIfNotExistsAsync(containerProperties);
            Console.WriteLine($"✅ Container ready: {container.Container.Id}");

            // Get sample songs
            var songs = GetSampleSongs();
            Console.WriteLine($"🎶 Prepared {songs.Count} songs for seeding");

            // Seed songs
            var successCount = 0;
            var skipCount = 0;

            foreach (var song in songs)
            {
                try
                {
                    await container.Container.CreateItemAsync(song, new PartitionKey(song.PartitionKey));
                    Console.WriteLine($"✅ Added: {song.Artist} - {song.Name} ({song.Year})");
                    successCount++;
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"⏭️  Skipped (already exists): {song.Artist} - {song.Name}");
                    skipCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to add {song.Artist} - {song.Name}: {ex.Message}");
                }
            }

            Console.WriteLine($"\n🎉 Seeding completed!");
            Console.WriteLine($"✅ Successfully added: {successCount} songs");
            Console.WriteLine($"⏭️  Skipped (already existed): {skipCount} songs");
            Console.WriteLine($"🎵 Total songs in database: {successCount + skipCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error during seeding: {ex.Message}");
            throw;
        }
    }

    private List<Song> GetSampleSongs()
    {
        return new List<Song>
        {
            // Classic Rock
            new Song 
            { 
                Id = "f47ac10b-58cc-4372-a567-0e02b2c3d479", 
                Artist = "Queen", 
                Name = "Bohemian Rhapsody", 
                Year = 1975
            },
            new Song 
            { 
                Id = "6ba7b810-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Led Zeppelin", 
                Name = "Stairway to Heaven", 
                Year = 1971
            },
            new Song 
            { 
                Id = "6ba7b811-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "The Beatles", 
                Name = "Hey Jude", 
                Year = 1968
            },
            new Song 
            { 
                Id = "6ba7b812-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Pink Floyd", 
                Name = "Wish You Were Here", 
                Year = 1975
            },
            new Song 
            { 
                Id = "6ba7b813-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "The Rolling Stones", 
                Name = "Paint It Black", 
                Year = 1966
            },

            // 80s Hits
            new Song 
            { 
                Id = "6ba7b814-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Michael Jackson", 
                Name = "Billie Jean", 
                Year = 1982
            },
            new Song 
            { 
                Id = "6ba7b815-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Prince", 
                Name = "Purple Rain", 
                Year = 1984
            },
            new Song 
            { 
                Id = "6ba7b816-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Madonna", 
                Name = "Like a Virgin", 
                Year = 1984
            },
            new Song 
            { 
                Id = "6ba7b817-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Duran Duran", 
                Name = "Hungry Like the Wolf", 
                Year = 1982
            },
            new Song 
            { 
                Id = "6ba7b818-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Cyndi Lauper", 
                Name = "Time After Time", 
                Year = 1983
            },

            // 90s Alternative
            new Song 
            { 
                Id = "6ba7b819-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Nirvana", 
                Name = "Smells Like Teen Spirit", 
                Year = 1991
            },
            new Song 
            { 
                Id = "6ba7b81a-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Pearl Jam", 
                Name = "Alive", 
                Year = 1991
            },
            new Song 
            { 
                Id = "6ba7b81b-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Radiohead", 
                Name = "Creep", 
                Year = 1992
            },
            new Song 
            { 
                Id = "6ba7b81c-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Red Hot Chili Peppers", 
                Name = "Under the Bridge", 
                Year = 1991
            },
            new Song 
            { 
                Id = "6ba7b81d-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "R.E.M.", 
                Name = "Losing My Religion", 
                Year = 1991
            },

            // 2000s Pop/Rock
            new Song 
            { 
                Id = "6ba7b81e-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Coldplay", 
                Name = "Yellow", 
                Year = 2000
            },
            new Song 
            { 
                Id = "6ba7b81f-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "The White Stripes", 
                Name = "Seven Nation Army", 
                Year = 2003
            },
            new Song 
            { 
                Id = "6ba7b820-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Outkast", 
                Name = "Hey Ya!", 
                Year = 2003
            },
            new Song 
            { 
                Id = "6ba7b821-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Green Day", 
                Name = "Boulevard of Broken Dreams", 
                Year = 2004
            },
            new Song 
            { 
                Id = "6ba7b822-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "The Killers", 
                Name = "Mr. Brightside", 
                Year = 2003
            },

            // 2010s Hits
            new Song 
            { 
                Id = "6ba7b823-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Adele", 
                Name = "Rolling in the Deep", 
                Year = 2010
            },
            new Song 
            { 
                Id = "6ba7b824-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Bruno Mars", 
                Name = "Uptown Funk", 
                Year = 2014
            },
            new Song 
            { 
                Id = "6ba7b825-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Ed Sheeran", 
                Name = "Shape of You", 
                Year = 2017
            },
            new Song 
            { 
                Id = "6ba7b826-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Pharrell Williams", 
                Name = "Happy", 
                Year = 2013
            },
            new Song 
            { 
                Id = "6ba7b827-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Imagine Dragons", 
                Name = "Radioactive", 
                Year = 2012
            },

            // Hip-Hop Classics
            new Song 
            { 
                Id = "6ba7b828-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Eminem", 
                Name = "Lose Yourself", 
                Year = 2002
            },
            new Song 
            { 
                Id = "6ba7b829-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Jay-Z", 
                Name = "99 Problems", 
                Year = 2003
            },
            new Song 
            { 
                Id = "6ba7b82a-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Kanye West", 
                Name = "Stronger", 
                Year = 2007
            },
            new Song 
            { 
                Id = "6ba7b82b-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Drake", 
                Name = "God's Plan", 
                Year = 2018
            },
            new Song 
            { 
                Id = "6ba7b82c-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Kendrick Lamar", 
                Name = "HUMBLE.", 
                Year = 2017
            },

            // Dance/Electronic
            new Song 
            { 
                Id = "6ba7b82d-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Daft Punk", 
                Name = "Get Lucky", 
                Year = 2013
            },
            new Song 
            { 
                Id = "6ba7b82e-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Avicii", 
                Name = "Wake Me Up", 
                Year = 2013
            },
            new Song 
            { 
                Id = "6ba7b82f-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Calvin Harris", 
                Name = "Feel So Close", 
                Year = 2011
            },
            new Song 
            { 
                Id = "6ba7b830-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Swedish House Mafia", 
                Name = "Don't You Worry Child", 
                Year = 2012
            },
            new Song 
            { 
                Id = "6ba7b831-9dad-11d1-80b4-00c04fd430c8", 
                Artist = "Deadmau5", 
                Name = "Ghosts 'n' Stuff", 
                Year = 2008
            },

            // Additional hits to reach 50 songs
            new Song { Id = "6ba7b832-9dad-11d1-80b4-00c04fd430c8", Artist = "Johnny Cash", Name = "Ring of Fire", Year = 1963 },
            new Song { Id = "6ba7b833-9dad-11d1-80b4-00c04fd430c8", Artist = "Dolly Parton", Name = "Jolene", Year = 1973 },
            new Song { Id = "6ba7b834-9dad-11d1-80b4-00c04fd430c8", Artist = "Taylor Swift", Name = "Love Story", Year = 2008 },
            new Song { Id = "6ba7b835-9dad-11d1-80b4-00c04fd430c8", Artist = "Aretha Franklin", Name = "Respect", Year = 1967 },
            new Song { Id = "6ba7b836-9dad-11d1-80b4-00c04fd430c8", Artist = "Whitney Houston", Name = "I Will Always Love You", Year = 1992 },
            new Song { Id = "6ba7b837-9dad-11d1-80b4-00c04fd430c8", Artist = "ABBA", Name = "Dancing Queen", Year = 1976 },
            new Song { Id = "6ba7b838-9dad-11d1-80b4-00c04fd430c8", Artist = "Bob Marley", Name = "No Woman No Cry", Year = 1974 },
            new Song { Id = "6ba7b839-9dad-11d1-80b4-00c04fd430c8", Artist = "AC/DC", Name = "Back in Black", Year = 1980 },
            new Song { Id = "6ba7b83a-9dad-11d1-80b4-00c04fd430c8", Artist = "Guns N' Roses", Name = "Sweet Child O' Mine", Year = 1987 },
            new Song { Id = "6ba7b83b-9dad-11d1-80b4-00c04fd430c8", Artist = "Metallica", Name = "Enter Sandman", Year = 1991 },
            new Song { Id = "6ba7b83c-9dad-11d1-80b4-00c04fd430c8", Artist = "Fleetwood Mac", Name = "Go Your Own Way", Year = 1977 },
            new Song { Id = "6ba7b83d-9dad-11d1-80b4-00c04fd430c8", Artist = "Elvis Presley", Name = "Can't Help Falling in Love", Year = 1961 },
            new Song { Id = "6ba7b83e-9dad-11d1-80b4-00c04fd430c8", Artist = "The Eagles", Name = "Hotel California", Year = 1976 },
            new Song { Id = "6ba7b83f-9dad-11d1-80b4-00c04fd430c8", Artist = "Journey", Name = "Don't Stop Believin'", Year = 1981 },
            new Song { Id = "6ba7b840-9dad-11d1-80b4-00c04fd430c8", Artist = "Bon Jovi", Name = "Livin' on a Prayer", Year = 1986 }
        };
    }

    public void Dispose()
    {
        _cosmosClient?.Dispose();
    }
}