using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Theatreers.Show
{
    public static class CreateShowListObject
    {
        [FunctionName("CreateShowListObject")]
        public static async void Run([CosmosDBTrigger(
            databaseName: "theatreers",
            collectionName: "shows",
            ConnectionStringSetting = "cosmosConnectionString",
            LeaseCollectionName = "leases", 
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> inputs, ILogger log,
            [CosmosDB(
                databaseName: "theatreers",
                collectionName: "showListObjects",
                ConnectionStringSetting = "cosmosConnectionString"
            )] IDocumentClient outputs
        )
        {
            try
            {
                if (inputs != null && inputs.Count > 0)
                {
                    foreach (var show in inputs)
                    {
                        if (show.GetPropertyValue<string>("doctype") == "show")
                        {
                            string firstCharacter = show.GetPropertyValue<string>("showName")[0].ToString().ToUpper();
                            int number;
                            bool isNumber = int.TryParse(firstCharacter, out number);
                            show.SetPropertyValue("id", show.GetPropertyValue<string>("showId"));
                            show.SetPropertyValue("showName", show.GetPropertyValue<string>("showName"));

                            if (isNumber)
                            {
                                show.SetPropertyValue("partition", "0-9");
                            } else
                            {
                                show.SetPropertyValue("partition", firstCharacter);
                            }
                            await outputs.UpsertDocumentAsync(
                              UriFactory.CreateDocumentCollectionUri("theatreers", "showListObjects"),
                              show);
                            log.LogInformation($"Reacting to change feed... Creating a Show List Objectsucceeded");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation($"Reacting to change feed... Creating a Show List Object failed :: {ex.Message}");
            }
        }
    }
}
