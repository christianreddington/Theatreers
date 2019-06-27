
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
        ILogger log)
    {
      return await ShowServiceHelper.GetObjectsAsync("news", req, documentClient, log, "shows", "show");
    }
  }
}