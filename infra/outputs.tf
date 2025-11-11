# Output values for the deployed resources - Frontend Only

output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "static_web_app_name" {
  description = "Name of the Static Web App"
  value       = azurerm_static_web_app.frontend.name
}

output "static_web_app_url" {
  description = "URL of the Static Web App"
  value       = "https://${azurerm_static_web_app.frontend.default_host_name}"
}

output "deployment_summary" {
  description = "Summary of deployed resources"
  value = {
    resource_group = azurerm_resource_group.main.name
    frontend_url   = "https://${azurerm_static_web_app.frontend.default_host_name}"
    location       = azurerm_resource_group.main.location
    environment    = var.environment
    architecture   = "Frontend-Only (Static JSON data)"
  }
}