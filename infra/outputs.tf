# Output values for the deployed resources

output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "function_app_name" {
  description = "Name of the Function App"
  value       = azurerm_linux_function_app.api.name
}

output "function_app_url" {
  description = "URL of the Function App"
  value       = "https://${azurerm_linux_function_app.api.name}.azurewebsites.net"
}

output "static_web_app_name" {
  description = "Name of the Static Web App"
  value       = azurerm_static_web_app.frontend.name
}

output "static_web_app_url" {
  description = "URL of the Static Web App"
  value       = "https://${azurerm_static_web_app.frontend.default_host_name}"
}

output "cosmos_db_account_name" {
  description = "Name of the Cosmos DB account"
  value       = var.existing_cosmos_db_name != "" ? var.existing_cosmos_db_name : "No Cosmos DB configured"
}

output "cosmos_db_endpoint" {
  description = "Endpoint of the Cosmos DB account"
  value       = var.existing_cosmos_db_name != "" ? data.azurerm_cosmosdb_account.existing[0].endpoint : "No Cosmos DB configured"
}

output "application_insights_name" {
  description = "Name of Application Insights"
  value       = azurerm_application_insights.appinsights.name
}

output "application_insights_instrumentation_key" {
  description = "Application Insights instrumentation key"
  value       = azurerm_application_insights.appinsights.instrumentation_key
  sensitive   = true
}

output "deployment_summary" {
  description = "Summary of deployed resources"
  value = {
    resource_group    = azurerm_resource_group.main.name
    function_app_url  = "https://${azurerm_linux_function_app.api.name}.azurewebsites.net"
    frontend_url      = "https://${azurerm_static_web_app.frontend.default_host_name}"
    cosmos_db         = var.existing_cosmos_db_name != "" ? var.existing_cosmos_db_name : "No Cosmos DB configured"
    location          = azurerm_resource_group.main.location
    environment       = var.environment
  }
}