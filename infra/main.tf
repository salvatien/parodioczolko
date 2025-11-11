# Parodioczolko Infrastructure - Frontend Only
# This Terraform configuration deploys:
# - Static Web App for Angular frontend (Free tier)
# - Simplified for frontend-only architecture

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

# Static Web App for Angular frontend (Free tier)
resource "azurerm_static_web_app" "frontend" {
  name                = "stapp-parodioczolko-${var.environment}-${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location           = "West Europe" # Static Web Apps have limited region availability
  sku_tier           = "Free"
  sku_size           = "Free"

  tags = local.common_tags
}