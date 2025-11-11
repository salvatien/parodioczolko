# Backend Archive

This directory contains the archived backend code from the original full-stack architecture.

## What's Archived

- **ParodioczolkoApi**: .NET 8 Web API (original implementation)
- **ParodioczolkoApi.Functions**: .NET 8 Azure Functions (final backend implementation)
- **ParodioczolkoApi.Seeder**: Cosmos DB seeding tool with 50+ songs
- **ParodioczolkoApi.Tests**: Unit tests for the API

## Why Archived

The project was converted from full-stack (backend + frontend) to frontend-only architecture to:

- Eliminate monthly costs (from $6-17/month to $0)
- Remove Cosmos DB RU/s limitations (1000 RU/s constraint)
- Simplify deployment and maintenance
- Perfect fit for small user base (5 users)

## Architecture Change

**Before (Full-Stack):**
- .NET 8 Azure Functions API
- Azure Cosmos DB database
- Application Insights monitoring
- Complex CI/CD with multiple environments

**After (Frontend-Only):**
- Angular 20 with static JSON data
- Azure Static Web Apps hosting (free)
- Single deployment workflow
- All song data bundled with application

## Song Data Migration

The 50+ songs from `ParodioczolkoApi.Seeder/CosmosDbSeeder.cs` were migrated to `frontend/src/assets/songs.json` to maintain all functionality while eliminating backend dependencies.

## Future Use

This archived code remains available if:
- User base grows significantly (>100 users)
- Real-time features become necessary
- Advanced analytics are required
- Dynamic song management is needed

The backend infrastructure was fully functional and can be restored if requirements change.

---
**Archived on:** November 11, 2025
**Reason:** Architecture simplification for cost optimization