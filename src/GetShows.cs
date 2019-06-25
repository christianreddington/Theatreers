
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Theatreers.Show
{
  public static class GetShows
  {
    [FunctionName("GetShowsAsync")]
    public static async Task<IActionResult> GetShowAsync(
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
      string CorrelationId = Guid.NewGuid().ToString();
      String requestId = req.HttpContext.Request.Path.ToString().Replace("/api/shows/", "");

      Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "showlist");
      dynamic results = documentClient.CreateDocumentQuery<ShowListObject>(collectionUri)
                                  .Where(c => c.partition == requestId)
                                            .AsEnumerable();

      if (results == null)
      {
        log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: GetShows API Request failure :: ID {requestId}");
        return new NotFoundResult();
      }
      else
      {
        log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: GetShows API Request success :: ID {requestId}");
        return new OkObjectResult(results);
      }
    }
  }
}