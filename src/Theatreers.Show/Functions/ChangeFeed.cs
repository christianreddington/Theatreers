//using Microsoft.Azure.Documents;
//using Microsoft.Azure.Documents.Client;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Theatreers.Show.Models;

//namespace Theatreers.Show.Functions
//{
//  public static class ChangeFeed
//  {
//    [FunctionName("ChangeFeed")]
//    public static async void ChangeFeedAsync(
//      [CosmosDBTrigger(
//        databaseName: "theatreers",
//        collectionName: "shows",
//        ConnectionStringSetting = "cosmosConnectionString",
//        LeaseCollectionName = "leases",
//        LeaseCollectionPrefix = "local",
//        CreateLeaseCollectionIfNotExists = true
//      )]IReadOnlyList<Document> inputs,
//      ILogger log,
//      [CosmosDB(
//        databaseName: "theatreers",
//        collectionName: "showlist",
//        ConnectionStringSetting = "cosmosConnectionString"
//      )] IDocumentClient outputs
//    )
//    {
//      CosmosBaseObject<ShowListObject> output = new CosmosBaseObject<ShowListObject>()
//      {
//        IsDeleted = false,
//        Doctype = "showlist",
//        CreatedAt = DateTime.Now
//      };
//      try
//      {
//        if (inputs != null && inputs.Count > 0)
//        {
//          foreach (var show in inputs)
//          {

//            if (show.GetPropertyValue<string>("doctype") == "show" && show.GetPropertyValue<bool>("isDeleted") != true)
//            {
//              string firstCharacter = show.GetPropertyValue<ShowObject>("innerObject").ShowName[0].ToString().ToUpper();
//              int number;
//              bool isNumber = int.TryParse(firstCharacter, out number);

//              output.ShowId = show.GetPropertyValue<string>("showId");
//              output.InnerObject = new ShowListObject()
//              {
//                ShowName = show.GetPropertyValue<ShowObject>("innerObject").ShowName
//              };

//              if (isNumber)
//              {
//                output.InnerObject.Partition = "0-9";
//              }
//              else
//              {
//                output.InnerObject.Partition = firstCharacter;
//              }
//              await outputs.UpsertDocumentAsync(
//                UriFactory.CreateDocumentCollectionUri("theatreers", "showlist"),
//                output);
//              log.LogInformation($"[Reacting to Change Feed :: Creation of ShowListObject for {show.GetPropertyValue<string>("showName")} succeeded");
//              output = new CosmosBaseObject<ShowListObject>()
//              {
//                IsDeleted = false,
//                Doctype = "showlist",
//                CreatedAt = DateTime.Now
//              };
//            }
//            else if (show.GetPropertyValue<string>("doctype") == "show" && show.GetPropertyValue<bool>("isDeleted") == true)
//            {

//              //Setup a connection to the Show and Show List collection to set TTL of related object
//              Uri showCollectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");
//              Uri showListCollectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "showlist");
              
//              //Initialise variables appropriately
//              string showId = show.GetPropertyValue<string>("showId");
//              string firstCharacter = show.GetPropertyValue<ShowObject>("innerObject").ShowName[0].ToString().ToUpper();
//              int number;
//              bool isNumber = int.TryParse(firstCharacter, out number);
//              string partitionKey;

//              //Assign the appropriate partitionKey for showlist
//              if (isNumber)
//              {
//                partitionKey = "0-9";
//              }
//              else
//              {
//                partitionKey = firstCharacter;
//              }

//              //Try and set the ttl of the showList object
//              try
//              {
//                // Query the showList by the partitionKey, trying to find the same showID
//                CosmosBaseObject<ShowListObject> showListObject = outputs.CreateDocumentQuery<CosmosBaseObject<ShowListObject>>(showListCollectionUri, new FeedOptions { PartitionKey = new PartitionKey(partitionKey) })
//                                            .Where(c => c.ShowId == showId)
//                                            .AsEnumerable()
//                                            .FirstOrDefault();

//                // Set the ttl of 1 to that object
//                showListObject.Ttl = 1;
//                showListObject.IsDeleted = true;
//                showListObject.Doctype = "showlist";

//                // Save it
//                //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
//                //If unsuccessful, catch any exception, log that and throw a BadRequestResult
//                await outputs.UpsertDocumentAsync(showListCollectionUri, showListObject, new RequestOptions {PartitionKey = new PartitionKey(partitionKey)});
//                log.LogInformation($"Deleting the Show List listing successful for {showId}");
//              }
//              catch (Exception ex)
//              {
//                log.LogInformation($"Deleting the Show List listing failed for {showId} :: {ex.Message}");
//              }

//              List<CosmosBaseObject<dynamic>> showObjects = outputs.CreateDocumentQuery<CosmosBaseObject<dynamic>> (showCollectionUri, new FeedOptions { PartitionKey = new PartitionKey(showId) })
//                                          .Where(c => c.ShowId == showId)
//                                          .ToList();

//              foreach (CosmosBaseObject<dynamic> showObject in showObjects){
//                try
//                {
//                  // Set the ttl of 1 to that object
//                  showObject.Ttl = 1;
//                  showObject.IsDeleted = true;

//                  // Save it
//                  //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
//                  //If unsuccessful, catch any exception, log that and throw a BadRequestResult
//                  await outputs.UpsertDocumentAsync(showCollectionUri, showObject);
//                  log.LogInformation($"Deleting the item {showObject.Id} for {showObject.ShowId} was successful");
//                }
//                catch (Exception ex)
//                {
//                  log.LogInformation($"Deleting the the item {showObject.Id} for {showObject.ShowId} has failed :: {ex.Message}");
//                }
//              }

//            }
//          }
//        }
//      }
//      catch (Exception ex)
//      {
//        log.LogInformation($"[Reacting to Change Feed failed :: {ex.Message}");
//      }
//    }
//  }
// }
