using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theatreers.Core.Abstractions;
using Theatreers.Core.Models;
using Theatreers.Show.Models;

namespace Theatreers.Show.Functions
{
  public static class ChangeFeed
  {
    [FunctionName("ChangeFeed")]
    public static async Task ChangeFeedAsync(
      [CosmosDBTrigger(
        databaseName: "theatreers",
        collectionName: "shows",
        ConnectionStringSetting = "cosmosConnectionString",
        LeaseCollectionName = "leases",
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
      ShowListObject output = new ShowListObject()
      {
        IsDeleted = false,
        Doctype = "showlist",
        CreatedAt = DateTime.Now
      };
      try
      {
        if (inputs != null && inputs.Count > 0)
        {
          foreach (var show in inputs)
          {

            if (show.GetPropertyValue<string>("doctype") == DocTypes.Show && show.GetPropertyValue<int>("ttl") < 0)
            {
              string firstCharacter = show.GetPropertyValue<string>("showName")[0].ToString().ToUpper();
              int number;
              bool isNumber = int.TryParse(firstCharacter, out number);

              output.Id = show.GetPropertyValue<string>("id");
              output.ShowName = show.GetPropertyValue<string>("showName");

              if (isNumber)
              {
                output.Partition = "0-9";
              }
              else
              {
                output.Partition = firstCharacter;
              }
              await outputs.UpsertDocumentAsync(
                UriFactory.CreateDocumentCollectionUri("theatreers", "showlist"),
                output);
              log.LogInformation($"[Reacting to Change Feed :: Creation of ShowListObject for {show.GetPropertyValue<string>("showName")} succeeded");
              output = new ShowListObject()
              {
                Doctype = "showlist",
                CreatedAt = DateTime.Now
              };
            }
            else if (show.GetPropertyValue<string>("doctype") == DocTypes.Show && show.GetPropertyValue<int>("ttl") > 0)
            {

              //Setup a connection to the Show and Show List collection to set TTL of related object
              Uri showCollectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");
              Uri showListCollectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "showlist");

              //Initialise variables appropriately
              string showId = show.GetPropertyValue<string>("id");
              string firstCharacter = show.GetPropertyValue<string>("showName")[0].ToString().ToUpper();
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
                ShowListObject showListObject = outputs.CreateDocumentQuery<ShowListObject>(showListCollectionUri, new FeedOptions { PartitionKey = new PartitionKey(partitionKey) })
                                            .Where(c => c.Id == showId)
                                            .AsEnumerable()
                                            .FirstOrDefault();

                // Set the ttl of 1 to that object
                showListObject.Ttl = 1;
                showListObject.Doctype = DocTypes.ShowList;

                // Save it
                //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
                //If unsuccessful, catch any exception, log that and throw a BadRequestResult
                await outputs.UpsertDocumentAsync(showListCollectionUri, showListObject, new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });
                log.LogInformation($"Deleting the Show List listing successful for {showId}");
              }
              catch (Exception ex)
              {
                log.LogInformation($"Deleting the Show List listing failed for {showId} :: {ex.Message}");
              }

              List <ShowDomainObject> showObjects = outputs.CreateDocumentQuery<ShowDomainObject>(showCollectionUri, new FeedOptions { PartitionKey = new PartitionKey(showId) })
                                          .Where(c => c.Partition == showId)
                                          .ToList();

              foreach (dynamic showObject in showObjects)
              {
                try
                {
                  // Set the ttl of 1 to that object
                  showObject.Ttl = 1;
                  showObject.IsDeleted = true;

                  // Save it
                  //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
                  //If unsuccessful, catch any exception, log that and throw a BadRequestResult
                  await outputs.UpsertDocumentAsync(showCollectionUri, showObject);
                  log.LogInformation($"Deleting the item {showObject.Id} for {showObject.Partition} was successful");
                }
                catch (Exception ex)
                {
                  log.LogInformation($"Deleting the the item {showObject.Id} for {showObject.Partition} has failed :: {ex.Message}");
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
