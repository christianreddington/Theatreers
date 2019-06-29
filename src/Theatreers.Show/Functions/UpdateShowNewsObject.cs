
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;
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
using Theatreers.Show.Utils;

namespace Theatreers.Show.Functions
{
  public static class UpdateShowNewsObject
  {
    [FunctionName("UpdateShowNewsObject")]

    public static async Task<IActionResult> UpdateShowNewsObjectAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        methods: "PUT",
        Route = "show/{id}/news/{newsid}"
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
        String requestId = req.RequestUri.AbsolutePath.Replace($"/api/show/", "").Replace($"/news/", "::");
        String[] ids = requestId.Split("::");
        Models.NewsObject message = new Models.NewsObject();

        var docExists = documentClient.CreateDocumentQuery<CosmosBaseObject<NewsObject>>(showCollectionUri, new FeedOptions { PartitionKey = new PartitionKey(ids[0]) })
                           .Where(doc => doc.Id == ids[1] && doc.Doctype == "news")
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
              Id = ids[1],
              Doctype = "news",
              ShowId = ids[0],
              InnerObject = message
            };

            await documentClient.UpsertDocumentAsync(showCollectionUri, submitObject);
            log.LogInformation($"[Request Correlation ID: {correlationId}] :: News Update Success :: Object ID: {submitObject.Id} ");
          }
          catch (Exception ex)
          {
            log.LogInformation($"[Request Correlation ID: {correlationId}] :: News Update Fail ::  :: Object ID: {submitObject.Id} - {ex.Message}");
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
