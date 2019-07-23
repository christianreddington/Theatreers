
using Dynamitey.DynamicObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Theatreers.Core.Models;
using Theatreers.Show.Models;

namespace Theatreers.Show.Functions
{
  public static class GetShowListObjects
  {
    [FunctionName("GetShowListObjects")]
    public static async Task<IActionResult> GetShowListObjectsAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        "get",
        Route = "shows/{letter}"
      )]HttpRequest req,
      [CosmosDB(
        databaseName: "theatreers",
        collectionName: "showlist",
        ConnectionStringSetting = "cosmosConnectionString",
        PartitionKey = "{letter}"
      )] IDocumentClient documentClient,
      string letter,
      ILogger log
    )
    {
      Actions.Actions action = new Show.Actions.Actions("theatreers", "showlist");
      ICollection<CosmosBaseObject<ShowListObject>> _object = await action.GetShowsAsync(documentClient, letter);

      if (_object.Count > 0 && _object != null)
      {
        return new OkObjectResult(_object);
      }

      return new NotFoundResult();
    }
  }
}