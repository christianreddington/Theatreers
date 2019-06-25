using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Theatreers.Show
{
  public static class CreateShowObject
  {

    [FunctionName("CreateShowObject")]
    public static async Task<IActionResult> RunAsync(
      [OrchestrationTrigger] DurableOrchestrationContext context,
      ILogger log,
      [CosmosDB(
        databaseName: "theatreers",
        collectionName: "shows",
        ConnectionStringSetting = "cosmosConnectionString"
      )] IAsyncCollector<CosmosBaseObject<ShowObject>> outputs
    )
    {
      Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");

      //Take the input as a string from the orchestrator function context
      //Deserialize into a transport and "returned" object
      //These have subtly different types, the latter having fewer properties for storage in CosmosDB
      string rawRequestBody = context.GetInput<string>();
      MessageObject<ShowObject> message = JsonConvert.DeserializeObject<MessageObject<ShowObject>>(rawRequestBody);
      message.Body.doctype = "show";
      message.Body.innerobject.id = message.Body.showId;

      //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
      //If unsuccessful, catch any exception, log that and throw a BadRequestResult
      try
      {
        await outputs.AddAsync(message.Body);
        log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Created of Show {message.Body.innerobject.showName} succeeded");
        return new OkObjectResult(outputs);
      }
      catch (Exception ex)
      {
        log.LogInformation($"[Request Correlation ID: {message.Headers.RequestCorrelationId}] :: Creation of Show {message.Body.innerobject.showName} failed :: {ex.Message}");
        return new BadRequestResult();
      }
    }
  }
}
