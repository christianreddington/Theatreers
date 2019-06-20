using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Theatreers.Show
{
    public static class DeleteShow
    {        
        [FunctionName("DeleteShow")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, methods: "delete", Route = "deleteshow")] HttpRequestMessage req,
            ILogger log,
            [CosmosDB(
                databaseName: "theatreers",
                collectionName: "shows",
                ConnectionStringSetting = "cosmosConnectionString"
            )] IAsyncCollector<ShowObject> outputs,
            ClaimsPrincipal identity)
        {
            if (identity != null && identity.Identity.IsAuthenticated)
            {
                string CorrelationId = Guid.NewGuid().ToString();
                Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");

                //Take the input as a string from the orchestrator function context
                //Deserialize into a transport and "returned" object
                //These have subtly different types, the latter having fewer properties for storage in CosmosDB

                ShowObject returnedObject = JsonConvert.DeserializeObject<ShowObject>(await req.Content.ReadAsStringAsync());
                returnedObject.doctype = "show";
                returnedObject.ttl = "1";

                //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
                //If unsuccessful, catch any exception, log that and throw a BadRequestResult
                try
                {
                    await outputs.AddAsync(returnedObject);
                    log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: Show Deletion Success");
                    return new OkResult();
                }
                catch (Exception ex)
                {
                    log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: Show Deletion Fail :: {ex.Message}");
                    return new BadRequestResult();
                }
            }
            else
            {
                return new UnauthorizedResult();
            }
        }
    }
}
