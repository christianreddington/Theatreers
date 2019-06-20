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
    public static class ChangeFeed
    {
        [FunctionName("ChangeFeed")]
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
            string showName = "";
            try
            {
                if (inputs != null && inputs.Count > 0)
                {
                    foreach (var show in inputs)
                    {
                        if (show.GetPropertyValue<string>("doctype") == "show" && show.GetPropertyValue<string>("isDeleted") == "false")
                        {
                            string firstCharacter = show.GetPropertyValue<string>("showName")[0].ToString().ToUpper();
                            int number;
                            bool isNumber = int.TryParse(firstCharacter, out number);
                            show.SetPropertyValue("id", show.GetPropertyValue<string>("showId"));
                            show.SetPropertyValue("showName", show.GetPropertyValue<string>("showName"));

                            if (isNumber)
                            {
                                show.SetPropertyValue("partition", "0-9");
                            }
                            else
                            {
                                show.SetPropertyValue("partition", firstCharacter);
                            }
                            showName = show.GetPropertyValue<string>("showName");
                            await outputs.UpsertDocumentAsync(
                              UriFactory.CreateDocumentCollectionUri("theatreers", "showListObjects"),
                              show);
                            log.LogInformation($"[Reacting to Change Feed :: Creation of ShowListObject for {show.GetPropertyValue<string>("showName")} succeeded");
                        }
                        else if (show.GetPropertyValue<string>("doctype") == "show" && show.GetPropertyValue<string>("isDeleted") == "false")
                        {
                            Uri showListCollectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "showListObjects");
                            ShowListObject result = outputs.CreateDocumentQuery<ShowListObject>(showListCollectionUri)
                                                        .Where(c => c.showId == show.GetPropertyValue<string>("showId"))
                                                        .FirstOrDefault();

                            // Set the ttl of 1 to that object
                            result.ttl = "1";

                            // Save it
                            //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
                            //If unsuccessful, catch any exception, log that and throw a BadRequestResult
                            try
                            {
                                await outputs.UpsertDocumentAsync(showListCollectionUri, result);
                                log.LogInformation($"Deleting the Show List listing successful for {result.showId}");
                            }
                            catch (Exception ex)
                            {
                                log.LogInformation($"Deleting the Show List listing failed for {result.showId} :: {ex.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation($"[Reacting to Change Feed :: Creation of ShowListObject for {showName} failed :: {ex.Message}");
            }
        }
    }
}
