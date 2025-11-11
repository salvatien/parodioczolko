using ParodioczolkoApi.Seeder;

class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("🎵 Parodioczolko Database Seeder");
        Console.WriteLine("================================");

        try
        {
            // Parse command line arguments
            var useEmulator = args.Length > 0 && args[0].ToLower() == "emulator";
            var endpoint = GetEndpoint(useEmulator);
            var key = GetKey(useEmulator);
            
            // Get database and container names from environment variables or use defaults
            var databaseName = Environment.GetEnvironmentVariable("COSMOS_DATABASE_NAME") ?? "ParodioczolkoDb";
            var containerName = Environment.GetEnvironmentVariable("COSMOS_CONTAINER_NAME") ?? "Songs";

            Console.WriteLine($"📍 Target: {(useEmulator ? "Cosmos DB Emulator" : "Azure Cosmos DB")}");
            Console.WriteLine($"🌐 Endpoint: {endpoint}");
            Console.WriteLine($"📊 Database: {databaseName}");
            Console.WriteLine($"📦 Container: {containerName}");
            Console.WriteLine();

            // Initialize seeder
            using var seeder = new CosmosDbSeeder(endpoint, key, databaseName, containerName, useEmulator);

            // Confirm before proceeding
            if (!useEmulator)
            {
                Console.Write("⚠️  You are about to seed PRODUCTION database. Continue? (y/N): ");
                var confirm = Console.ReadLine();
                if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("❌ Operation cancelled.");
                    return;
                }
            }

            // Run seeding
            await seeder.SeedDataAsync();

            Console.WriteLine("\n🎉 Database seeding completed successfully!");
            Console.WriteLine("Your Parodioczolko game is ready to play! 🎮");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
            Console.WriteLine("Please check your configuration and try again.");
            Environment.Exit(1);
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static string GetEndpoint(bool useEmulator)
    {
        if (useEmulator)
        {
            return "https://localhost:8081";
        }

        var endpoint = Environment.GetEnvironmentVariable("COSMOS_ENDPOINT");
        if (string.IsNullOrEmpty(endpoint))
        {
            Console.WriteLine("❌ Please set COSMOS_ENDPOINT environment variable for production use.");
            Console.WriteLine("Example: https://your-cosmos-account.documents.azure.com:443/");
            Environment.Exit(1);
        }
        return endpoint;
    }

    private static string GetKey(bool useEmulator)
    {
        if (useEmulator)
        {
            // Default Cosmos DB Emulator key
            return "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        }

        var key = Environment.GetEnvironmentVariable("COSMOS_KEY");
        // In production, we can use either key or managed identity (key can be null for managed identity)
        return key ?? string.Empty;
    }
}
