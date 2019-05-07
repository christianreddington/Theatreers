variable "organisationalPrefix" {
  type = "string"
  default = "mts"
  description = "Organisational prefix used for Azure Resource deployments"
}

variable "locations" {
  type = "list"
  default = [
    {
      abbreviation = "neu"
      location = "North Europe"
    },
    {
      abbreviation = "weu"
      location = "West Europe"
    }
  ]
}