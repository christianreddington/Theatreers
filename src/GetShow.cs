
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.CognitiveServices.Search;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Threading;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.Net.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Theatreers.Show
{
    public static class GetShow
    {
        [FunctionName("GetShow_ShowObjects")]
        public static async Task<IActionResult> GetShowObjectsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = "show/{id}/show")]HttpRequest req,
            [CosmosDB(
                databaseName: "theatreers",
                collectionName: "shows",
                ConnectionStringSetting = "cosmosConnectionString",
                Id = "{id}",
                PartitionKey = "{id}")] IDocumentClient documentClient,
            ILogger log)
        {
            string CorrelationId = Guid.NewGuid().ToString();
            String requestId = req.HttpContext.Request.Path.ToString().Replace("/api/show/", "").Replace("/show", "");

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");
            dynamic results = documentClient.CreateDocumentQuery<ShowObject>(collectionUri)
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
            dynamic results = documentClient.CreateDocumentQuery<ImageObject>(collectionUri)
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
            dynamic results = documentClient.CreateDocumentQuery<NewsObject>(collectionUri)
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