using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
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
            [CosmosDB(databaseName: "theatreers", collectionName: "items", ConnectionStringSetting = "cosmosConnectionString")] IAsyncCollector<ShowMessage> outputs
        )
        {
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
