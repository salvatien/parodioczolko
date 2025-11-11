# Parodioczolko Infrastructure
# This Terraform configuration deploys:
# - Azure Functions for the .NET API (Consumption plan for cost optimization)
# - Static Web App for Angular frontend (Free tier)
# - Uses existing Cosmos DB account (data source)
# - Application Insights for monitoring
# - Storage Account for Functions runtime

terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.80"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.4"
    }
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
    key_vault {
      purge_soft_delete_on_destroy    = true
      recover_soft_deleted_key_vaults = true
    }
  }
}

# Local values for common configuration
locals {
  common_tags = {
    Environment = var.environment
    Project     = "Parodioczolko"
    ManagedBy   = "Terraform"
    CostCenter  = "Development"
  }
}

# Generate random suffix for unique resource names
resource "random_string" "suffix" {
  length  = 6
  special = false
  upper   = false
}

# Data source for current client configuration
data "azurerm_client_config" "current" {}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = "rg-parodioczolko-${var.environment}"
  location = var.location

  tags = local.common_tags
}

# Data source for existing Cosmos DB account
data "azurerm_cosmosdb_account" "existing" {
  count               = var.existing_cosmos_db_name != "" ? 1 : 0
  name                = var.existing_cosmos_db_name
  resource_group_name = var.existing_cosmos_db_resource_group
}

# Storage Account for Functions runtime
resource "azurerm_storage_account" "functions" {
  name                     = "stparodio${random_string.suffix.result}"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  
  # Security settings
  min_tls_version                 = "TLS1_2"
  allow_nested_items_to_be_public = false
  public_network_access_enabled   = true

  tags = local.common_tags
}

# Log Analytics Workspace for Application Insights
resource "azurerm_log_analytics_workspace" "loganalyticsworkspace" {
  name                = "law-parodioczolko-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = local.common_tags
}

# Application Insights for monitoring
resource "azurerm_application_insights" "appinsights" {
  name                = "appi-parodioczolko-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.loganalyticsworkspace.id

  tags = local.common_tags
}

# App Service Plan for Functions (Consumption plan)
resource "azurerm_service_plan" "functions" {
  name                = "asp-parodioczolko-${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  
  # Consumption plan (Y1) - still cost-effective and serverless
  os_type  = "Linux"
  sku_name = "Y1"

  tags = local.common_tags
}

# Function App
resource "azurerm_linux_function_app" "api" {
  name                = "func-parodioczolko-api-${var.environment}-${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  storage_account_name       = azurerm_storage_account.functions.name
  storage_account_access_key = azurerm_storage_account.functions.primary_access_key
  service_plan_id            = azurerm_service_plan.functions.id

  # Managed Identity for Cosmos DB access
  identity {
    type = "SystemAssigned"
  }

  site_config {
    cors {
      allowed_origins = [
        "https://${azurerm_static_web_app.frontend.default_host_name}",
        "https://portal.azure.com"
      ]
      support_credentials = false
    }
    
    application_insights_key               = azurerm_application_insights.appinsights.instrumentation_key
    application_insights_connection_string = azurerm_application_insights.appinsights.connection_string
  }

  app_settings = {
    "FUNCTIONS_WORKER_RUNTIME"       = "dotnet-isolated"
    
    # Application Insights
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.appinsights.instrumentation_key
    "APPLICATIONINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.appinsights.connection_string
    
    # Cosmos DB connection
    "CosmosDb__AccountEndpoint" = var.existing_cosmos_db_name != "" ? data.azurerm_cosmosdb_account.existing[0].endpoint : ""
    "CosmosDb__DatabaseName"    = "ParodioczolkoDb"
    "CosmosDb__ContainerName"   = "Songs"
  }

  tags = local.common_tags
}

# Static Web App for Angular frontend (Free tier)
resource "azurerm_static_web_app" "frontend" {
  name                = "stapp-parodioczolko-${var.environment}-${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location           = "West Europe" # Static Web Apps have limited region availability
  sku_tier           = "Free"
  sku_size           = "Free"

  tags = local.common_tags
}