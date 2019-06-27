
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
  public static class GetImageObjectByShow
  {
    [FunctionName("GetImageObjectByShow")]
    public static async Task<IActionResult> GetImageObjectByShowAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = "show/{id}/image")]HttpRequest req,
        [CosmosDB(
                databaseName: "theatreers",
                collectionName: "shows",
                ConnectionStringSetting = "cosmosConnectionString",
                Id = "{id}",
                PartitionKey = "{id}")] IDocumentClient documentClient,
        ILogger log)
    {
      return await ShowServiceHelper.GetObjectsAsync("image", req, documentClient, log, "shows", "show");
    }
  }
}