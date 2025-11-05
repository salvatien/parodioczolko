# Parodioczolko API - Backend

A .NET 8 Web API for the Parodioczolko song description game.

## Features

- RESTful API for song management
- Azure Cosmos DB integration with managed identity
- Mobile-optimized endpoints
- Swagger/OpenAPI documentation
- CORS configured for Angular frontend
- Health check endpoint

## API Endpoints

### Songs
- `GET /api/songs/random` - Get a random song for the game
- `GET /api/songs` - Get all songs (for administration)
- `GET /api/songs/{id}` - Get a specific song by ID

### Health
- `GET /health` - Health check endpoint

## Configuration

### Development (using Cosmos DB Emulator)
```json
{
  "CosmosDb": {
    "Endpoint": "https://localhost:8081",
    "Key": "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    "DatabaseName": "ParodioczolkoDb",
    "ContainerName": "Songs"
  }
}
```

### Production (using Azure Cosmos DB with Managed Identity)
```json
{
  "CosmosDb": {
    "Endpoint": "https://your-cosmosdb-account.documents.azure.com:443/",
    "DatabaseName": "ParodioczolkoDb",
    "ContainerName": "Songs"
  }
}
```

## Running the Application

### Prerequisites
- .NET 8 SDK
- Azure Cosmos DB Emulator (for local development) or Azure Cosmos DB account

### Local Development
1. Start the Cosmos DB Emulator
2. Run the application:
   ```bash
   dotnet run
   ```
3. Open Swagger UI at `https://localhost:7XXX` (port will be shown in console)

### Database Setup
The application expects a Cosmos DB database with:
- Database: `ParodioczolkoDb`
- Container: `Songs`
- Partition Key: `/partitionKey`

### Sample Song Document
```json
{
  "id": "song-1",
  "partitionKey": "song",
  "artist": "Artist Name",
  "name": "Song Title",
  "year": 2023
}
```

## Docker Support
To be implemented for containerized deployment to Azure App Service.

## Authentication
- Development: Uses Cosmos DB primary key
- Production: Uses Azure Managed Identity for secure, keyless authentication

## Logging
Configured with ASP.NET Core logging. In production, logs will be integrated with Azure Application Insights.