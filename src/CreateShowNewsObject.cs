
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.NewsSearch;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theatreers.Show
{
    public static class CreateShowNewsObject
    {
        [FunctionName("CreateShowNewsObject")]

        public static async Task<IActionResult> RunAsync(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log,
            [CosmosDB(databaseName: "theatreers", 
            collectionName: "shows", 
            ConnectionStringSetting = "cosmosConnectionString")] IAsyncCollector<NewsObject> outputs
        )
        {
            //Take the input as a string from the orchestrator function context
            //Deserialize into a transport object
            string rawRequestBody = context.GetInput<string>();
            DecoratedShowObject transitObject = JsonConvert.DeserializeObject<DecoratedShowObject>(rawRequestBody);

            //Leverage the Cognitive Services Bing Search API and log out the action
            INewsSearchClient client = new NewsSearchClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
            log.LogInformation($"[Request Correlation ID: {transitObject.MessageProperties.RequestCorrelationId}] :: Searching for associated images");
            Microsoft.Azure.CognitiveServices.Search.NewsSearch.Models.News newsResults = client.News.SearchAsync(query: $"{transitObject.showName} (Musical)").Result;

            //Initialise a temporaryObject and loop through the results
            //For each result, create a new NewsObject which has a condensed set 
            //of properties, for storage in CosmosDB in line with the show data model
            //Once looped through send an OK Result
            //TODO: There is definitely a better way of doing this, but got a rough working approach out
            NewsObject tempObject = new NewsObject();
            foreach (Microsoft.Azure.CognitiveServices.Search.NewsSearch.Models.NewsArticle newsItem in newsResults.Value)
            {
                try
                {
                    tempObject.name = newsItem.Name;
                    tempObject.url = newsItem.Url;
                    tempObject.DatePublished = newsItem.DatePublished;
                    tempObject.BingId = newsItem.BingId;
                    tempObject.showId = transitObject.showId;
                    tempObject.doctype = "news";
                    await outputs.AddAsync(tempObject);
                    log.LogInformation($"[Request Correlation ID: {transitObject.MessageProperties.RequestCorrelationId}] :: News Article Creation Success :: Image ID: {tempObject.BingId} ");
                }
                catch (Exception ex)
                {
                    log.LogInformation($"[Request Correlation ID: {transitObject.MessageProperties.RequestCorrelationId}] :: News Article Creation Fail ::  :: Image ID: {tempObject.BingId} - {ex.Message}");
                    return new BadRequestResult();
                }
                finally
                {
                    IDisposable disposable = tempObject as IDisposable;
                    if (disposable != null) disposable.Dispose();
                }
            }

            return new OkResult();
        }
    }
}
