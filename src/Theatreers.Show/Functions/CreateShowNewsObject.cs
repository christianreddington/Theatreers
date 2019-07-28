
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.CognitiveServices.Search.NewsSearch;
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
//using Theatreers.Core.Models;
//using Theatreers.Show.Models;

//namespace Theatreers.Show.Functions
//{
//  public static class CreateShowNewsObject
//  {

//    private const string databaseName = "theatreers";
//    private const string collectionName = "shows";

//    [FunctionName("CreateShowNewsObjectByOrchestrator")]
//    public static async Task<IActionResult> CreateShowNewsObjectByOrchestratorAsync(
//      [OrchestrationTrigger] DurableOrchestrationContext context,
//      ILogger log,
//      [CosmosDB(
//        databaseName: databaseName,
//        collectionName: collectionName,
//        ConnectionStringSetting = "cosmosConnectionString"
//      )] IDocumentClient documentClient
//    )
//    {
//      //Take the input as a string from the orchestrator function context
//      //Deserialize into a transport object
//      string rawRequestBody = context.GetInput<string>();
//      MessageObject<ShowObject> message = JsonConvert.DeserializeObject<MessageObject<ShowObject>>(rawRequestBody);

//      //Leverage the Cognitive Services Bing Search API and log out the action
//      INewsSearchClient client = new NewsSearchClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
//      log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Searching for associated images");
//      Microsoft.Azure.CognitiveServices.Search.NewsSearch.Models.News newsResults = client.News.SearchAsync(query: $"{message.Body.ShowName} (Musical)").Result;

//      //Initialise a temporaryObject and loop through the results
//      //For each result, create a new NewsObject which has a condensed set 
//      //of properties, for storage in CosmosDB in line with the show data model
//      //Once looped through send an OK Result
//      //TODO: There is definitely a better way of doing this, but got a rough working approach out
//      MessageObject<NewsObject> _object = new MessageObject<NewsObject>()
//      {
//        Body = new NewsObject()
//        {
//          CreatedAt = DateTime.Now,
//          Doctype = DocTypes.News,
//          Partition = message.Body.Partition
//        },
//        Headers = new MessageHeaders(){
//          RequestCorrelationId = message.Headers.RequestCorrelationId,
//          RequestCreatedAt = DateTime.Now
//        }
//      };
//      foreach (Microsoft.Azure.CognitiveServices.Search.NewsSearch.Models.NewsArticle newsItem in newsResults.Value)
//      {
//        try
//        {
//          _object.Body = new NewsObject()
//          {
//            BingId = newsItem.BingId,
//            DatePublished = newsItem.DatePublished,
//            Name = newsItem.Name,
//            Url = newsItem.Url
//          };
//          Actions.Actions action = new Show.Actions.Actions(databaseName, collectionName);
//          await action.CreateNewsAsync(documentClient, _object);
//          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: News Article Creation Success :: Image ID: {_object.Body.BingId} ");
//        }
//        catch (Exception ex)
//        {
//          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: News Article Creation Fail ::  :: Image ID: {_object.Body.BingId} - {ex.Message}");
//          return new BadRequestResult();
//        }
//        finally
//        {
//          IDisposable disposable = _object as IDisposable;
//          if (disposable != null) disposable.Dispose();
//        }
//      }

//      return new OkResult();
//    }


//    [FunctionName("CreateShowNewsObjectByHttpAsync")]
//    public static async Task<IActionResult> CreateShowNewsObjectByHttpAsync(
//      [HttpTrigger(
//        AuthorizationLevel.Anonymous,
//        methods: "post",
//        Route = "show/{id}/news"
//      )]HttpRequestMessage req,
//      [CosmosDB(
//        databaseName: databaseName,
//        collectionName: collectionName,
//        ConnectionStringSetting = "cosmosConnectionString"
//      )] IDocumentClient documentClient,
//      ClaimsPrincipal identity,
//      string id,
//      ILogger log
//        )
//    {
//      if (identity != null && identity.Identity.IsAuthenticated)
//      {
//        NewsObject inputObject = JsonConvert.DeserializeObject<NewsObject>(await req.Content.ReadAsStringAsync());

//        MessageObject <NewsObject> message = new MessageObject<NewsObject>()
//        {
//          Headers = new MessageHeaders()
//          {
//            RequestCorrelationId = Guid.NewGuid().ToString(),
//            RequestCreatedAt = DateTime.Now
//          },
//          Body = new NewsObject()
//          {
//            CreatedAt = DateTime.Now,
//            Doctype = DocTypes.News,
//            BingId = inputObject.BingId,
//            DatePublished = inputObject.DatePublished,
//            Name = inputObject.Name,
//            Url = inputObject.Url,
//            Partition = id
//          }
//        };

//        Actions.Actions action = new Show.Actions.Actions(databaseName, collectionName);

//        try
//        {
//          await action.CreateNewsAsync(documentClient, message);
//        }
//        catch (Exception ex)
//        {
//          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of News Article {message.Body.Name} failed :: {ex.Message}");
//          return new BadRequestObjectResult($"There was an error: {ex.Message}");
//        }

//        log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of News Article {message.Body.Name} succeeded");
//        return new OkResult();
//      }
//      else
//      {
//        return new UnauthorizedResult();
//      }
//    }
//  }
//}
