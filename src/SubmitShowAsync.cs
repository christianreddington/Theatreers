using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Theatreers.Show
{
    public static class SubmitShowAsync
    {        
        
        [FunctionName("SubmitShowAsync")]
        public static async Task<IActionResult> RunAsync(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log,
            [CosmosDB(
                databaseName: "theatreers", 
                collectionName: "shows",
                ConnectionStringSetting = "cosmosConnectionString"
            )] IAsyncCollector<ShowMessage> outputs
        )
        {

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");

            //var collectionLink = outputs.get.CreateDocumentCollectionUri(databaseId, collectionId);

            //Take the input as a string from the orchestrator function context
            //Deserialize into a transport and "returned" object
            //These have subtly different types, the latter having fewer properties for storage in CosmosDB
            string rawRequestBody = context.GetInput<string>();
            DecoratedShowMessage transitObject = JsonConvert.DeserializeObject<DecoratedShowMessage>(rawRequestBody);
            ShowMessage returnedObject = JsonConvert.DeserializeObject<ShowMessage>(rawRequestBody);
            returnedObject.doctype = "show";

            //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
            //If unsuccessful, catch any exception, log that and throw a BadRequestResult
            try
            {
                await outputs.AddAsync(returnedObject);
                log.LogInformation($"[Request Correlation ID: {transitObject.MessageProperties.RequestCorrelationId}] :: Show Creation Success");
                return new OkObjectResult(outputs);
            } catch (Exception ex)
            {
                log.LogInformation($"[Request Correlation ID: {transitObject.MessageProperties.RequestCorrelationId}] :: Show Creation Fail :: {ex.Message}");
                return new BadRequestResult();
            }
        }
    }
}
