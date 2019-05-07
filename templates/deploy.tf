# Configure the Azure Provider
provider "azurerm" { }

# Set appropriate variables
data "template_file" "data_id" {
  count = "${length(var.locations)}"
  template = "${lookup(var.locations[count.index], "abbreviation")}"
}

data "template_file" "data_location" {
  count = "${length(var.locations)}"
  template = "${lookup(var.locations[count.index], "location")}"
}

# Create a resource group

resource "azurerm_resource_group" "coreResourceGroup" {
  count = "${length(var.locations)}"
  name = "${var.organisationalPrefix}-${element(data.template_file.data_id.*.rendered, count.index)}-rg"
  location = "${element(data.template_file.data_location.*.rendered, count.index)}"
  tags = {
    "environment" = "prod"
  }
}

resource "azurerm_app_service_plan" "websiteFrontendFarm" {  
  count = "${length(var.locations)}"
  name = "${var.organisationalPrefix}-${element(data.template_file.data_id.*.rendered, count.index)}-plan" 
  location = "${element(data.template_file.data_location.*.rendered, count.index)}"
  resource_group_name = "${var.organisationalPrefix}-${element(data.template_file.data_id.*.rendered, count.index)}-rg"
  sku = {
    tier = "F1"
    size = "0"
    capacity = "1"
  }
}