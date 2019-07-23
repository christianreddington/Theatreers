//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.Documents.Client;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using System;
//using System.Net.Http;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Theatreers.Show.Models;

//namespace Theatreers.Show.Functions
//{
//  public static class UpdateShow
//  {
//    [FunctionName("UpdateShow")]
//    public static async Task<IActionResult> Run(
//      [HttpTrigger(
//        AuthorizationLevel.Anonymous, 
//        methods: "PUT",
//        Route = "show/{id}")] HttpRequestMessage req,
//      ILogger log,
//      [CosmosDB(
//        databaseName: "theatreers",
//        collectionName: "shows",
//        ConnectionStringSetting = "cosmosConnectionString",
//        Id = "{id}",
//        PartitionKey = "{id}"
//      )] IAsyncCollector<CosmosBaseObject<ShowObject>> outputs,
//      ClaimsPrincipal identity
//    )
//    {
//      if (identity != null && identity.Identity.IsAuthenticated)
//      {
//        string CorrelationId = Guid.NewGuid().ToString();
//        Uri collectionUri = UriFactory.CreateDocumentCollectionUri("theatreers", "shows");

//        //Take the input as a string from the orchestrator function context
//        //Deserialize into a transport and "returned" object
//        //These have subtly different types, the latter having fewer properties for storage in CosmosDB

//        MessageObject<ShowObject> message = new MessageObject<ShowObject>();
//        message.Body.InnerObject = JsonConvert.DeserializeObject<ShowObject>(await req.Content.ReadAsStringAsync());
//        message.Body.Doctype = "show";

//        //If successful, push the output to CosmosDB, log the creation and return an OkObjectResult
//        //If unsuccessful, catch any exception, log that and throw a BadRequestResult
//        try
//        {
//          await outputs.AddAsync(message.Body);
//          log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: Show Creation Success");
//          return new OkResult();
//        }
//        catch (Exception ex)
//        {
//          log.LogInformation($"[Request Correlation ID: {CorrelationId}] :: Show Creation Fail :: {ex.Message}");
//          return new BadRequestResult();
//        }
//      }
//      else
//      {
//        return new UnauthorizedResult();
//      }
//    }
//  }
//}
