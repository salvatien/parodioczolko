# 🎵 Parodioczolko - Frontend-Only Deployment Guide

This guide helps you deploy the Parodioczolko song guessing game as a frontend-only application with static data.

## 🏗️ Architecture Overview - Frontend Only

- **Frontend**: Angular 20 app hosted on Azure Static Web Apps (Free tier)
- **Data**: Static JSON files bundled with the application
- **Infrastructure**: Simplified Terraform for Static Web App only
- **CI/CD**: GitHub Actions for automated frontend deployment

## 💰 Cost Estimation

- **Monthly Cost**: $0 (Free tier only)
- **Static Web Apps**: Free tier (perfect for small applications)
- **No backend costs**: Eliminated Functions, Cosmos DB, and monitoring costs

## 🚀 Architecture Decision

**Why Frontend-Only?**
- **Cost Effective**: Zero monthly costs vs $5-15/month for full backend
- **Simplicity**: No RU/s limits, no database management
- **Performance**: Data bundled with app, no API calls needed
- **Scalability**: Perfect for small user bases (< 100 users)
- **Reliability**: No backend dependencies

## 🎯 What Changed

### From Full-Stack to Frontend-Only

**Removed:**
- Azure Functions (.NET 8 API)
- Azure Cosmos DB 
- Application Insights monitoring
- Storage Account for Functions
- Complex environment configurations

**Kept:**
- Angular 20 frontend
- Azure Static Web Apps hosting
- GitHub Actions deployment
- All 50 songs (now in `songs.json`)

## 🔧 Setup Instructions

### 1. Prerequisites

- Azure subscription
- GitHub repository
- Node.js and Angular CLI for local development

### 2. Local Development

```bash
# Navigate to frontend directory
cd frontend

# Install dependencies
npm install

# Start development server
npm start

# Build for production
npm run build
```

### 3. Data Management

**Song Data Location**: `frontend/src/assets/songs.json`

**Adding New Songs**: 
- Edit the JSON file directly
- Future enhancement: GitHub API-based song management UI

### 4. Azure Deployment

**Option A: Terraform (Recommended)**

```bash
# Navigate to infrastructure directory
cd infra

# Initialize Terraform
terraform init

# Plan deployment
terraform plan

# Deploy Static Web App
terraform apply
```

**Option B: Manual Azure Portal**
- Create Azure Static Web App
- Connect to GitHub repository
- Configure build settings for Angular

### 5. GitHub Actions (Automatic)

The repository includes GitHub Actions that automatically:
- Build the Angular application
- Deploy to Azure Static Web Apps
- Run on every push to main branch

## 📁 Project Structure

```
parodioczolko/
├── frontend/                    # Angular 20 application
│   ├── src/assets/songs.json   # Static song data (50 songs)
│   ├── src/app/services/       # Song service (static data)
│   └── ...
├── infra/                      # Simplified Terraform (Static Web App only)
├── backend-archived/           # Archived backend code (for reference)
└── .github/workflows/         # CI/CD pipelines
```

## 🎮 Game Features

- **50+ Songs**: Curated collection from multiple genres and decades
- **Mobile Optimized**: Perfect for phone and tablet gameplay
- **Offline Ready**: All data bundled with the app
- **Fast Loading**: No API calls, immediate song access

## 🔄 Future Enhancements

1. **GitHub API Integration**: Add UI for song management
2. **Progressive Web App**: Offline support and installation
3. **User Preferences**: Local storage for settings
4. **Leaderboards**: Client-side scoring system

## 🛠️ Troubleshooting

**Build Issues:**
- Ensure TypeScript supports JSON imports
- Check `tsconfig.json` has `resolveJsonModule: true`

**Deployment Issues:**
- Verify GitHub Actions secrets are configured
- Check Static Web App build configuration

**Data Issues:**
- Validate JSON format in `songs.json`
- Ensure all song objects have required fields: `id`, `artist`, `name`, `year`

## 📊 Benefits Achieved

✅ **Zero monthly costs** (vs $5-15/month with backend)
✅ **Simplified architecture** (no database management)  
✅ **Better performance** (no API latency)
✅ **Easier maintenance** (frontend-only updates)
✅ **Perfect scalability** for intended use case (5 users)

---

**Note**: The archived backend code remains available in `backend-archived/` for reference or future use if scaling requirements change.