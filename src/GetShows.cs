
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
    public static class GetShows
    {
            [FunctionName("GetShowsAsync")]
            public static async Task<IActionResult> GetShowAsync(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get",
                Route = "shows/{letter}")]HttpRequest req,
                [CosmosDB(
                databaseName: "theatreers",
                collectionName: "showsAlphabetised",
                ConnectionStringSetting = "cosmosConnectionString",
                PartitionKey = "{letter}")] IDocumentClient documentClient,
                ILogger log)
            {
                string CorrelationId = Guid.NewGuid().ToString();
                String requestId = req.HttpContext.Request.Path.ToString().Replace("/api/shows/","");

                Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "showsAlphabetised");
                dynamic results = documentClient.CreateDocumentQuery<AlphabetisedShow>(collectionUri)
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