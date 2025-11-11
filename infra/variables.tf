# Input variables for the Terraform configuration

variable "environment" {
  description = "Environment name (dev or prod)"
  type        = string
  default     = "dev"
  
  validation {
    condition     = contains(["dev", "prod"], var.environment)
    error_message = "Environment must be one of: dev, prod."
  }
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "West Europe"
  
  validation {
    condition = contains([
      "West Europe", "North Europe", "East US", "East US 2", 
      "West US", "West US 2", "Central US", "South Central US"
    ], var.location)
    error_message = "Location must be a valid Azure region."
  }
}

variable "project_name" {
  description = "Name of the project"
  type        = string
  default     = "parodioczolko"
  
  validation {
    condition     = length(var.project_name) <= 20 && can(regex("^[a-z][a-z0-9]*$", var.project_name))
    error_message = "Project name must be lowercase, start with a letter, and be max 20 characters."
  }
}
