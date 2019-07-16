
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
//using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;
//using Microsoft.Azure.Documents;
//using Microsoft.Azure.Documents.Client;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using System;
//using System.Linq;
//using System.Net.Http;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Theatreers.Show.Models;
//using Theatreers.Show.Utils;

//namespace Theatreers.Show.Functions
//{
//  public static class CreateShowImageObject
//  {
//    [FunctionName("CreateShowImageObjectByOrchestrator")]

//    public static async Task<IActionResult> CreateShowImageObjectByOrchestrator(
//      [OrchestrationTrigger] DurableOrchestrationContext context,
//      ILogger log,
//      [CosmosDB(
//        databaseName: "theatreers",
//        collectionName: "shows",
//        ConnectionStringSetting = "cosmosConnectionString"
//      )] IAsyncCollector<CosmosBaseObject<Models.ImageObject>> outputs
//    )
//    {
//      //Take the input as a string from the orchestrator function context
//      //Deserialize into a transport object
//      string rawRequestBody = context.GetInput<string>();
//      MessageObject<ShowObject> message = JsonConvert.DeserializeObject<MessageObject<ShowObject>>(rawRequestBody);

//      //Leverage the Cognitive Services Bing Search API and log out the action
//      IImageSearchClient client = new ImageSearchClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
//      log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Searching for associated images");
//      Images imageResults = client.Images.SearchAsync(query: $"{message.Body.InnerObject.ShowName} (Musical)").Result;

//      //Initialise a temporaryObject and loop through the results
//      //For each result, create a new NewsObject which has a condensed set 
//      //of properties, for storage in CosmosDB in line with the show data model
//      //Once looped through send an OK Result
//      //TODO: There is definitely a better way of doing this, but got a rough working approach out
//      CosmosBaseObject<Models.ImageObject> tempObject = new CosmosBaseObject<Models.ImageObject>()
//      {
//        ShowId = message.Body.ShowId,
//        Doctype = "image",
//        CreatedAt = DateTime.Now
//      };
//      foreach (Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models.ImageObject image in imageResults.Value)
//      {
//        try
//        {
//          tempObject.InnerObject = new Models.ImageObject()
//          {
//            ContentUrl = image.ContentUrl,
//            HostPageUrl = image.HostPageUrl,
//            ImageId = image.ImageId,
//            Name = image.Name
//          };
//          await outputs.AddAsync(tempObject);
//          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Creation Success :: Image ID: {tempObject.InnerObject.ImageId} ");
//        }
//        catch (Exception ex)
//        {
//          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Image Creation Fail ::  :: Image ID: {tempObject.InnerObject.ImageId} - {ex.Message}");
//          return new BadRequestResult();
//        }
//        finally
//        {
//          IDisposable disposable = tempObject as IDisposable;
//          if (disposable != null) disposable.Dispose();
//        }
//      }

//      return new OkResult();
//    }


//    [FunctionName("CreateShowImageObjectByHttp")]

//    public static async Task<IActionResult> CreateShowImageObjectByHttpAsync(
//      [HttpTrigger(
//        AuthorizationLevel.Anonymous,
//        methods: "post",
//        Route = "show/{id}/image"
//      )] HttpRequestMessage req,
//      ILogger log,
//      [CosmosDB(
//        databaseName: "theatreers",
//        collectionName: "shows",
//        ConnectionStringSetting = "cosmosConnectionString"
//      )]  IDocumentClient documentClient,
//      ClaimsPrincipal identity
//    )
//    {
//      if (identity != null && identity.Identity.IsAuthenticated)
//      {
//        Uri showCollectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");
//        string correlationId = Guid.NewGuid().ToString();
//        CosmosBaseObject<Models.ImageObject> submitObject = null;
//        String requestId = req.RequestUri.AbsolutePath.Replace($"/api/show/", "").Replace($"/image", "");
//        Models.ImageObject message = new Models.ImageObject();

//        var docExists = documentClient.CreateDocumentQuery<CosmosBaseObject<ShowObject>>(showCollectionUri)
//                           .Where(doc => doc.ShowId == requestId)
//                           .Select(doc => doc.Id)
//                           .AsEnumerable()
//                           .Any();

//        if (docExists)
//        {
//          try
//          {
//            //Take the input as a string from the orchestrator function context
//            //Deserialize into a transport object
//            message = JsonConvert.DeserializeObject<Models.ImageObject>(await req.Content.ReadAsStringAsync());
//            message.ImageId = "manual";

//            submitObject = new CosmosBaseObject<Models.ImageObject>()
//            {
//              Doctype = "image",
//              ShowId = requestId,
//              InnerObject = message
//            };

//            await documentClient.UpsertDocumentAsync(showCollectionUri, submitObject);
//            log.LogInformation($"[Request Correlation ID: {correlationId}] :: Image Creation Success :: Image ID: {submitObject.InnerObject.ImageId} ");
//          }
//          catch (Exception ex)
//          {
//            log.LogInformation($"[Request Correlation ID: {correlationId}] :: Image Creation Fail ::  :: Image ID: {submitObject.InnerObject.ImageId} - {ex.Message}");
//            return new BadRequestResult();
//          }
//          return new OkResult();
//        }
//        else
//        {
//          return new NotFoundResult();
//        }
//      }
//      else
//      {
//        return new UnauthorizedResult();
//      }
//    }
//  }
//}
