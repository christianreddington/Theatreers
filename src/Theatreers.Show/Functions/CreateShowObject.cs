using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Theatreers.Core.Models;
using Theatreers.Show.Models;

namespace Theatreers.Show.Functions
{
  public static class CreateShowObject
  {
    private const string databaseName = "theatreers";
    private const string collectionName = "shows";

    [FunctionName("CreateShowObjectByOrchestrator")]
    public static async Task<IActionResult> CreateShowObjectAsync(
      [OrchestrationTrigger] DurableOrchestrationContext context,
      ILogger log,
      [CosmosDB(
        databaseName: databaseName,
        collectionName: collectionName,
        ConnectionStringSetting = "cosmosConnectionString"
      )] IDocumentClient documentClient
    )
    {
      //Take the input as a string from the orchestrator function context
      //Deserialize into a transport and "returned" object
      //These have subtly different types, the latter having fewer properties for storage in CosmosDB
      string rawRequestBody = context.GetInput<string>();
      MessageObject<ShowObject> message = JsonConvert.DeserializeObject<MessageObject<ShowObject>>(rawRequestBody);

      message.Body.Doctype = DocTypes.Show;
      message.Body.Id = message.Body.Partition;
      message.Body.CreatedAt = DateTime.Now;

      Actions.Actions action = new Show.Actions.Actions(databaseName, collectionName);

      try
      {
        await action.CreateShowAsync(documentClient, message);
      }
      catch (Exception ex)
      {
        log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of Show {message.Body.ShowName} failed :: {ex.Message}");
        return new BadRequestObjectResult($"There was an error: {ex.Message}");
      }

      log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Created of Show {message.Body.ShowName} succeeded");
      return new OkResult();
    }

    [FunctionName("CreateShowObjectByHttp")]
    public static async Task<IActionResult> CreateShowObjectByHttpAsync(
      [HttpTrigger(
        AuthorizationLevel.Anonymous,
        "POST",
        Route = "show/show"
      )]HttpRequestMessage req,
      [CosmosDB(
        databaseName: "theatreers",
        collectionName: "shows",
        ConnectionStringSetting = "cosmosConnectionString"
      )] IDocumentClient documentClient,
      ClaimsPrincipal identity,
      ILogger log
        )
    {
      if (identity != null && identity.Identity.IsAuthenticated)
      {
        string showId = Guid.NewGuid().ToString();

        MessageObject<ShowObject> message = new MessageObject<ShowObject>()
        {
          Headers = new MessageHeaders()
          {
            RequestCorrelationId = Guid.NewGuid().ToString(),
            RequestCreatedAt = DateTime.Now
          },
          Body = JsonConvert.DeserializeObject<ShowObject>(await req.Content.ReadAsStringAsync())
        };

        /*message.BOdy.

        new ShowObject()
        {
          CreatedAt = DateTime.Now,
          Doctype = "show",
          Id = showId,
          S
                    InnerObject = JsonConvert.DeserializeObject<ShowObject>(await req.Content.ReadAsStringAsync()),
          Partition = showId
        }*/

        Actions.Actions action = new Show.Actions.Actions(databaseName, collectionName);

        try
        {
          await action.CreateShowAsync(documentClient, message);
        }
        catch (Exception ex)
        {
          log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of Show {message.Body.ShowName} failed :: {ex.Message}");
          return new BadRequestObjectResult($"There was an error: {ex.Message}");
        }

        log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Created of Show {message.Body.ShowName} succeeded");
        return new OkResult();
      } else {
        return new UnauthorizedResult();
      }
    }
  }
}
