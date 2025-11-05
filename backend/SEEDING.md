# 🎵 Database Seeding Guide

This guide will help you populate your Cosmos DB database with sample songs for the Parodioczolko game.

## 🚀 Quick Start

### Option 1: Cosmos DB Emulator (Recommended for Development)

1. **Start Cosmos DB Emulator**:
   - Install from: https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator
   - Start the emulator (it runs on https://localhost:8081)

2. **Run the seeder**:
   ```powershell
   cd backend/ParodioczolkoApi.Seeder
   dotnet run emulator
   ```

### Option 2: Azure Cosmos DB (Production)

1. **Set environment variables**:
   ```powershell
   $env:COSMOS_ENDPOINT = "https://your-cosmos-account.documents.azure.com:443/"
   $env:COSMOS_KEY = "your-cosmos-db-primary-key"
   ```

2. **Run the seeder**:
   ```powershell
   cd backend/ParodioczolkoApi.Seeder
   dotnet run production
   ```

## 📊 What Gets Seeded

The seeder creates:
- **Database**: `ParodioczolkoDb`
- **Container**: `Songs` (with `/partitionKey` partition key)
- **Sample Data**: 50 diverse songs across multiple genres:
  - Classic Rock (Queen, Led Zeppelin, The Beatles, Pink Floyd)
  - 80s Hits (Michael Jackson, Prince, Madonna)
  - 90s Alternative (Nirvana, Pearl Jam, Radiohead)
  - 2000s Pop/Rock (Coldplay, The White Stripes, Green Day)
  - 2010s Hits (Adele, Bruno Mars, Ed Sheeran)
  - Hip-Hop (Eminem, Jay-Z, Kanye West, Drake)
  - Electronic/Dance (Daft Punk, Avicii, Calvin Harris)
  - And many more classics!

## 🎮 Testing Your Setup

After seeding, you can test the API endpoints:

1. **Start the API**:
   ```powershell
   cd backend/ParodioczolkoApi
   dotnet run
   ```

2. **Test endpoints**:
   - Get all songs: `GET https://localhost:7247/api/songs`
   - Get random song: `GET https://localhost:7247/api/songs/random`
   - Get specific song: `GET https://localhost:7247/api/songs/{id}`

3. **View in browser**: https://localhost:7247/swagger

## 🔍 Troubleshooting

### Cosmos DB Emulator Issues
- Ensure the emulator is running and accessible at https://localhost:8081
- Check Windows Firewall isn't blocking the connection
- Try restarting the emulator

### Azure Cosmos DB Issues
- Verify your endpoint URL is correct
- Check your access key is valid
- Ensure your Azure subscription is active
- Verify network connectivity to Azure

### General Issues
- Ensure .NET 8 SDK is installed: `dotnet --version`
- Check project builds successfully: `dotnet build`
- Review error messages in the console output

## 📝 Notes

- The seeder automatically creates the database and container if they don't exist
- Existing songs with the same ID will be updated
- The seeder supports both key-based and managed identity authentication
- All songs include realistic data perfect for the game

Happy gaming! 🎵🎮