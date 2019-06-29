
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.NewsSearch;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Theatreers.Show.Models;

namespace Theatreers.Show.Functions
{
  public static class CreateShowNewsObject
  {
    [FunctionName("CreateShowNewsObjectByOrchestrator")]
    public static async Task<IActionResult> CreateShowNewsObjectByOrchestratorAsync(
      [OrchestrationTrigger] DurableOrchestrationContext context,
      ILogger log,
      [CosmosDB(
        databaseName: "theatreers",
        collectionName: "shows",
        ConnectionStringSetting = "cosmosConnectionString"
      )] IAsyncCollector<CosmosBaseObject<NewsObject>> outputs
    )
    {
      //Take the input as a string from the orchestrator function context
      //Deserialize into a transport object
      string rawRequestBody = context.GetInput<string>();
      MessageObject<ShowObject> message = JsonConvert.DeserializeObject<MessageObject<ShowObject>>(rawRequestBody);

      //Leverage the Cognitive Services Bing Search API and log out the action
      INewsSearchClient client = new NewsSearchClient(new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("bingSearchSubscriptionKey")));
      log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Searching for associated images");
      Microsoft.Azure.CognitiveServices.Search.NewsSearch.Models.News newsResults = client.News.SearchAsync(query: $"{message.Body.InnerObject.ShowName} (Musical)").Result;

      //Initialise a temporaryObject and loop through the results
      //For each result, create a new NewsObject which has a condensed set 
      //of properties, for storage in CosmosDB in line with the show data model
      //Once looped through send an OK Result
      //TODO: There is definitely a better way of doing this, but got a rough working approach out
      CosmosBaseObject<NewsObject> tempObject = new CosmosBaseObject<NewsObject>();
      foreach (Microsoft.Azure.CognitiveServices.Search.NewsSearch.Models.NewsArticle newsItem in newsResults.Value)
      {
        try
        {
          tempObject.InnerObject = new NewsObject()
          {
            Name = newsItem.Name,
            Url = newsItem.Url,
            DatePublished = newsItem.DatePublished,
            BingId = newsItem.BingId
          };
          tempObject.ShowId = message.Body.ShowId;
          tempObject.Doctype = "news";
          tempObject.CreatedAt = DateTime.Now;
          await outputs.AddAsync(tempObject);
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: News Article Creation Success :: Image ID: {tempObject.InnerObject.BingId} ");
        }
        catch (Exception ex)
        {
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: News Article Creation Fail ::  :: Image ID: {tempObject.InnerObject.BingId} - {ex.Message}");
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


    [FunctionName("CreateShowNewsObjectByHttp")]

    public static async Task<IActionResult> CreateShowImageObjectByHttpAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "post",
        Route = "show/{id}/news"
      )] HttpRequestMessage req,
      ILogger log,
      [CosmosDB(
        databaseName: "theatreers",
        collectionName: "shows",
        ConnectionStringSetting = "cosmosConnectionString"
      )]  IDocumentClient documentClient,
      ClaimsPrincipal identity
    )
    {
      if (identity != null && identity.Identity.IsAuthenticated)
      {
        Uri showCollectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");
        string correlationId = Guid.NewGuid().ToString();
        CosmosBaseObject<Models.NewsObject> submitObject = null;
        String requestId = req.RequestUri.AbsolutePath.Replace($"/api/show/", "").Replace($"/news", "");
        Models.NewsObject message = new Models.NewsObject();

        var docExists = documentClient.CreateDocumentQuery<CosmosBaseObject<ShowObject>>(showCollectionUri)
                           .Where(doc => doc.ShowId == requestId)
                           .Select(doc => doc.Id)
                           .AsEnumerable()
                           .Any();

        if (docExists)
        {
          try
          {
            //Take the input as a string from the orchestrator function context
            //Deserialize into a transport object
            message = JsonConvert.DeserializeObject<Models.NewsObject>(await req.Content.ReadAsStringAsync());
            message.BingId = "manual";

            submitObject = new CosmosBaseObject<Models.NewsObject>()
            {
              Doctype = "news",
              ShowId = requestId,
              InnerObject = message
            };

            await documentClient.UpsertDocumentAsync(showCollectionUri, submitObject);
            log.LogInformation($"[Request Correlation ID: {correlationId}] :: Image Creation Success :: Image ID: {submitObject.InnerObject.BingId} ");
          }
          catch (Exception ex)
          {
            log.LogInformation($"[Request Correlation ID: {correlationId}] :: Image Creation Fail ::  :: Image ID: {submitObject.InnerObject.BingId} - {ex.Message}");
            return new BadRequestResult();
          }
          return new OkResult();
        }
        else
        {
          return new NotFoundResult();
        }
      }
      else
      {
        return new UnauthorizedResult();
      }
    }
  }
}
