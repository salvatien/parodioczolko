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
            var autoConfirm = args.Length > 1 && args[1].ToLower() == "--auto-confirm";
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

            // Validate database name contains "Parodioczolko" for safety
            if (!databaseName.Contains("Parodioczolko", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"❌ Error: Database name '{databaseName}' must contain 'Parodioczolko' for safety.");
                Console.WriteLine("This prevents accidentally seeding the wrong database.");
                Environment.Exit(1);
            }

            // Initialize seeder
            using var seeder = new CosmosDbSeeder(endpoint, key, databaseName, containerName, useEmulator);

            // Confirm before proceeding (unless auto-confirm is enabled)
            if (!useEmulator && !autoConfirm)
            {
                Console.Write("⚠️  You are about to seed non-emulator database. Continue? (y/N): ");
                var confirm = Console.ReadLine();
                if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("❌ Operation cancelled.");
                    return;
                }
            }
            else if (!useEmulator && autoConfirm)
            {
                Console.WriteLine("⚠️  Auto-confirm enabled. Proceeding with non-emulator database seeding...");
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

        // Only wait for key press if running interactively (not in CI/CD)
        if (Environment.UserInteractive && !args.Any(a => a.ToLower() == "--auto-confirm"))
        {
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
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
