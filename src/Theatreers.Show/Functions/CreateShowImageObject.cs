using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Theatreers.Core.Models;
using Theatreers.Show.Models;
using ImageObject = Theatreers.Show.Models.ImageObject;

namespace Theatreers.Show.Functions
{
  public static class CreateShowImageObject
  {
    private const string databaseName = "theatreers";
    private const string collectionName = "shows";

    [FunctionName("CreateShowImageObjectByOrchestrator")]

    public static async Task<IActionResult> CreateShowImageObjectByOrchestrator(
      [OrchestrationTrigger] DurableOrchestrationContext context,
      ILogger log,
      [CosmosDB(
        databaseName: "theatreers",
        collectionName: "shows",
        ConnectionStringSetting = "cosmosConnectionString"
      )] IDocumentClient documentClient
    )
    {
      //Take the input as a string from the orchestrator function context
      //Deserialize into a transport object
      string rawRequestBody = context.GetInput<string>();
      MessageObject<ShowObject> message = JsonConvert.DeserializeObject<MessageObject<ShowObject>>(rawRequestBody);

      //Leverage the Cognitive Services Bing Search API and log out the action
      IImageSearchClient client = new ImageSearchClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
      log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Searching for associated images");
      Images imageResults = client.Images.SearchAsync(query: $"{message.Body.ShowName} (Musical)").Result;

      //Initialise a temporaryObject and loop through the results
      //For each result, create a new NewsObject which has a condensed set 
      //of properties, for storage in CosmosDB in line with the show data model
      //Once looped through send an OK Result
      //TODO: There is definitely a better way of doing this, but got a rough working approach out
      MessageObject<ImageObject> _object = new MessageObject<ImageObject>()
      {
        Body = new ImageObject()
        {
          CreatedAt = DateTime.Now,
          Doctype = DocTypes.News,
          Partition = message.Body.Partition
        },
        Headers = new MessageHeaders()
        {
          RequestCorrelationId = message.Headers.RequestCorrelationId,
          RequestCreatedAt = DateTime.Now
        }
      };
      foreach (Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models.ImageObject image in imageResults.Value)
      {
        try
        {
          _object.Body = new Models.ImageObject()
          {
            ContentUrl = image.ContentUrl,
            HostPageUrl = image.HostPageUrl,
            ImageId = image.ImageId,
            Name = image.Name
          };
          Actions.Actions action = new Show.Actions.Actions(databaseName, collectionName);
          await action.CreateImageAsync(documentClient, _object);
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Creation Success :: Image ID: {_object.Body.ImageId} ");
        }
        catch (Exception ex)
        {
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Creation Fail ::  :: Image ID: {_object.Body.ImageId} - {ex.Message}");
          return new BadRequestResult();
        }
        finally
        {
          IDisposable disposable = _object as IDisposable;
          if (disposable != null) disposable.Dispose();
        }
      }

      return new OkResult();
    }


    [FunctionName("CreateShowImageObjectByHttpAsync")]
    public static async Task<IActionResult> CreateShowImageObjectByHttpAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "post",
        Route = "show/{id}/image"
      )]HttpRequestMessage req,
      [CosmosDB(
        databaseName: databaseName,
        collectionName: collectionName,
        ConnectionStringSetting = "cosmosConnectionString"
      )] IDocumentClient documentClient,
      ClaimsPrincipal identity,
      string id,
      ILogger log
        )
    {
      if (identity != null && identity.Identity.IsAuthenticated)
      {

        ImageObject inputObject = JsonConvert.DeserializeObject<ImageObject>(await req.Content.ReadAsStringAsync());
        MessageObject<ImageObject> message = new MessageObject<ImageObject>()
        {
          Headers = new MessageHeaders()
          {
            RequestCorrelationId = Guid.NewGuid().ToString(),
            RequestCreatedAt = DateTime.Now
          },
          Body = new ImageObject()
          {
            CreatedAt = DateTime.Now,
            Doctype = DocTypes.News,
            ContentUrl = inputObject.ContentUrl,
            HostPageUrl = inputObject.HostPageUrl,
            ImageId = $"manual-{Guid.NewGuid().ToString()}",
            Partition = id
          }
        };

        Actions.Actions action = new Show.Actions.Actions(databaseName, collectionName);

        try
        {
          await action.CreateImageAsync(documentClient, message);
        }
        catch (Exception ex)
        {
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of Image {message.Body.Name} failed :: {ex.Message}");
          return new BadRequestObjectResult($"There was an error: {ex.Message}");
        }

        log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of Image {message.Body.Name} succeeded");
        return new OkResult();
      }
      else
      {
        return new UnauthorizedResult();
      }
    }
  }
}
