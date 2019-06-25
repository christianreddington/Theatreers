
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
  public static class GetShow
  {
    [FunctionName("GetShow_ShowObjects")]
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
      ILogger log
    )
    {
      string CorrelationId = Guid.NewGuid().ToString();
      String requestId = req.HttpContext.Request.Path.ToString().Replace("/api/show/", "").Replace("/show", "");

      Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");
      dynamic results = documentClient.CreateDocumentQuery<CosmosBaseObject<ShowObject>>(collectionUri)
                                  .Where(c => c.showId == requestId && c.doctype == "show")
                                  .AsEnumerable();

      if (results == null)
      {
        log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: GetShow API Request failure :: ID {requestId}");
        return new NotFoundResult();
      }
      else
      {
        log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: GetShow API Request success :: ID {requestId}");
        return new OkObjectResult(results);
      }
    }


    [FunctionName("GetShow_ImageObjects")]
    public static async Task<IActionResult> GetShowImageObjectsAsync(
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
      string CorrelationId = Guid.NewGuid().ToString();
      String requestId = req.HttpContext.Request.Path.ToString().Replace("/api/show/", "").Replace("/image", "");

      Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");
      dynamic results = documentClient.CreateDocumentQuery<CosmosBaseObject<ImageObject>>(collectionUri)
                                  .Where(c => c.showId == requestId && c.doctype == "image")
                                  .AsEnumerable();

      if (results == null)
      {
        log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: GetShow API Request failure :: ID {requestId}");
        return new NotFoundResult();
      }
      else
      {
        log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: GetShow API Request success :: ID {requestId}");
        return new OkObjectResult(results);
      }
    }


    [FunctionName("GetShow_NewsObjects")]
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
      string CorrelationId = Guid.NewGuid().ToString();
      String requestId = req.HttpContext.Request.Path.ToString().Replace("/api/show/", "").Replace("/news", "");

      Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");
      dynamic results = documentClient.CreateDocumentQuery<CosmosBaseObject<NewsObject>>(collectionUri)
                                  .Where(c => c.showId == requestId && c.doctype == "news")
                                  .AsEnumerable();

      if (results == null)
      {
        log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: GetShow API Request failure :: ID {requestId}");
        return new NotFoundResult();
      }
      else
      {
        log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: GetShow API Request success :: ID {requestId}");
        return new OkObjectResult(results);
      }
    }
  }
}