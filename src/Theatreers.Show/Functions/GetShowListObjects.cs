
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Theatreers.Show.Utils;

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
      ILogger log
    )
    {
      return await ShowServiceHelper.GetObjectsAsync("showList", req, documentClient, log, "showlist", "shows");
    }
  }
}