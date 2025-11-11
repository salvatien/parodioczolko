# 🎵 Parodioczolko - CI/CD Setup Guide

This guide helps you set up the complete CI/CD pipeline for the Parodioczolko song guessing game using Terraform and GitHub Actions.

## 🏗️ Architecture Overview

- **Frontend**: Angular 20 app hosted on Azure Static Web Apps (Free tier)
- **Backend**: .NET 8 Azure Functions (Consumption plan - essentially free)
- **Database**: Azure Cosmos DB (Free tier - 1000 RU/s, 25 GB storage)
- **Monitoring**: Application Insights (Free tier)
- **Infrastructure**: Terraform for Infrastructure as Code
- **CI/CD**: GitHub Actions for automated deployment

## 💰 Cost Estimation

- **Monthly Cost**: ~$0-2 (Free tiers + minimal consumption billing)
- **Azure Functions**: Free for up to 1M requests/month
- **Static Web Apps**: Free tier
- **Cosmos DB**: Free tier (first 1000 RU/s and 25 GB)
- **Application Insights**: Free tier (first 1 GB/month)

## 🚀 Setup Instructions

### 1. Prerequisites

- Azure subscription
- GitHub repository
- Terraform CLI installed locally (optional, runs in GitHub Actions)

### 2. Azure Service Principal Setup

Create a service principal for GitHub Actions:

```bash
az ad sp create-for-rbac --name "github-actions-parodioczolko" \
  --role contributor \
  --scopes /subscriptions/YOUR_SUBSCRIPTION_ID \
  --sdk-auth
```

This will output JSON that you'll use in GitHub Secrets.

### 3. Terraform State Storage

Create a storage account for Terraform state:

```bash
# Create resource group for Terraform state
az group create --name rg-terraform-state --location "West Europe"

# Create storage account (replace <unique-suffix> with random characters)
az storage account create \
  --resource-group rg-terraform-state \
  --name saterraformstate<unique-suffix> \
  --sku Standard_LRS \
  --encryption-services blob

# Create container for state files
az storage container create \
  --name tfstate \
  --account-name saterraformstate<unique-suffix>
```

### 4. GitHub Secrets Configuration

Add these secrets to your GitHub repository (Settings → Secrets → Actions):

```
AZURE_CLIENT_ID=<from service principal JSON>
AZURE_CLIENT_SECRET=<from service principal JSON>
AZURE_SUBSCRIPTION_ID=<your subscription ID>
AZURE_TENANT_ID=<from service principal JSON>
TERRAFORM_STORAGE_RG=rg-terraform-state
TERRAFORM_STORAGE_ACCOUNT=saterraformstate<unique-suffix>
```

### 5. Static Web App Deployment Token

After the infrastructure is deployed, you'll need to get the Static Web App deployment token:

```bash
# Get the deployment token (run after infrastructure deployment)
az staticwebapp secrets list --name <your-static-web-app-name> --query "properties.apiKey" -o tsv
```

Add this as a GitHub secret:
```
AZURE_STATIC_WEB_APPS_API_TOKEN=<deployment-token>
```

## 📋 Deployment Process

### Option 1: Automatic Deployment

1. Push code to `main` branch
2. Infrastructure workflow runs automatically if `/infra` files changed
3. Application workflow runs automatically if `/backend` or `/frontend` files changed

### Option 2: Manual Deployment

1. Go to Actions tab in GitHub
2. Run "Deploy Infrastructure" workflow first
3. Run "Deploy Application" workflow after infrastructure is ready

### Deployment Steps:

1. **Infrastructure Deployment** (`infrastructure.yml`):
   - Validates and applies Terraform configuration
   - Creates Azure resources with free tier settings
   - Outputs resource names for application deployment

2. **Application Deployment** (`application.yml`):
   - Builds and tests .NET Functions API
   - Builds and tests Angular frontend
   - Deploys API to Azure Functions
   - Deploys frontend to Static Web Apps
   - Seeds database with sample songs

## 🔧 Local Development

### Backend (Azure Functions)

```bash
cd backend/ParodioczolkoApi.Functions
func start
```

The API will be available at `http://localhost:7071/api`

### Frontend (Angular)

```bash
cd frontend
npm install
npm start
```

The frontend will be available at `http://localhost:4200`

### Database Seeding

```bash
cd backend/ParodioczolkoApi.Seeder
dotnet run emulator
```

## 📂 Project Structure

```
parodioczolko/
├── .github/
│   └── workflows/
│       ├── infrastructure.yml    # Infrastructure deployment
│       └── application.yml       # Application deployment
├── backend/
│   ├── ParodioczolkoApi/         # Original Web API (for reference)
│   ├── ParodioczolkoApi.Functions/ # Azure Functions API
│   ├── ParodioczolkoApi.Seeder/  # Database seeding tool
│   └── ParodioczolkoApi.Tests/   # Unit tests
├── frontend/                      # Angular 20 application
├── infra/                        # Terraform infrastructure code
│   ├── main.tf                   # Main infrastructure configuration
│   ├── variables.tf              # Input variables
│   ├── outputs.tf                # Output values
│   ├── terraform.tfvars          # Default variable values
│   └── backend.tf                # Remote state configuration
└── README.md
```

## 🌍 Environments

The pipeline supports multiple environments:

- **dev**: Development environment (default)
- **staging**: Staging environment
- **prod**: Production environment

Each environment gets its own:
- Resource group: `rg-parodioczolko-{env}`
- Function app: `func-parodioczolko-api-{env}-{suffix}`
- Static web app: `stapp-parodioczolko-{env}-{suffix}`
- Cosmos DB: `cosmos-parodioczolko-{env}-{suffix}`

## 🔍 Monitoring

- **Application Insights**: Monitor function performance and errors
- **Azure Monitor**: Resource health and metrics
- **GitHub Actions**: CI/CD pipeline status and logs

## 🛠️ Troubleshooting

### Common Issues:

1. **Terraform State Conflicts**: Ensure only one deployment runs at a time
2. **Resource Name Conflicts**: Random suffixes should prevent this
3. **CORS Issues**: Check Static Web App URL in Function App CORS settings
4. **Cosmos DB Free Tier**: Only one free tier per subscription

### Useful Commands:

```bash
# Check infrastructure status
terraform plan -var="environment=dev"

# Validate Terraform configuration
terraform validate

# Check Azure resources
az resource list --resource-group rg-parodioczolko-dev

# View function logs
az monitor log-analytics query \
  --workspace <workspace-id> \
  --analytics-query "FunctionAppLogs | limit 50"
```

## 🎮 Using the Application

After successful deployment:

1. Visit the Static Web App URL (shown in deployment summary)
2. Play the song guessing game
3. API endpoints are available at the Function App URL

### API Endpoints:

- `GET /api/songs` - Get all songs
- `GET /api/songs/random` - Get a random song
- `GET /api/songs/{id}` - Get song by ID

## 📝 Notes

- The setup uses Azure free tiers to minimize costs
- All resources are deployed in West Europe region
- Database is automatically seeded with 50 sample songs
- HTTPS is enforced across all services
- Managed Identity is used for secure resource access

Happy gaming! 🎵🎮