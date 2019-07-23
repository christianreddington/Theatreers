
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Theatreers.Core.Models;
using Theatreers.Show.Actions;
using Theatreers.Show.Models;

namespace Theatreers.Show.Functions
{
  public static class GetShowObject
  {
    [FunctionName("GetShowObject")]
    public static async Task<IActionResult> GetShowObjectsAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        "get",
        Route = "show/{id}/show"
      )]HttpRequest req,
      [CosmosDB(
        databaseName: "theatreers",
        collectionName: "shows",
        ConnectionStringSetting = "cosmosConnectionString",
        Id = "{id}",
        PartitionKey = "{id}"
      )] IDocumentClient documentClient,
      string id,
      ILogger log
    )
    {
      Actions.Actions action = new Show.Actions.Actions("theatreers", "shows");
      ShowObject _object = await action.GetShowAsync(documentClient, id);

      if (_object != null)
      {
        return new OkObjectResult(_object);
      }

      return new NotFoundObjectResult($"Sorry, but the show with ID {id} does not exist!");
    }
  }
}