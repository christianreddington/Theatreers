using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Theatreers.Show
{
  public static class ChangeFeed
  {
    [FunctionName("ChangeFeed")]
    public static async void Run(
      [CosmosDBTrigger(
        databaseName: "theatreers",
        collectionName: "shows",
        ConnectionStringSetting = "cosmosConnectionString",
        LeaseCollectionName = "leases",
        LeaseCollectionPrefix = "local",
        CreateLeaseCollectionIfNotExists = true
      )]IReadOnlyList<Document> inputs,
      ILogger log,
      [CosmosDB(
        databaseName: "theatreers",
        collectionName: "showlist",
        ConnectionStringSetting = "cosmosConnectionString"
      )] IDocumentClient outputs
    )
    {
      string showName = "";
      CosmosBaseObject<ShowListObject> output = new CosmosBaseObject<ShowListObject>()
      {
        isDeleted = "false",
        doctype = "showlist"
      };
      try
      {
        if (inputs != null && inputs.Count > 0)
        {
          foreach (var show in inputs)
          {

            if (show.GetPropertyValue<string>("doctype") == "show" && (string)show.GetPropertyValue<string>("isDeleted") != "true")
            {
              string firstCharacter = show.GetPropertyValue<ShowObject>("innerobject").showName[0].ToString().ToUpper();
              int number;
              bool isNumber = int.TryParse(firstCharacter, out number);

              output.showId = show.GetPropertyValue<string>("showId");
              output.innerobject = new ShowListObject()
              {
                showName = show.GetPropertyValue<ShowObject>("innerobject").showName
              };
             // output.SetPropertyValue("id", show.GetPropertyValue<string>("showId"));
              //output.SetPropertyValue("showName", show.GetPropertyValue<ShowObject>("innerobject").showName);

              if (isNumber)
              {
                output.innerobject.partition = "0-9";
                //output.SetPropertyValue("partition", "0-9");
              }
              else
              {
                output.innerobject.partition = firstCharacter;
                //output.SetPropertyValue("partition", firstCharacter);
              }
              await outputs.UpsertDocumentAsync(
                UriFactory.CreateDocumentCollectionUri("theatreers", "showlist"),
                output);
              log.LogInformation($"[Reacting to Change Feed :: Creation of ShowListObject for {show.GetPropertyValue<string>("showName")} succeeded");
              output = new CosmosBaseObject<ShowListObject>()
              {
                isDeleted = "false",
                doctype = "showlist"
              };
            }
            else if (show.GetPropertyValue<string>("doctype") == "show" && show.GetPropertyValue<string>("isDeleted") == "true")
            {

              //Setup a connection to the Show and Show List collection to set TTL of related object
              Uri showCollectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");
              Uri showListCollectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "showlist");
              
              //Initialise variables appropriately
              string showId = show.GetPropertyValue<string>("showId");
              string firstCharacter = show.GetPropertyValue<ShowObject>("innerobject").showName[0].ToString().ToUpper();
              int number;
              bool isNumber = int.TryParse(firstCharacter, out number);
              string partitionKey;

              //Assign the appropriate partitionKey for showlist
              if (isNumber)
              {
                partitionKey = "0-9";
              }
              else
              {
                partitionKey = firstCharacter;
              }

              //Try and set the ttl of the showList object
              try
              {
                // Query the showList by the partitionKey, trying to find the same showID
                CosmosBaseObject<ShowListObject> result = outputs.CreateDocumentQuery<CosmosBaseObject<ShowListObject>>(showListCollectionUri, new FeedOptions { PartitionKey = new PartitionKey(partitionKey) })
                                            .Where(c => c.showId == showId)
                                            .AsEnumerable()
                                            .FirstOrDefault();

                // Set the ttl of 1 to that object
                result.ttl = 1;

                // Save it
                //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
                //If unsuccessful, catch any exception, log that and throw a BadRequestResult
                await outputs.UpsertDocumentAsync(showListCollectionUri, result);
                log.LogInformation($"Deleting the Show List listing successful for {showId}");
              }
              catch (Exception ex)
              {
                log.LogInformation($"Deleting the Show List listing failed for {showId} :: {ex.Message}");
              }

              List<CosmosBaseObject<dynamic>> showObjects = outputs.CreateDocumentQuery<CosmosBaseObject<dynamic>> (showCollectionUri, new FeedOptions { PartitionKey = new PartitionKey(showId) })
                                          .Where(c => c.showId == showId)
                                          .ToList();

              foreach (CosmosBaseObject<dynamic> showObject in showObjects){
                try
                {
                  // Set the ttl of 1 to that object
                  showObject.ttl = 1;
                  showObject.isDeleted = "false";

                  // Save it
                  //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
                  //If unsuccessful, catch any exception, log that and throw a BadRequestResult
                  await outputs.UpsertDocumentAsync(showCollectionUri, showObject);
                  log.LogInformation($"Deleting the item {showObject.id} for {showObject.showId} was successful");
                }
                catch (Exception ex)
                {
                  log.LogInformation($"Deleting the the item {showObject.id} for {showObject.showId} has failed :: {ex.Message}");
                }
              }

            }
          }
        }
      }
      catch (Exception ex)
      {
        log.LogInformation($"[Reacting to Change Feed failed :: {ex.Message}");
      }
    }
  }
}
