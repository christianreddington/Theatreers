
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
  public static class GetNewsObjectByShow
  {
    [FunctionName("GetNewsObjectByShow")]
    public static async Task<IActionResult> GetShowNewsObjectsAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get",
          Route = "show/{id}/news")]HttpRequest req,
        [CosmosDB(
          databaseName: "theatreers",
          collectionName: "shows",
          ConnectionStringSetting = "cosmosConnectionString",
          Id = "{id}",
          PartitionKey = "{id}")] IDocumentClient documentClient,
        string id,
        ILogger log)
    {
      Actions.Actions action = new Show.Actions.Actions("theatreers", "shows");
      ICollection<CosmosBaseObject<NewsObject>> _object = await action.GetNewsByShowAsync(documentClient, id);

      if (_object != null && _object.Count > 0)
      {
        return new OkObjectResult(_object);
      }

      return new NotFoundObjectResult($"Sorry, but the show with ID {id} does not exist!");
    }
  }
}