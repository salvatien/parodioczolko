# Terraform backend configuration for remote state management
# This file configures remote state storage in Azure Storage Account
# Comment out the backend block for local development

# terraform {
#   backend "azurerm" {
#     # Configuration will be provided via CLI parameters in GitHub Actions
#     # resource_group_name  = "rg-terraform-state"
#     # storage_account_name = "saterraformstate<suffix>"
#     # container_name      = "tfstate"
#     # key                 = "parodioczolko-dev.tfstate"
#   }
# }

# Note: The backend configuration values are provided via GitHub Actions secrets:
# - TERRAFORM_STORAGE_RG: Resource group containing the storage account
# - TERRAFORM_STORAGE_ACCOUNT: Storage account name for state files
# 
# These values are passed as CLI arguments in the GitHub workflow:
# terraform init \
#   -backend-config="resource_group_name=${{ secrets.TERRAFORM_STORAGE_RG }}" \
#   -backend-config="storage_account_name=${{ secrets.TERRAFORM_STORAGE_ACCOUNT }}" \
#   -backend-config="container_name=tfstate" \
#   -backend-config="key=parodioczolko-${{ env.ENVIRONMENT }}.tfstate"