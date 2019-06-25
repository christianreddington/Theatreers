
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Theatreers.Show
{
  public static class CreateShowImageObject
  {
    [FunctionName("CreateShowImageObject")]

    public static async Task<IActionResult> RunAsync(
      [OrchestrationTrigger] DurableOrchestrationContext context,
      ILogger log,
      [CosmosDB(
        databaseName: "theatreers", 
        collectionName: "shows", 
        ConnectionStringSetting = "cosmosConnectionString"
      )] IAsyncCollector<CosmosBaseObject<ImageObject>> outputs
    )
    {
      //Take the input as a string from the orchestrator function context
      //Deserialize into a transport object
      string rawRequestBody = context.GetInput<string>();
      MessageObject<ShowObject> message = JsonConvert.DeserializeObject<MessageObject<ShowObject>>(rawRequestBody);

      //Leverage the Cognitive Services Bing Search API and log out the action
      IImageSearchClient client = new ImageSearchClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
      log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Searching for associated images");
      Images imageResults = client.Images.SearchAsync(query: $"{message.Body.innerobject.showName} (Musical)").Result;

      //Initialise a temporaryObject and loop through the results
      //For each result, create a new NewsObject which has a condensed set 
      //of properties, for storage in CosmosDB in line with the show data model
      //Once looped through send an OK Result
      //TODO: There is definitely a better way of doing this, but got a rough working approach out
      CosmosBaseObject<ImageObject> tempObject = new CosmosBaseObject<ImageObject>();
      foreach (Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models.ImageObject image in imageResults.Value)
      {
        try
        {
          tempObject.innerobject = new ImageObject()
          {
            contentUrl = image.ContentUrl,
            hostPageUrl = image.HostPageUrl,
            imageId = image.ImageId,
            name = image.Name
          };
          tempObject.showId = message.Body.showId;
          tempObject.doctype = "image";
          await outputs.AddAsync(tempObject);
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Creation Success :: Image ID: {tempObject.innerobject.imageId} ");
        }
        catch (Exception ex)
        {
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Creation Fail ::  :: Image ID: {tempObject.innerobject.imageId} - {ex.Message}");
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
