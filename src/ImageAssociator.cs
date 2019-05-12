
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theatreers.Show
{
    public static class ImageAssociator
    {
        [FunctionName("SubmitImageAsync")]

        public static async Task<IActionResult> RunAsync(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log,
            [CosmosDB(databaseName: "theatreers", collectionName: "items", ConnectionStringSetting = "cosmosConnectionString")] IAsyncCollector<ImageObject> outputs
        )
        {
            //Take the input as a string from the orchestrator function context
            //Deserialize into a transport object
            string rawRequestBody = context.GetInput<string>();
            DecoratedShowMessage transitObject = JsonConvert.DeserializeObject<DecoratedShowMessage>(rawRequestBody);

            //Leverage the Cognitive Services Bing Search API and log out the action
            IImageSearchClient client = new ImageSearchClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
            log.LogInformation($"[Request Correlation ID: {transitObject.MessageProperties.RequestCorrelationId}] :: Searching for associated images");
            Images imageResults = client.Images.SearchAsync(query: transitObject.ShowName).Result;

            //Initialise a temporaryObject and loop through the results
            //For each result, create a new NewsObject which has a condensed set 
            //of properties, for storage in CosmosDB in line with the show data model
            //Once looped through send an OK Result
            //TODO: There is definitely a better way of doing this, but got a rough working approach out
            ImageObject tempObject = new ImageObject();
            foreach (Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models.ImageObject image in imageResults.Value)
            {
                try
                {
                    tempObject.contentUrl = image.ContentUrl;
                    tempObject.hostPageUrl = image.HostPageUrl;
                    tempObject.imageId = image.ImageId;
                    tempObject.name = image.Name;
                    tempObject.partitionKey = transitObject.partitionKey;
                    tempObject.doctype = "image";
                    await outputs.AddAsync(tempObject);
                    log.LogInformation($"[Request Correlation ID: {transitObject.MessageProperties.RequestCorrelationId}] :: Image Creation Success :: Image ID: {tempObject.imageId} ");
                }
                catch (Exception ex)
                {
                    log.LogInformation($"[Request Correlation ID: {transitObject.MessageProperties.RequestCorrelationId}] :: Image Creation Fail ::  :: Image ID: {tempObject.imageId} - {ex.Message}");
                    return new BadRequestResult();
                } finally
                {
                    IDisposable disposable = tempObject as IDisposable;
                    if (disposable != null) disposable.Dispose();
                }
            }

            return new OkResult();
        }
    }
}
